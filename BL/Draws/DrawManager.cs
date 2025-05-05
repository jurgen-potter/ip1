using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Draws;
using QRCoder;

namespace CitizenPanel.BL.Draws;

public class DrawManager(IDrawRepository drawRepository) : IDrawManager
{
    public IEnumerable<Invitation> AddInvitations(int amount, List<Criteria> criteria, Panel panel)
    {
    QRCodeGenerator qrGenerator = new QRCodeGenerator();
    List<Invitation> invitations = new List<Invitation>();

    Dictionary<Gender, double> genderPercentages = new();
    Dictionary<string, double> ageGroupPercentages = new(); // e.g., "18-25" => 10

    // Step 1: Parse criteria into gender and age group maps
    foreach (var c in criteria)
    {
        if (c.Name.ToUpper() == "GESLACHT")
        {
            foreach (var sub in c.SubCriteria)
            {
                if (sub.Name.ToUpper() == "MAN")
                    genderPercentages[Gender.Male] = sub.Percentage;
                else if (sub.Name.ToUpper() == "VROUW")
                    genderPercentages[Gender.Female] = sub.Percentage;
            }
        }
        else if (c.Name.ToUpper() == "LEEFTIJD")
        {
            foreach (var sub in c.SubCriteria)
                ageGroupPercentages[sub.Name.ToUpper()] = sub.Percentage;
        }
    }

    // Step 2: Cross-multiply to calculate (gender, ageGroup) counts
    Dictionary<(Gender gender, string ageGroup), int> invitationCounts = new();

    foreach (var genderEntry in genderPercentages)
    {
        foreach (var ageEntry in ageGroupPercentages)
        {
            double combinedPercentage = (genderEntry.Value / 100.0) * (ageEntry.Value / 100.0);
            int count = (int)Math.Round(combinedPercentage * amount);

            var key = (genderEntry.Key, ageEntry.Key);
            if (invitationCounts.ContainsKey(key))
                invitationCounts[key] += count;
            else
                invitationCounts[key] = count;
        }
    }

    // Step 3: Create invitations based on the calculated distribution
    foreach (var pair in invitationCounts)
    {
        var gender = pair.Key.gender;
        var ageGroup = pair.Key.ageGroup;
        int count = pair.Value;

        for (int i = 0; i < count; i++)
        {
            int age = ageGroup switch
            {
                "18-25" => 18,
                "26-35" => 26,
                "36-50" => 36,
                "51-60" => 51,
                "60+"   => 61,
                _ => 36
            };

            string code = GenerateCode();
            string qrCodePlace = $"http://whimp24.duckdns.org/MemberRegister/RegisterMember?code={code}";

            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodePlace, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
            string qrCodeString = Convert.ToBase64String(qrCodeAsPngByteArr);

            Invitation invitation = new Invitation
            {
                Code = code,
                Age = age,
                Gender = gender,
                PanelId = panel.Id,
                QRCodeString = qrCodeString
            };

            Invitation newInvitation = drawRepository.CreateInvitation(invitation);
            invitations.Add(newInvitation);
        }
    }

    return invitations;
    }
    
    public Invitation GetInvitationWithCode(string code)
    {
        return drawRepository.ReadInvitationWithCode(code);
    }

    public IEnumerable<Invitation> GetAllInvitationsByPanelId(int panelId)
    {
        return drawRepository.ReadAllInvitationsByPanelId(panelId);
    }

    public Invitation ChangeInvitation(Invitation invitation)
    {
        return drawRepository.UpdateInvitation(invitation);
    }

    public string GenerateCode()
    {
        Random random = new Random();
        string code = String.Empty;
        string codeTemplate = $"0000-0000-0000-0000-0000";
        foreach (char c in codeTemplate)
        {
            char newLetDig = c;
            if (newLetDig == '0')
            {
                int replaceNumber = random.Next(0, 62);
                if (replaceNumber >= 36)
                {
                    newLetDig = (char)('a' + replaceNumber - 36);
                }
                else if (replaceNumber >= 10)
                {
                    newLetDig = (char)('A' + replaceNumber - 10);
                }
                else
                {
                    newLetDig = (char)('0' + replaceNumber);
                }
            }

            code += newLetDig;
        }

        return code;
    }
    
    public Criteria GetCriteria(int criteriaId)
    {
        return drawRepository.ReadCriteria(criteriaId);
    }

    public IEnumerable<Criteria> GetAllCriteria()
    {
        return drawRepository.ReadAllCriteria();
    }

    public SubCriteria GetSubCriteria(int subCriteriaId)
    {
        return drawRepository.ReadSubCriteria(subCriteriaId);
    }

