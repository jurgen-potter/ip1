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
            Percentage = 10
        };
        var subCrit2 = new SubCriteria()
        {
            Name = "Auto",
            Percentage = 90
        };
        var subCrit3 = new SubCriteria()
        {
            Name = "Hoog opgeleid",
            Percentage = 10
        };
        var subCrit4 = new SubCriteria()
        {
            Name = "Laag opgeleid",
            Percentage = 90
        };
        _panelDbContext.SubCriteria.AddRange(subCrit1, subCrit2, subCrit3, subCrit4);

        var extraCrit1 = new ExtraCriteria()
        {
            Name = "Vervoer",
            SubCriteria = { subCrit1, subCrit2 },
        };
        var extraCrit2 = new ExtraCriteria()
        {
            Name = "Opleiding",
            SubCriteria = { subCrit3, subCrit4 },
        };
        _panelDbContext.ExtraCriteria.AddRange(extraCrit1, extraCrit2);

        // Create panel objects
        var panel1 = new Panel()
        {
            PanelId = 1,
            Name = "Panel 1",
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
            DrawStatus = DrawStatus.FirstPhaseActive
        };
        panel1.ExtraCriteria.Add(extraCrit1);
        panel1.ExtraCriteria.Add(extraCrit2);

        var panel2 = new Panel()
        {
            PanelId = 2,
            Name = "Panel 2",
        };

        // Create initial list with members for Panel 1 and Panel 2
        var members = new List<Member>
        {
            // Panel 1 members
            new Member
            {
                Email = "jan@zonderid.com", Gender = Gender.Male,
                Age = 22, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "els@example.com", Gender = Gender.Female,
                Age = 35, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "bart@example.com", Gender = Gender.Male,
                Age = 50, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "sophie@example.com",
                Gender = Gender.Female, Age = 28, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "tom@example.com", Gender = Gender.Male,
                Age = 45, Town = "Antwerpen", Panel = panel1
            },

            // Panel 2 members
            new Member
            {
                Email = "lisa@example.com", Gender = Gender.Female,
                Age = 33, Town = "Antwerpen", Panel = panel2
            },
            new Member
            {
                Email = "mark@example.com", Gender = Gender.Male,
                Age = 67, Town = "Antwerpen", Panel = panel2
            },
            new Member
            {
                Email = "lotte@example.com", Gender = Gender.Female,
                Age = 72, Town = "Antwerpen", Panel = panel2
            },
            new Member
            {
                Email = "pieter@example.com",
                Gender = Gender.Male, Age = 19, Town = "Antwerpen", Panel = panel2
            },
            new Member
            {
                Email = "emma@example.com", Gender = Gender.Female,
                Age = 21, Town = "Antwerpen", Panel = panel2
            }
        };

        // Add additional members for Panel 1
        members.AddRange(new[]
        {
            new Member
            {
                Email = "jan@example.com", Gender = Gender.Male,
                Age = 22, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "peter@example.com", Gender = Gender.Male,
                Age = 19, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "david@example.com", Gender = Gender.Male,
                Age = 24, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "johan@example.com",
                Gender = Gender.Male, Age = 21, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "koen@example.com", Gender = Gender.Male,
                Age = 25, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "simon@example.com", Gender = Gender.Male,
                Age = 18, Town = "Antwerpen", Panel = panel1
            },
        });

        // Additional groups for Panel 1 (Men 26-40, 41-60, 60+; Women 18-25, 26-40, 41-60, 60+)
        // Men 26-40
        members.AddRange(new[]
        {
            new Member
            {
                Email = "thomas@example.com",
                Gender = Gender.Male, Age = 35, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "maarten@example.com",
                Gender = Gender.Male, Age = 28, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "jeroen@example.com",
                Gender = Gender.Male, Age = 37, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "pieter@example.com",
                Gender = Gender.Male, Age = 30, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "wouter@example.com",
                Gender = Gender.Male, Age = 33, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Email = "michel@example.com",
                Gender = Gender.Male, Age = 39, Town = "Antwerpen", Panel = panel1
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
                    Age = 45, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "marc@example.com",
                    Gender = Gender.Male,
                    Age = 58, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "patrick@example.com",
                    Gender = Gender.Male, Age = 52, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "dirk@example.com",
                    Gender = Gender.Male,
                    Age = 49, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "hans@example.com",
                    Gender = Gender.Male,
                    Age = 44, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "erik@example.com",
                    Gender = Gender.Male,
                    Age = 55, Town = "Antwerpen", Panel = panel1
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
                    Age = 68, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "willem@example.com",
                    Gender = Gender.Male, Age = 71, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "gerard@example.com",
                    Gender = Gender.Male, Age = 65, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "robert@example.com",
                    Gender = Gender.Male, Age = 77, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "hugo@example.com",
                    Gender = Gender.Male,
                    Age = 69, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "albert@example.com",
                    Gender = Gender.Male, Age = 73, Town = "Antwerpen", Panel = panel1
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
                    Age = 22, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "lisa@example.com",
                    Gender = Gender.Female,
                    Age = 19, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "emma@example.com",
                    Gender = Gender.Female,
                    Age = 24, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "sara@example.com",
                    Gender = Gender.Female,
                    Age = 21, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "laura@example.com",
                    Gender = Gender.Female, Age = 25, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "nina@example.com",
                    Gender = Gender.Female,
                    Age = 18, Town = "Antwerpen", Panel = panel1
                },
            });

        // Women 26-40
        members
            .AddRange(new[]
            {
                new Member
                {
                    Email = "eva@example.com", Gender = Gender.Female,
                    Age = 35, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "sophie@example.com",
                    Gender = Gender.Female, Age = 28, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "julie@example.com",
                    Gender = Gender.Female, Age = 37, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "els@example.com", Gender = Gender.Female,
                    Age = 30, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "lieve@example.com",
                    Gender = Gender.Female, Age = 33, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "anja@example.com",
                    Gender = Gender.Female,
                    Age = 39, Town = "Antwerpen", Panel = panel1
                },
            });

        // Women 41-60
        members
            .AddRange(new[]
            {
                new Member
                {
                    Email = "maria@example.com",
                    Gender = Gender.Female, Age = 45, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "ann@example.com", Gender = Gender.Female,
                    Age = 58, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "ingrid@example.com",
                    Gender = Gender.Female, Age = 52, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "martine@example.com",
                    Gender = Gender.Female, Age = 49, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "hilde@example.com",
                    Gender = Gender.Female, Age = 44, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
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
                    Email = "helena@example.com",
                    Gender = Gender.Female, Age = 68, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "godelieve@example.com",
                    Gender = Gender.Female, Age = 71, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "rosa@example.com",
                    Gender = Gender.Female,
                    Age = 65, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "margareta@example.com",
                    Gender = Gender.Female, Age = 77, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Email = "mariette@example.com",
                    Gender = Gender.Female, Age = 69, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
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
            Age = 30,
            Code = "blLy-Tvxl-1TXG-nYg3-CBtD",
            Gender = Gender.Female,
            PanelId = 1,
            Postcode = "2140",
            QRCodeString =
                "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"
        };

        var invitation2 = new Invitation()
        {
            Age = 20,
            Code = "tDK6-KFhD-ipIi-tQjz-m6cZ",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2100",
            QRCodeString =
                "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"
        };

        var invitation3 = new Invitation()
        {
            Age = 40,
            Code = "Iga7-FHyw-yVfo-8wvN-Bxa8",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2110",
            QRCodeString =
                "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"
        };

        var invitation4 = new Invitation()
        {
            Age = 40,
            Code = "FIcV-zyXs-reXO-I6dh-rMCB",
            Gender = Gender.Female,
            PanelId = 2,
            Postcode = "2105",
            QRCodeString =
                "iVBORw0KGgoAAAANSUhEUgAABHQAAAR0AQAAAAA4d3wbAAAHuklEQVR4nO3dXW7cMAxF4dlB97/L7iBtB3EkkZRnAhSwGXx6KJr4RxyCvPeMISuPj1uN34+rI1iHeORH/egv+kMP+QU/xRvXDjwmP+pHf9Efesgv+CneuHbgMflRP/qL/tBDfsFP8ca1A4/Jj/rRX/SHHvILfoo3rh14TH7Uj/6iP/fXw0ccv+Yzn7/5d8FxxfN/4cfneX8vG9d+/i8c3U4kHvlRP/qL/tBDfsFP8QYe68OHIYDjxuOq8Lt07QjviHGcF+5Szyse+VE/+ov+0EN+wU/xBh5rxYf1Vc8bD0QfEL7wfB33OO+4y+lE4pEf9aO/6A895Bf8FG/gsb58OJ6Cp+fcy6joPDz7rh+Ki0d+1I/+oj/0kF/wU7yBx34kHwaUH1HMYWVYDw/FxSM/6kd/0R96yC/4Kd7AYz+KD7c/fl1whLc8AB+RhWffgeJfTSQe+VE/+ov+0EN+wU/xBh7rw4dh5IUh/+ufPJF45Ef96C/6Qw/5BT/FG3isGR9WIzzYTq8ohjUmywPw9MQ7vO1YDfHIj/rRX/SHHvILfoo38FgfPszPubc4Pq/RrsY2iuWy4qh45Ef96C/6Qw/5BT/FG3isFR8ukT1/qC9dDqQYB9lvGH++gXjkR/3oL/pDD/kFP8UbeKwfH84kHnh+O2263RRA2Iav3lTv9H1G8ciP+tFf9Ice8gt+ijfw2J35ML/AWL96uPk7LeOU6nfj5OpdSPHIj/rRX/SHHvILfoo38FhHPgzzzEerO+Uft6tS0kP2HLd45Ef96C/6Qw/5BT/FG3isEx8eY2XsZdoqxnD3EcX2E1ShiEd+1I/+oj/0kF/wU7yBx1rxYc3aY8ZlnhRZWF4SFmCPK5ZV3Sko8ciP+tFf9Ice8gt+ijfwWEc+XEg8vOhYBVo97J6Pjt9VtxKP/Kgf/UV/6CG/4Kd4A4915MPjquK0csVIje3hBuEThM+8BC8e+VE/+ov+0EN+wU/xBh7rw4f1+4dVFB81mI956h3zwqPw/PakeORH/egv+kMP+QU/xRt4rA8fzucuo2Lyetpqs7yF8audqU+/X4hHftSP/qI/9JBf8FO8gcduy4fVo+vtxh4Vu+d9pj9vNW6ftwcRj/yoH/1Ff+ghv+CneAOPteXDEFlalL05JcyYvhos2F6vVBGP/Kgf/UV/6CG/4Kd4A4915MPwYuKC48X1eQeR/Ox7juzx3f30xCM/6kd/0R96yC/4Kd7AYzflw3DjcEG1WOTs5LTuZLOU5O33K8UjP+pHf9Efesgv+CnewGM34sP5qmWddbhdPdlg3XDt84owx3KyeORH/egv+kMP+QU/xRt4rCMfpqfb+e7z//JC7TTZOFpR/BKoeORH/egv+kMP+QU/xRt4rB0ffpzc+Hx1yPZjVOHNN3hz/YZ45Ef96C/6Qw/5BT/FG3jsbnx4PtnXBY+3Ts5/HnE+L4C+eORH/egv+kMP+QU/xRt4rB8fvrUpyDhQrcceJ4elKS/XoohHftSP/qI/9JBf8FO8gcd68uGTv5dz5x/zau36BtValGql92ueF4/8qB/9RX/oIb/gp3gDj92SD8eM1d55KagccvUXW+abbr8ujCEe+VE/+ov+0EN+wU/xBh7rwodhhNUhxaXT3PPRzWeZr6iCF4/8qB/9RX/oIb/gp3gDj7Xiw3TPEVReWJ2COkZ17Tzy8hLxyI/60V/0hx7yC36KN/BYTz4Mm3hk6p6nraJdRrjVt/fTE4/8qB/9RX/oIb/gp3gDj92bD/Od0i56IbJlK73E7tXI54lHftSP/qI/9JBf8FO8gcc68mFYCTJf8N7j8XFy2ihkWZ4dDuzXt4hHftSP/qI/9JBf8FO8gcfuy4fpWXWg7vxj+C7w6tXIJe55NvHIj/rRX/SHHvILfoo38FhHPgzjbMfp+WtAQPnM6fUCkuVPvohHftSP/qI/9JBf8FO8gcc68mG19V0dyuYZeb3Px3JedVQ88qN+9Bf9oYf8gp/iDTzWjA/rex4QfvK24wb0w7XzFWGnvtP1z+KRH/Wjv+gPPeQX/BRv4LH78uH81Lp6lTEcXf52S4girCeZ56722BOP/Kgf/UV/6CG/4Kd4A4+14sNwMPB3tQvIdrPpemVJ3i1kToZ45Ef96C/6Qw/5BT/FG3isFR9WI01RbUAdNtVbAp3Pyx83BSUe+VE/+ov+0EN+wU/xBh7rwoeJtvM66/QSYqD9gPxhA73Ned/ZD0Q88qN+9Bf9oYf8gp/iDTx2Hz7M7x++2hhvs+3H5ylhfcoyR0J58ciP+tFf9Ice8gt+ijfwWD8+TOid8X5MNm5cXTYf/fX664J45Ef96C/6Qw/5BT/FG3jsp/BhmCfvRx0elG8XYIclJ6f7gYhHftSP/qI/9JBf8FO8gce68WGa54infvYdVmuHT3D2DUA88qN+9Bf9oYf8gp/iDTzWkQ9TePmheAD06oo5+AX5T560i0d+1I/+oj/0kF/wU7yBx5rxYRgvw5s/RngUvsSzfb0x5UE88qN+9Bf9oYf8gp/iDTzWhQ/vMMQjP+pHf9Efesgv+CneuHbgMflRP/qL/tBDfsFP8ca1A4/Jj/rRX/SHHvILfoo3rh14TH7Uj/6iP/SQX/BTvHHtwGPyo370F/2hh2/5xR+dCDhlydzRowAAAABJRU5ErkJggg=="
        };

        var invitation5 = new Invitation()
        {
            Age = 25,
            Code = "HEMf-Xu0L-1ETh-urZJ-26s9",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "Antwerpen",
            QRCodeString =
                "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",
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