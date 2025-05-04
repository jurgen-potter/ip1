using System.Runtime.InteropServices.JavaScript;
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
        var subCrit8 = new SubCriteria()
        {
            Name = "Fiets",
            Percentage = 10,
            TenantId = "antwerpen"
        };
        var subCrit9 = new SubCriteria()
        {
            Name = "Auto",
            Percentage = 90,
            TenantId = "antwerpen"
        };
        var subCrit10 = new SubCriteria()
        {
            Name = "Hoog opgeleid",
            Percentage = 10,
            TenantId = "antwerpen"
        };
        var subCrit11 = new SubCriteria()
        {
            Name = "Laag opgeleid",
            Percentage = 90,
            TenantId = "antwerpen"
        };

        _panelDbContext.SubCriteria.AddRange(subCrit1, subCrit2, subCrit3, subCrit4, subCrit5, subCrit6, subCrit7, subCrit8, subCrit9, subCrit10, subCrit11);

        var crit1 = new Criteria()
        {
            Name = "Geslacht",
            SubCriteria = { subCrit1, subCrit2 },
            TenantId = "antwerpen"
        };
        var crit2 = new Criteria()
        {
            Name = "Leeftijd",
            SubCriteria = { subCrit3, subCrit4, subCrit5, subCrit6, subCrit7 },
            TenantId = "antwerpen"
        };
        var crit3 = new Criteria()
        {
            Name = "Vervoer",
            SubCriteria = { subCrit8, subCrit9 },
            TenantId = "antwerpen"
        };
        var crit4 = new Criteria()
        {
            Name = "Opleiding",
            SubCriteria = { subCrit10, subCrit11 },
            TenantId = "antwerpen"
        };
        _panelDbContext.Criteria.AddRange(crit1, crit2, crit3, crit4);


        // Create panel objects
        var panel1 = new Panel()
        {
            Name = "Panel Antwerpen",
            Description = "Dit is de omschrijving van het panel.",
            StartDate = new DateOnly(2025, 1, 12),
            EndDate = new DateOnly(2025, 7, 22),
            DrawStatus = DrawStatus.FirstPhaseActive,
            TenantId = "antwerpen",
            Meetings = new List<Meeting>()
            {
                new Meeting()
                {
                    Title = "Eerste bijeenkomst",
                    Date = new DateOnly(2025, 4, 12),
                    Recommendations = new List<Recommendation>()
                    {
                        new Recommendation()
                        {
                            Title = "Meer bomen",
                            Description = "We willen graag meer bomen planten in de stad",
                            Votes = 0,
                            TenantId = "antwerpen",
                            IsVotable = true,
                            NeededVotes = 5
                        },
                        new Recommendation()
                        {
                            Title = "Minder afval",
                            Description = "Minder afval op de straat door meer vuilnisbakken te plaatsen",
                            Votes = 0,
                            TenantId = "antwerpen",
                            IsVotable = true,
                            NeededVotes = 5
                        }
                    },
                    TenantId = "antwerpen"
                },
                new Meeting()
                {
                    Title = "Tweede bijeenkomst",
                    Date = new DateOnly(2025, 5, 1),
                    Recommendations = new List<Recommendation>()
                    {
                        new Recommendation()
                        {
                            Title = "Betere wegen",
                            Description = "We willen betere wegen aanleggen in de gemeente antwerpen",
                            Votes = 0,
                            TenantId = "antwerpen",
                            IsVotable = true,
                            NeededVotes = 10,
                            IsAnonymous = true
                        }
                    },
                    TenantId = "antwerpen"
                },
                new Meeting()
                {
                    Title = "Derde bijeenkomst",
                    Date = new DateOnly(2025, 6, 12),
                    Recommendations = new List<Recommendation>(),
                    TenantId = "antwerpen"
                }
            }
        };
        _panelDbContext.Panels.Add(panel1);
        panel1.Criteria.Add(crit1);
        panel1.Criteria.Add(crit2);
        panel1.Criteria.Add(crit3);
        panel1.Criteria.Add(crit4);

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
        panel1.Members.Add(paul?.MemberProfile);
        //panel3.Members.Add(paul?.MemberProfile);
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
        
        var userVotesRec1 = new List<UserVote>
        {
            new UserVote
            {
                Voter = members[0],
                Recommended = false,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            },
            new UserVote
            {
                Voter = members[1],
                Recommended = true,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            },
            new UserVote
            {
                Voter = members[2],
                Recommended = true,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            },
            new UserVote
            {
                Voter = members[3],
                Recommended = false,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            },
            new UserVote
            {
                Voter = members[4],
                Recommended = false,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            }
        };

        var userVotesRec2 = new List<UserVote>
        {
            new UserVote
            {
                Voter = members[0],
                Recommended = false,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            },
            new UserVote
            {
                Voter = members[1],
                Recommended = true,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            },
            new UserVote
            {
                Voter = members[2],
                Recommended = false,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            },
            new UserVote
            {
                Voter = members[3],
                Recommended = true,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            }
        };

        var rec1 = panel1.Meetings.FirstOrDefault().Recommendations.FirstOrDefault();
        rec1.UserVotes = userVotesRec1;
        rec1.Votes = userVotesRec1.Count();
        
        var rec2 = panel1.Meetings.FirstOrDefault().Recommendations.LastOrDefault();
        rec2.UserVotes = userVotesRec2;
        rec2.Votes = userVotesRec2.Count();
        
        _panelDbContext.AddRange(members);
    }

    private void SeedInvitations()
    {
        //invitations
        var invitation1 = new Invitation
        {
            //TenantId = "antwerpen",
            Age = 30,
            Code = "blLy-Tvxl-1TXG-nYg3-CBtD",
            Gender = Gender.Female,
            PanelId = 1,
            Town = "2140",
            QRCodeString =
                "",
            TenantId = "antwerpen"
        };

        var invitation2 = new Invitation
        {
            //TenantId = "antwerpen",
            Age = 20,
            Code = "tDK6-KFhD-ipIi-tQjz-m6cZ",
            Gender = Gender.Male,
            PanelId = 1,
            Town = "2100",
            QRCodeString =
                "",
            TenantId = "antwerpen"
        };

        var invitation3 = new Invitation
        {
            //TenantId = "antwerpen",
            Age = 40,
            Code = "Iga7-FHyw-yVfo-8wvN-Bxa8",
            Gender = Gender.Male,
            PanelId = 1,
            Town = "2110",
            QRCodeString =
                "",
            TenantId = "antwerpen"
        };

        var invitation4 = new Invitation
        {
            //TenantId = "antwerpen",
            Age = 40,
            Code = "FIcV-zyXs-reXO-I6dh-rMCB",
            Gender = Gender.Female,
            PanelId = 2,
            Town = "2105",
            QRCodeString =
                "",
            TenantId = "antwerpen"
        };

        var invitation5 = new Invitation
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
            SelectedCriteria = new List<int>([8, 11]),
            TenantId = "antwerpen"
        };
        _panelDbContext.Invitations.AddRange(invitation1, invitation2, invitation3, invitation4, invitation5);
    }

    private void SeedQuestionnaires()
    {
        var questionnaire = new Questionnaire
        {
            Title = "Verkenningsmodule"
        };
        var question1 = new Question
        {
            Description =
                "Beschik je als organisator nog over minstens 6 maanden voordat de input van de participatie klaar moet zijn voor de politieke besluitvorming?",
            Weight = 5,
            Position = 1
        };
        var answer1 = new Answer
        {
            Description = "Ja",
            Position = 1,
            Advice = "U heeft genoeg tijd dus een panel kan een goed idee zijn.",
            IsCritical = false
        };
        var answer2 = new Answer
        {
            Description = "Nee",
            Position = 2,
            Advice = "U heeft niet genoeg tijd dus een panel is geen goed idee voor u.",
            IsCritical = true
        };
        var question2 = new Question
        {
            Description =
                "Is de gemeente bereid om de realisatie van de voorstellen van het burgerpanel ernstig te overwegen en minstens publiek te motiveren waarom dat niet is gebeurd?",
            Weight = 3,
            Position = 2
        };
        var answer3 = new Answer
        {
            Description = "Ja",
            Position = 1,
            Advice = "Als u bereid bent om de voorstellen ernstig te nemen is een panel een goed idee voor u.",
            IsCritical = false
        };
        var answer4 = new Answer
        {
            Description = "Nee",
            Position = 2,
            Advice = "Als u niet bereid bent om de voorstellen ernstig te nemen is een panel geen goed idee voor u.",
            IsCritical = true
        };
        var question3 = new Question
        {
            Description = "Wat is de bedoeling van het participatieproces bij dit vraagstuk?",
            Weight = 5,
            Position = 3
        };
        var answer5 = new Answer
        {
            Description = "We willen de mening horen van al wie vrijwillig wil deelnemen aan het debat. Iedereen moet kunnen deelnemen.",
            Position = 1,
            Advice = "Een panel wordt zorgvuldig bijeengesteld zodat iedereen gerepresenteerd wordt, als iedereen kan meedoen is dit panel geen goed idee voor u.",
            IsCritical = true
        };
        var answer6 = new Answer
        {
            Description = "We willen de mening horen van doelgroepen die vaak afwezig blijven bij participatie.",
            Position = 2,
            Advice = "Een panel wordt zorgvuldig bijeengesteld zodat iedereen gerepresenteerd wordt, een panel is dus een goed idee voor u.",
            IsCritical = false
        };
        var answer7 = new Answer
        {
            Description = "We willen de mening van een representatief staal participanten horen.",
            Position = 5,
            Advice = "Een panel wordt zorgvuldig bijeengesteld zodat iedereen gerepresenteerd wordt, een panel is dus een goed idee voor u.",
            IsCritical = false
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

        answer5.Question = question3;
        answer6.Question = question3;
        answer7.Question = question3;
        question3.Answers.Add(answer5);
        question3.Answers.Add(answer6);
        question3.Answers.Add(answer7);
        question3.Questionnaire = questionnaire;
        questionnaire.Questions.Add(question3);
        
        _panelDbContext.Questionnaires.Add(questionnaire);
        _panelDbContext.Questions.AddRange(question1, question2, question3);
        _panelDbContext.Answers.AddRange(answer1, answer2, answer3, answer4, answer5, answer6, answer7);
        
        var questionnaire2 = new Questionnaire
        {
            Title = "Procesbepalingsmodule"
        };
        var question4 = new Question
        {
            Description = "Wat wilt u als organisatie bereiken?",
            Weight = 5,
            Position = 1,
            IsDetail = true
        };
        var answer11 = new Answer
        {
            Description = "Aanbevelingen",
            Position = 1
        };
        var answer12 = new Answer
        {
            Description = "Actieplannen",
            Position = 2
        };
        var answer13 = new Answer
        {
            Description = "Gemeenschap inzichten",
            Position = 3
        };
        var question5 = new Question
        {
            Description = "Rond welke problematiek wilt u als organisatie werken?",
            Weight = 3,
            Position = 2,
            IsDetail = true
        };
        var answer8 = new Answer
        {
            Description = "Klimaatregelingen",
            Position = 1
        };
        var answer9 = new Answer
        {
            Description = "Stedelijke planning",
            Position = 2
        };
        var answer10 = new Answer
        {
            Description = "Begroting verdeling",
            Position = 3
        };
        var question6 = new Question
        {
            Description = "Hoe wilt u samenkomen?",
            Weight = 3,
            Position = 3,
            IsDetail = true
        };
        var answer14 = new Answer
        {
            Description = "In een zaal",
            Position = 1,
            Question = question6
        };
        var answer15 = new Answer
        {
            Description = "Online",
            Position = 2,
            Question = question6
        };
        var question7 = new Question
        {
            Description = "Welke output wilt u op het eind van het panel genereren?",
            Weight = 3,
            Position = 4,
            IsDetail = true
        };
        var answer16 = new Answer
        {
            Description = "Rapport",
            Position = 1,
            Question = question7
        };
        var answer17 = new Answer
        {
            Description = "Presentatie",
            Position = 2,
            Question = question7
        };
        var question8 = new Question
        {
            Description = "Hoe ziet u het rekruteren van deelnemers?",
            Weight = 3,
            Position = 5,
            IsDetail = false
        };
        var answer18 = new Answer
        {
            Description = "Ambitieus en duur",
            Advice = "een “state of the art” loting",
            Position = 1,
            Question = question8
        };
        var answer19 = new Answer
        {
            Description = "Weinig ambitieus en goedkoop",
            Advice = "open oproep voor deelnemers, selectie daaruit per bevolkingssegment en verder actief aangevuld met “afwezige doelgroepen”",
            Position = 2,
            Question = question8
        };
        var question9 = new Question
        {
            Description = "Hoe ziet u het organiseren van bijeenkomsten?",
            Weight = 3,
            Position = 6,
            IsDetail = false
        };
        var answer20 = new Answer
        {
            Description = "Heel ambitieus",
            Advice = "1 weekend vorming en informatie van talrijke experts, betrokkenen... over de materie, exhaustief infopakket; 2 debatweekends met finaal een stemming via digitale tool",
            Position = 1,
            Question = question9
        };
        var answer21 = new Answer
        {
            Description = "Redelijke ambitieus",
            Advice = "1 halve dag vorming en informatie..., infopakket; 2 debatdagen met finaal stemming via digitale tool",
            Position = 2,
            Question = question9
        };
        var answer22 = new Answer
        {
            Description = "Weinig ambitieus",
            Advice = "Een folder en korte introductie bij start van de deliberatie zelf, stemming bij handopsteking",
            Position = 3,
            Question = question9
        };
        var question10 = new Question
        {
            Description = "Hoe ziet u het organiseren van bijeenkomsten?",
            Weight = 3,
            Position = 7,
            IsDetail = false
        };
        var answer23 = new Answer
        {
            Description = "Heel ambitieus",
            Advice = "campagne met online filmpjes enz.",
            Position = 1,
            Question = question10
        };
        var answer24 = new Answer
        {
            Description = "Weinig ambitieus",
            Advice = "artikel op website en infoblad",
            Position = 2,
            Question = question10
        };

        answer11.Question = question4;
        answer12.Question = question4;
        answer13.Question = question4;
        question4.Answers.Add(answer11);
        question4.Answers.Add(answer12);
        question4.Answers.Add(answer13);
        question4.Questionnaire = questionnaire2;
        questionnaire2.Questions.Add(question4);

        answer8.Question = question5;
        answer9.Question = question5;
        answer10.Question = question5;
        question5.Answers.Add(answer8);
        question5.Answers.Add(answer9);
        question5.Answers.Add(answer10);
        question5.Questionnaire = questionnaire2;
        questionnaire2.Questions.Add(question5);
        
        question6.Answers.Add(answer14);
        question6.Answers.Add(answer15);
        question6.Questionnaire = questionnaire2;
        questionnaire2.Questions.Add(question6);
        
        question7.Answers.Add(answer16);
        question7.Answers.Add(answer17);
        question7.Questionnaire = questionnaire2;
        questionnaire2.Questions.Add(question7);
        
        question8.Answers.Add(answer18);
        question8.Answers.Add(answer19);
        question8.Questionnaire = questionnaire2;
        questionnaire2.Questions.Add(question8);
        
        question9.Answers.Add(answer20);
        question9.Answers.Add(answer21);
        question9.Answers.Add(answer22);
        question9.Questionnaire = questionnaire2;
        questionnaire2.Questions.Add(question9);
        
        question10.Answers.Add(answer23);
        question10.Answers.Add(answer24);
        question10.Questionnaire = questionnaire2;
        questionnaire2.Questions.Add(question10);

        _panelDbContext.Questionnaires.Add(questionnaire2);
        _panelDbContext.Questions.AddRange(question4, question5, question6, question7, question8, question9, question10);
        _panelDbContext.Answers.AddRange(answer11, answer12, answer13, answer8, answer9, answer10, answer11, answer12, answer13, answer14, answer15, answer16, answer17, answer18, answer19, answer20, answer21, answer22, answer23, answer24);
    }
}