    public IEnumerable<Criteria> GetCriteriaByPanel(int panelId)
    {
        return drawRepository.ReadCriteriaByPanel(panelId);
    }
    
    public Criteria AddCriteria(string name, List<SubCriteria> subCriteria)
    {
        Criteria criteria = new Criteria()
        {
            Name = name,
            SubCriteria = subCriteria
        };
        return drawRepository.CreateCriteria(criteria);
    }

    public SubCriteria AddSubCriteria(string name, double percentage)
    {
        SubCriteria subCriteria = new SubCriteria()
        {
            Name = name,
            Percentage = percentage
        };
        return drawRepository.CreateSubCriteria(subCriteria);
    }
    
    public void EditCriteria(int panelId, IEnumerable<Criteria> criteria)
    {
        drawRepository.UpdateCriteria(panelId, criteria);
    }
    
    public RecruitmentResult CalculateRecruitment(int totalAvailablePotentialPanelmembers, IEnumerable<Criteria> criteriaList)
    {
        int totalToDraw = (int)Math.Round(0.50 * Math.Sqrt(totalAvailablePotentialPanelmembers));
        int reservePool = (int)Math.Ceiling(totalToDraw / 0.08);
        
        var buckets = BuildBuckets(criteriaList.ToList(), totalToDraw);
        
        int totalCount = buckets.Sum(b => b.Count);
        while (totalCount < totalToDraw)
        {
            var bucket = buckets.OrderBy(b => b.Count).First();
            bucket.Count++;
            totalCount++;
        }
        while (totalCount > totalToDraw)
        {
            var bucket = buckets.OrderByDescending(b => b.Count).First();
            bucket.Count--;
            totalCount--;
        }
        
        return new RecruitmentResult
        {
            TotalNeededPanelmembers = totalToDraw,
            ReservePotPanelmembers = reservePool,
            Buckets = buckets
        };
    }

    private static List<RecruitmentBucket> BuildBuckets(List<Criteria> criteriaList, int totalToDraw)
    {
        var buckets = new List<RecruitmentBucket>();

        void Recurse(int depth, List<string> chosenCriteria, List<string> chosenSubs, double accumulatedPct)
        {
            if (depth == criteriaList.Count)
            {
                int count = (int)Math.Round(totalToDraw * accumulatedPct);
                if (count > 0)
                {
                    buckets.Add(new RecruitmentBucket
                    {
                        CriteriaNames = new List<string>(chosenCriteria),
                        SubCriteriaNames = new List<string>(chosenSubs),
                        Count = count
                    });
                }
                return;
            }

            var current = criteriaList[depth];
            foreach (var sub in current.SubCriteria)
            {
                chosenCriteria.Add(current.Name);
                chosenSubs.Add(sub.Name);

                Recurse(
                    depth + 1,
                    chosenCriteria,
                    chosenSubs,
                    accumulatedPct * (sub.Percentage / 100.0));

                chosenCriteria.RemoveAt(chosenCriteria.Count - 1);
                chosenSubs.RemoveAt(chosenSubs.Count - 1); 
            }
        }

        Recurse(0, new List<string>(), new List<string>(), 1.0);
        return buckets;
    }
    
    public bool RemoveInvitation(int invitationId)
    {
        return drawRepository.DeleteInvitation(invitationId);
    }

    public bool RemoveInvitationByEmail(string email)
    {
        return drawRepository.DeleteInvitationByEmail(email);
    }

    public IEnumerable<Criteria> GetInitialCriteria()
    {
        var criteriaList = new List<Criteria>()
        {
            new Criteria()
            {
                Name = "Geslacht",
                SubCriteria = new List<SubCriteria>()
                {
                    new SubCriteria()
                    {
                        Name = "Man",
                        Percentage = 0
                    },
                    new SubCriteria()
                    {
                        Name = "Vrouw",
                        Percentage = 0
                    }
                }
            },
            new Criteria()
            {
                Name = "Leeftijd",
                SubCriteria = new List<SubCriteria>()
                {
                    new SubCriteria()
                    {
                        Name = "18-25",
                        Percentage = 0
                    },
                    new SubCriteria()
                    {
                        Name = "26-35",
                        Percentage = 0
                    },
                    new SubCriteria()
                    {
                        Name = "36-50",
                        Percentage = 0
                    },
                    new SubCriteria()
                    {
                        Name = "51-60",
                        Percentage = 0
                    },
                    new SubCriteria()
                    {
                        Name = "60+",
                        Percentage = 0
                    }
                }
            }
        };
        
        return criteriaList;
    }
}