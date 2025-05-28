using System.ComponentModel;
using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using QRCoder;
using ClosedXML.Excel;
using System.Drawing;
using System.IO;

namespace CitizenPanel.BL.Utilities;

public class UtilityManager(IDrawManager drawManager) : IUtilityManager
{
    public IEnumerable<Invitation> GenerateInvitations(int amount, List<Criteria> criteria, Panel panel, int batch)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        List<Invitation> invitations = new List<Invitation>();

        Dictionary<Gender, double> genderPercentages = new();
        Dictionary<string, double> ageGroupPercentages = new();

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
                    "60+" => 61,
                    _ => 36
                };

                string code = GenerateCode();
                string qrCodePlace = $"https://panello.xyz/MemberRegister/RegisterMember?code={code}";

                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodePlace, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);

                Invitation newInvitation = drawManager.AddInvitation(code, qrCodeAsPngByteArr, panel.Id, gender, age, batch);
                invitations.Add(newInvitation);
            }
        }

        return invitations;
    }

    private static string GenerateCode()
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

    public int CalculateMembers(int totalAvailableMembers)
    {
        return (int)Math.Round(0.50 * Math.Sqrt(totalAvailableMembers));
    }

    public RecruitmentResult CalculateRecruitment(int totalToDraw, IEnumerable<Criteria> criteriaList)
    {
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
            TotalNeededInvitations = reservePool,
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
                int count = (int)Math.Ceiling((double)(totalToDraw * accumulatedPct));
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

    public byte[] GenerateExcelWithQrCodes(IEnumerable<Invitation> invitations)
    {
        using var workbook = new XLWorkbook();
        List<IXLWorksheet> worksheets = new List<IXLWorksheet>
        {
            workbook.Worksheets.Add("Uitnodigingen Mannen"),
            workbook.Worksheets.Add("Uitnodigingen Vrouwen")
        };

        const int imagePixelHeight = 80;
        const double excelRowHeight = imagePixelHeight / 0.75;
        List<int> rows = new List<int> { 2, 2 };

        foreach (var worksheet in worksheets)
        {
            worksheet.Cell(1, 1).Value = "Code";
            worksheet.Cell(1, 2).Value = "Geslacht";
            worksheet.Cell(1, 3).Value = "LeeftijdsCategorie";
            worksheet.Cell(1, 4).Value = "QR-Code";
        }
        

        foreach (var invitation in invitations.Where(i => i.IsRegistered == false))
        {
            var worksheet = worksheets[0];
            var currRow = rows[0];
            if (invitation.Gender == Gender.Female)
            {
                worksheet = worksheets[1];
                currRow = rows[1];
            }
            
            worksheet.Row(currRow).Height = excelRowHeight;
            worksheet.Cell(currRow, 1).Value = invitation.Code;
            worksheet.Cell(currRow, 2).Value = invitation.Gender.ToString();
            string leeftijdsCategorie;
            switch (invitation.Age)
            {
                case 18: leeftijdsCategorie = "18-25"; break;
                case 26: leeftijdsCategorie = "26-35"; break;
                case 36: leeftijdsCategorie = "36-50"; break;
                case 51 : leeftijdsCategorie = "51-60"; break;
                default: leeftijdsCategorie = "60+"; break;
            }
            worksheet.Cell(currRow, 3).Value = leeftijdsCategorie;

            if (invitation.QRCode is { Length: > 0 })
            {
                using var qrStream = new MemoryStream(invitation.QRCode);

                worksheet.AddPicture(qrStream, $"QR_{invitation.Code}")
                    .MoveTo(worksheet.Cell(currRow, 4))
                    .WithSize(80, 80);
            }

            if (invitation.Gender == Gender.Female)
            {
                rows[1]++;
            }
            else
            {
                rows[0]++;
            }
        }

        using var output = new MemoryStream();
        workbook.SaveAs(output);
        return output.ToArray();
    }
}