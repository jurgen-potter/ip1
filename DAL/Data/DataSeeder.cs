using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.Domain.User;
using Microsoft.EntityFrameworkCore;

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
            Name = "Man",
            Percentage = 40,
            TenantId = "antwerpen"
        };
        var subCrit2 = new SubCriteria()
        {
            Name = "Vrouw",
            Percentage = 60,
            TenantId = "antwerpen"
        };
        var subCrit3 = new SubCriteria()
        {
            Name = "18-25",
            Percentage = 20,
            TenantId = "antwerpen"
        };
        var subCrit4 = new SubCriteria()
        {
            Name = "26-35",
            Percentage = 50,
            TenantId = "antwerpen"
        };
        var subCrit5 = new SubCriteria()
        {
            Name = "36-50",
            Percentage = 10,
            TenantId = "antwerpen"
        };
        var subCrit6 = new SubCriteria()
        {
            Name = "51-60",
            Percentage = 10,
            TenantId = "antwerpen"
        };
        var subCrit7 = new SubCriteria()
        {
            Name = "60+",
            Percentage = 10,
            TenantId = "antwerpen"
        };
        _panelDbContext.SubCriteria.AddRange(subCrit1, subCrit2, subCrit3, subCrit4);

        var crit1 = new Criteria()
        {
            Name = "Geslacht",
            SubCriteria = { subCrit1, subCrit2 },
            TenantId = "antwerpen"
        };
        var crit2 = new Criteria()
        {
            Name = "Leeftijd",
            SubCriteria = { subCrit3, subCrit4 , subCrit5 , subCrit6 , subCrit7},
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
                // new RecruitmentBucket { Gender = "Mannen", AgeGroup = "18-25", Count = 0, Target = 5 },
                // new RecruitmentBucket { Gender = "Mannen", AgeGroup = "26-40", Count = 0, Target = 5 },
                // new RecruitmentBucket { Gender = "Mannen", AgeGroup = "41-60", Count = 0, Target = 5 },
                // new RecruitmentBucket { Gender = "Mannen", AgeGroup = "60+", Count = 0, Target = 5 },
                // new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "18-25", Count = 0, Target = 5 },
                // new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "26-40", Count = 0, Target = 5 },
                // new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "41-60", Count = 0, Target = 5 },
                // new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "60+", Count = 0, Target = 5 }
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
            TenantId = "brussel"
        };
        _panelDbContext.Panels.Add(panel2);
        
        var panel3 = new Panel()
        {
            Name = "Panel Antwerpen 2",
            Description = "Dit is ook nog een omschrijving van een panel",
            StartDate = new DateOnly(2025, 3, 11),
            EndDate = new DateOnly(2025, 7, 17),
            TenantId = "antwerpen"
        };
        _panelDbContext.Panels.Add(panel3);
        
        var antwerpen = _panelDbContext.ApplicationUsers
            .Include(u => u.OrganizationProfile)
            .SingleOrDefault(u => u.UserName == "antwerpen@example.com");
        var brussel = _panelDbContext.ApplicationUsers
            .Include(u => u.OrganizationProfile)
            .SingleOrDefault(u => u.UserName == "brussel@example.com");
        var paul = _panelDbContext.ApplicationUsers
            .Include(u => u.MemberProfile)
            .SingleOrDefault(u => u.UserName == "paul@example.com");
        panel1.Organization = antwerpen?.OrganizationProfile;
        panel1.Members.Add(paul?.MemberProfile);
        panel2.Organization = brussel?.OrganizationProfile;
        panel3.Organization = antwerpen?.OrganizationProfile;
        //panel3.Members.Add(paul?.MemberProfile);
        antwerpen?.OrganizationProfile.Panels.Add(panel1);
        antwerpen?.OrganizationProfile.Panels.Add(panel3);
        brussel?.OrganizationProfile.Panels.Add(panel2);
        paul?.MemberProfile.Panels.Add(panel1);
        //paul?.MemberProfile.Panels.Add(panel3);
        
        // Create initial list with members for Panel 1 and Panel 2
        var members = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Email = "jan@zonderid.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 22,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            // Panel 1 members
            new ApplicationUser
            {
                Email = "els@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 35,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "bart@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 50,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "sophie@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 28,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "tom@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 45,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },

            // Panel 2 members
            new ApplicationUser
            {
                Email = "lisa@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 33,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "mark@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 67,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel2},
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "lotte@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 72,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel2 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "pieter@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 19,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel2 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "emma@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 21,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel2 },
                    TenantId = "antwerpen"
                }
            },

            // Additional members for Panel 1
            new ApplicationUser
            {
                Email = "jan@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 22,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "peter@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 19,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "david@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 24,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "johan@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 21,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "koen@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 25,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "simon@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 18,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },

            // Men 26-40
            new ApplicationUser
            {
                Email = "thomas@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 35,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "maarten@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 28,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "jeroen@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 37,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "pieter@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 30,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "wouter@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 33,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "michel@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 39,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },

            // Men 41-60
            new ApplicationUser
            {
                Email = "frank@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 45,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "marc@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 58,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "patrick@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 52,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "dirk@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 49,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "hans@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 44,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "erik@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 55,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            
            // Men 60+
            new ApplicationUser
            {
                Email = "jozef@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 68,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "willem@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 71,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "gerard@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 65,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "robert@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 77,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "hugo@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 69,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "albert@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Male,
                    Age = 73,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },

            // Women 18-25
            new ApplicationUser
            {
                Email = "anna@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 22,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "lisa@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 19,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "emma@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 24,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "sara@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 21,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "laura@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 25,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "nina@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 18,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },

            // Women 26-40
            new ApplicationUser
            {
                Email = "eva@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 35,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "sophie@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 28,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "julie@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 37,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "els@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 30,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "lieve@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 33,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "anja@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 39,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },

            // Women 41-60
            new ApplicationUser
            {
                Email = "maria@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 45,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "ann@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 58,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "ingrid@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 52,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "martine@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 49,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "hilde@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 44,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "sonja@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 55,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },

            // Women 60+
            new ApplicationUser
            {
                Email = "helena@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 68,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "godelieve@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 71,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "rosa@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 65,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "margareta@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 77,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "mariette@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 69,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            },
            new ApplicationUser
            {
                Email = "alice@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    Gender = Gender.Female,
                    Age = 73,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen"
                }
            }
        };

        _panelDbContext.AddRange(members);
    }

    private void SeedInvitations()
    {
        //invitations
        var invitation1 = new Invitation()
        {
            //TenantId = "antwerpen",
            Age = 30,
            Code = "blLy-Tvxl-1TXG-nYg3-CBtD",
            Gender = Gender.Female,
            PanelId = 1,
            Town = "2140",
            QRCodeString =
                ""
        };

        var invitation2 = new Invitation()
        {
            //TenantId = "antwerpen",
            Age = 20,
            Code = "tDK6-KFhD-ipIi-tQjz-m6cZ",
            Gender = Gender.Male,
            PanelId = 1,
            Town = "2100",
            QRCodeString =
                ""
        };

        var invitation3 = new Invitation()
        {
            //TenantId = "antwerpen",
            Age = 40,
            Code = "Iga7-FHyw-yVfo-8wvN-Bxa8",
            Gender = Gender.Male,
            PanelId = 1,
            Town = "2110",
            QRCodeString =
                ""
        };

        var invitation4 = new Invitation()
        {
            //TenantId = "antwerpen",
            Age = 40,
            Code = "FIcV-zyXs-reXO-I6dh-rMCB",
            Gender = Gender.Female,
            PanelId = 2,
            Town = "2105",
            QRCodeString =
                ""
        };

        var invitation5 = new Invitation()
        {
            //TenantId = "antwerpen",
            Age = 25,
            Code = "HEMf-Xu0L-1ETh-urZJ-26s9",
            Gender = Gender.Male,
            PanelId = 1,
            Town = "Antwerpen",
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