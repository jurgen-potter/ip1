using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL;
using QRCoder;

namespace CitizenPanel.BL;

public class DrawManager : IDrawManager
{
    private readonly IDrawRepository _drawRepository;

    public DrawManager(IDrawRepository drawRepository)
    {
        _drawRepository = drawRepository;
    }

    public IEnumerable<Invitation> AddInvitations(List<DummyMember> members)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        List<Invitation> invitations = new List<Invitation>();
        foreach (DummyMember dummyMember in members)
        {
            int age = dummyMember.Age;
            Gender gender = dummyMember.Gender;
            int panelId = dummyMember.PanelId;
            string town = dummyMember.Town;
            string code = GenerateCode();

            string qrCodePlace = "https://whimp-24.ew.r.appspot.com/MemberRegister/RegisterMember?code=" + code; 
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodePlace, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
            string qrCodeString = Convert.ToBase64String(qrCodeAsPngByteArr);

            
            Invitation invitation = new Invitation()
            {
                Code = code,
                Age = age,
                Gender = gender,
                PanelId = panelId,
                QRCodeString = qrCodeString,
            };
            Invitation newInvitation = _drawRepository.CreateInvitation(invitation);
            invitations.Add(newInvitation);
        }
        
        return invitations;
    }

    public IEnumerable<Invitation> AddInvitations(Panel panel, List<DummyMember> members)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        List<Invitation> invitations = new List<Invitation>();

        foreach (Criteria criteria in panel.Criteria)
        {
            foreach (SubCriteria subCriteria in criteria.SubCriteria)
            {
                
            }
        }
        
        foreach (DummyMember dummyMember in members)
        {
            int age = dummyMember.Age;
            Gender gender = dummyMember.Gender;
            int panelId = dummyMember.PanelId;
            string town = dummyMember.Town;
            string code = GenerateCode();

            string qrCodePlace = "https://whimp-24.ew.r.appspot.com/MemberRegister/RegisterMember?code=" + code;
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodePlace, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
            string qrCodeString = Convert.ToBase64String(qrCodeAsPngByteArr);


            Invitation invitation = new Invitation()
            {
                Code = code,
                Age = age,
                Gender = gender,
                PanelId = panelId,
                QRCodeString = qrCodeString,
            };
            Invitation newInvitation = _drawRepository.CreateInvitation(invitation);
            invitations.Add(newInvitation);
        }

        return invitations;
    }

    public Invitation GetInvitationWithCode(string code)
    {
        return _drawRepository.ReadInvitationWithCode(code);
    }

    public IEnumerable<Invitation> GetAllInvitations()
    {
        return _drawRepository.ReadAllInvitations();
    }

    public Invitation ChangeInvitation(Invitation invitation)
    {
        return _drawRepository.UpdateInvitation(invitation);
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
        return _drawRepository.ReadCriteria(criteriaId);
    }

    public IEnumerable<Criteria> GetAllCriteria()
    {
        return _drawRepository.ReadAllCriteria();
    }

    public SubCriteria GetSubCriteria(int subCriteriaId)
    {
        return _drawRepository.ReadSubCriteria(subCriteriaId);
    }

    public IEnumerable<Criteria> GetCriteriaByPanel(int panelId)
    {
        return _drawRepository.ReadCriteriaByPanel(panelId);
    }
    
    public Criteria AddCriteria(string name, List<SubCriteria> subCriteria)
    {
        Criteria criteria = new Criteria()
        {
            Name = name,
            SubCriteria = subCriteria
        };
        return _drawRepository.CreateCriteria(criteria);
    }

    public SubCriteria AddSubCriteria(string name, double percentage)
    {
        SubCriteria subCriteria = new SubCriteria()
        {
            Name = name,
            Percentage = percentage
        };
        return _drawRepository.CreateSubCriteria(subCriteria);
    }
    
    public void EditCriteria(int panelId, IEnumerable<Criteria> criteria)
    {
        _drawRepository.UpdateCriteria(panelId, criteria);
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
        return _drawRepository.DeleteInvitation(invitationId);
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