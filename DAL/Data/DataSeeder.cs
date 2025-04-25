using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL.Data;

public class DataSeeder
{
    private readonly PanelDbContext _panelDbContext;

    public DataSeeder(PanelDbContext panelDbContext)
    {
        _panelDbContext = panelDbContext;
    }

    public void Seed()
    {
        SeedPanels();
        _panelDbContext.SaveChanges();
        _panelDbContext.ChangeTracker.Clear();

        SeedInvitations();
        SeedQuestionnaires();

        _panelDbContext.SaveChanges();
        _panelDbContext.ChangeTracker.Clear();
    }

    private void SeedPanels()
    {
        //criteria
        var subCrit1 = new SubCriteria()
        {
            Name = "Fiets",
            Percentage = 10,
            TenantId = "antwerpen"
        };
        var subCrit2 = new SubCriteria()
        {
            Name = "Auto",
            Percentage = 90,
            TenantId = "antwerpen"
        };
        var subCrit3 = new SubCriteria()
        {
            Name = "Hoog opgeleid",
            Percentage = 10,
            TenantId = "antwerpen"
        };
        var subCrit4 = new SubCriteria()
        {
            Name = "Laag opgeleid",
            Percentage = 90,
            TenantId = "antwerpen"
        };
        _panelDbContext.SubCriteria.AddRange(subCrit1, subCrit2, subCrit3, subCrit4);

        var crit1 = new Criteria()
        {
            Name = "Vervoer",
            SubCriteria = { subCrit1, subCrit2 },
            TenantId = "antwerpen"
        };
        var crit2 = new Criteria()
        {
            Name = "Opleiding",
            SubCriteria = { subCrit3, subCrit4 },
            TenantId = "antwerpen"
        };
        _panelDbContext.Criteria.AddRange(crit1, crit2);

        // Create panel objects
        var panel1 = new Panel()
        {
            Name = "Panel Antwerpen",
            Description = "Dit is de omschrijving van het panel.",
            StartDate = new DateOnly(2025, 1, 12),
            EndDate = new DateOnly(2025, 7, 22),
            RecruitmentBuckets = new List<RecruitmentBucket>()
            {
                new RecruitmentBucket { Gender = "Mannen", AgeGroup = "18-25", Count = 0, Target = 5 },
                new RecruitmentBucket { Gender = "Mannen", AgeGroup = "26-40", Count = 0, Target = 5 },
                new RecruitmentBucket { Gender = "Mannen", AgeGroup = "41-60", Count = 0, Target = 5 },
                new RecruitmentBucket { Gender = "Mannen", AgeGroup = "60+", Count = 0, Target = 5 },
                new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "18-25", Count = 0, Target = 5 },
                new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "26-40", Count = 0, Target = 5 },
                new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "41-60", Count = 0, Target = 5 },
                new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "60+", Count = 0, Target = 5 }
            },
            DrawStatus = DrawStatus.FirstPhaseActive,
            Recommendations = new List<Recommendation>()
            {
                new Recommendation()
                {
                    Title = "Meer fietspaden",
                    Description = "Gemeente Antwerpen moet meer fietspaden aanleggen",
                    Votes = 0,
                    TenantId = "antwerpen"
                },
                new Recommendation()
                {
                    Title = "Autovrije binnenstad",
                    Description = "Gemeente Antwerpen moet auto's uit de binnenstad verbieden",
                    Votes = 0,
                    TenantId = "antwerpen"
                }
            },
            TenantId = "antwerpen"
        };
        _panelDbContext.Panels.Add(panel1);
        panel1.Criteria.Add(crit1);
        panel1.Criteria.Add(crit2);

        var panel2 = new Panel()
        {
            Name = "Panel Brussel",
            Description = "Dit is ook een omschrijving van een panel.",
            StartDate = new DateOnly(2025, 3, 1),
            EndDate = new DateOnly(2025, 8, 14),
            TenantId = "antwerpen"
        };
        _panelDbContext.Panels.Add(panel2);

        // Create initial list with members for Panel 1 and Panel 2
        var members = new List<Member>
        {
            // Panel 1 members
            new Member
            {
                Email = "jan@zonderid.com", Gender = Gender.Male,
                Age = 22, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "els@example.com", Gender = Gender.Female,
                Age = 35, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "bart@example.com", Gender = Gender.Male,
                Age = 50, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "sophie@example.com",
                Gender = Gender.Female, Age = 28, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "tom@example.com", Gender = Gender.Male,
                Age = 45, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },

            // Panel 2 members
            new Member
            {
                Email = "lisa@example.com", Gender = Gender.Female,
                Age = 33, Town = "Antwerpen", Panel = panel2,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "mark@example.com", Gender = Gender.Male,
                Age = 67, Town = "Antwerpen", Panel = panel2,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "lotte@example.com", Gender = Gender.Female,
                Age = 72, Town = "Antwerpen", Panel = panel2,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "pieter@example.com",
                Gender = Gender.Male, Age = 19, Town = "Antwerpen", Panel = panel2,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "emma@example.com", Gender = Gender.Female,
                Age = 21, Town = "Antwerpen", Panel = panel2,
                TenantId = "antwerpen"
            }
        };

        // Add additional members for Panel 1
        members.AddRange(new[]
        {
            new Member
            {
                Email = "jan@example.com", Gender = Gender.Male,
                Age = 22, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "peter@example.com", Gender = Gender.Male,
                Age = 19, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "david@example.com", Gender = Gender.Male,
                Age = 24, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "johan@example.com",
                Gender = Gender.Male, Age = 21, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "koen@example.com", Gender = Gender.Male,
                Age = 25, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "simon@example.com", Gender = Gender.Male,
                Age = 18, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
        });

        // Additional groups for Panel 1 (Men 26-40, 41-60, 60+; Women 18-25, 26-40, 41-60, 60+)
        // Men 26-40
        members.AddRange(new[]
        {
            new Member
            {
                Email = "thomas@example.com",
                Gender = Gender.Male, Age = 35, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "maarten@example.com",
                Gender = Gender.Male, Age = 28, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "jeroen@example.com",
                Gender = Gender.Male, Age = 37, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "pieter@example.com",
                Gender = Gender.Male, Age = 30, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "wouter@example.com",
                Gender = Gender.Male, Age = 33, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
            new Member
            {
                Email = "michel@example.com",
                Gender = Gender.Male, Age = 39, Town = "Antwerpen", Panel = panel1,
                TenantId = "antwerpen"
            },
        });
        // Men 41-60
        members
            .AddRange(new[]
            {
                new Member
                {
                    Email = "frank@example.com",
                    Gender = Gender.Male,
                    Age = 45, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "marc@example.com",
                    Gender = Gender.Male,
                    Age = 58, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "patrick@example.com",
                    Gender = Gender.Male, Age = 52, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "dirk@example.com",
                    Gender = Gender.Male,
                    Age = 49, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "hans@example.com",
                    Gender = Gender.Male,
                    Age = 44, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "erik@example.com",
                    Gender = Gender.Male,
                    Age = 55, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
            });

        // Men 60+
        members
            .AddRange(new[]
            {
                new Member
                {
                    Email = "jozef@example.com",
                    Gender = Gender.Male,
                    Age = 68, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "willem@example.com",
                    Gender = Gender.Male, Age = 71, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "gerard@example.com",
                    Gender = Gender.Male, Age = 65, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "robert@example.com",
                    Gender = Gender.Male, Age = 77, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "hugo@example.com",
                    Gender = Gender.Male,
                    Age = 69, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "albert@example.com",
                    Gender = Gender.Male, Age = 73, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
            });

        // Women 18-25
        members
            .AddRange(new[]
            {
                new Member
                {
                    Email = "anna@example.com",
                    Gender = Gender.Female,
                    Age = 22, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "lisa@example.com",
                    Gender = Gender.Female,
                    Age = 19, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "emma@example.com",
                    Gender = Gender.Female,
                    Age = 24, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "sara@example.com",
                    Gender = Gender.Female,
                    Age = 21, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "laura@example.com",
                    Gender = Gender.Female, Age = 25, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "nina@example.com",
                    Gender = Gender.Female,
                    Age = 18, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
            });

        // Women 26-40
        members
            .AddRange(new[]
            {
                new Member
                {
                    Email = "eva@example.com", Gender = Gender.Female,
                    Age = 35, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "sophie@example.com",
                    Gender = Gender.Female, Age = 28, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "julie@example.com",
                    Gender = Gender.Female, Age = 37, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "els@example.com", Gender = Gender.Female,
                    Age = 30, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "lieve@example.com",
                    Gender = Gender.Female, Age = 33, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "anja@example.com",
                    Gender = Gender.Female,
                    Age = 39, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
            });

        // Women 41-60
        members
            .AddRange(new[]
            {
                new Member
                {
                    Email = "maria@example.com",
                    Gender = Gender.Female, Age = 45, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "ann@example.com", Gender = Gender.Female,
                    Age = 58, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    Email = "ingrid@example.com",
                    Gender = Gender.Female, Age = 52, Town = "Antwerpen", Panel = panel1,
                    TenantId = "antwerpen"
                },
                new Member
                {
                    TenantId = "antwerpen",
                    Email = "martine@example.com",
                    Gender = Gender.Female, Age = 49, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    TenantId = "antwerpen",
                    Email = "hilde@example.com",
                    Gender = Gender.Female, Age = 44, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    TenantId = "antwerpen",
                    Email = "sonja@example.com",
                    Gender = Gender.Female, Age = 55, Town = "Antwerpen", Panel = panel1
                },
            });

        // Women 60+
        members
            .AddRange(new[]
            {
                new Member
                {
                    TenantId = "antwerpen",
                    Email = "helena@example.com",
                    Gender = Gender.Female, Age = 68, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    TenantId = "antwerpen",
                    Email = "godelieve@example.com",
                    Gender = Gender.Female, Age = 71, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    TenantId = "antwerpen",
                    Email = "rosa@example.com",
                    Gender = Gender.Female,
                    Age = 65, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    TenantId = "antwerpen",
                    Email = "margareta@example.com",
                    Gender = Gender.Female, Age = 77, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    TenantId = "antwerpen",
                    Email = "mariette@example.com",
                    Gender = Gender.Female, Age = 69, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    TenantId = "antwerpen",
                    Email = "alice@example.com",
                    Gender = Gender.Female, Age = 73, Town = "Antwerpen", Panel = panel1
                },
            });

        _panelDbContext.AddRange(members);
    }

    private void SeedInvitations()
    {
        //invitations
        var invitation1 = new Invitation()
        {
            TenantId = "antwerpen",
            Age = 30,
            Code = "blLy-Tvxl-1TXG-nYg3-CBtD",
            Gender = Gender.Female,
            PanelId = 1,
            Postcode = "2140",
            QRCodeString =
                ""
        };

        var invitation2 = new Invitation()
        {
            TenantId = "antwerpen",
            Age = 20,
            Code = "tDK6-KFhD-ipIi-tQjz-m6cZ",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2100",
            QRCodeString =
                ""
        };

        var invitation3 = new Invitation()
        {
            TenantId = "antwerpen",
            Age = 40,
            Code = "Iga7-FHyw-yVfo-8wvN-Bxa8",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2110",
            QRCodeString =
                ""
        };

        var invitation4 = new Invitation()
        {
            TenantId = "antwerpen",
            Age = 40,
            Code = "FIcV-zyXs-reXO-I6dh-rMCB",
            Gender = Gender.Female,
            PanelId = 2,
            Postcode = "2105",
            QRCodeString =
                ""
        };

        var invitation5 = new Invitation()
        {
            TenantId = "antwerpen",
            Age = 25,
            Code = "HEMf-Xu0L-1ETh-urZJ-26s9",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "Antwerpen",
            QRCodeString =
                "",
            Email = "drawn@example.com",
            IsRegistered = true,
            IsDrawn = true,
            SelectedCriteria = new List<int>([1, 4]),
        };
        _panelDbContext.Invitations.Add(invitation1);
        _panelDbContext.Invitations.Add(invitation2);
        _panelDbContext.Invitations.Add(invitation3);
        _panelDbContext.Invitations.Add(invitation4);
        _panelDbContext.Invitations.Add(invitation5);
    }

    private void SeedQuestionnaires()
    {
        var questionnaire = new Questionnaire
        {
            Title = "Verkenningsmodule"
        };
        var question1 = new Question
        {
            Description = "Beschik je als organisator nog over minstens 6 maanden voordat de input van de participatie klaar moet zijn voor de politieke besluitvorming?",
            Weight = 5,
            Position = 1
        };
        var answer1 = new Answer
        {
            Description = "Ja",
            Position = 1
        };
        var answer2 = new Answer
        {
            Description = "Nee",
            Position = 2
        };
        var question2 = new Question
        {
            Description = "Is de gemeente bereid om de realisatie van de voorstellen van het burgerpanel ernstig te overwegen en minstens publiek te motiveren waarom dat niet is gebeurd?",
            Weight = 3,
            Position = 2
        };
        var answer3 = new Answer
        {
            Description = "Ja",
            Position = 1
        };
        var answer4 = new Answer
        {
            Description = "Nee",
            Position = 2
        };
        answer1.Question = question1;
        answer2.Question = question1;
        question1.Answers.Add(answer1);
        question1.Answers.Add(answer2);
        question1.Questionnaire = questionnaire;
        questionnaire.Questions.Add(question1);

        answer3.Question = question2;
        answer4.Question = question2;
        question2.Answers.Add(answer3);
        question2.Answers.Add(answer4);
        question2.Questionnaire = questionnaire;
        questionnaire.Questions.Add(question2);

        _panelDbContext.Questionnaires.Add(questionnaire);
        _panelDbContext.Questions.AddRange(question1, question2);
        _panelDbContext.Answers.AddRange(answer1, answer2, answer3, answer4);
    }
}