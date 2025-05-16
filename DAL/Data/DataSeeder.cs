using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Data;

public class DataSeeder(PanelDbContext panelDbContext)
{
    public void Seed()
    {
        SeedTenants();
        SeedPanels();
        panelDbContext.SaveChanges();
        panelDbContext.ChangeTracker.Clear();

        SeedInvitations();
        SeedQuestionnaires();

        panelDbContext.SaveChanges();
        panelDbContext.ChangeTracker.Clear();
    }

    private void SeedTenants()
    {
        var tenant1 = new Tenant
        {
            Id = "antwerpen",
            Name = "Antwerpen"
        };
        var tenant2 = new Tenant
        {
            Id = "brussel",
            Name = "Brussel"
        };
        panelDbContext.Tenants.AddRange(tenant1, tenant2);
    }
    
    private void SeedPanels()
    {
        //criteria
        var subCrit1 = new SubCriteria() { Name = "Man", Percentage = 40, TenantId = "antwerpen" };
        var subCrit2 = new SubCriteria() { Name = "Vrouw", Percentage = 60, TenantId = "antwerpen" };
        var subCrit3 = new SubCriteria() { Name = "18-25", Percentage = 20, TenantId = "antwerpen" };
        var subCrit4 = new SubCriteria() { Name = "26-35", Percentage = 50, TenantId = "antwerpen" };
        var subCrit5 = new SubCriteria() { Name = "36-50", Percentage = 10, TenantId = "antwerpen" };
        var subCrit6 = new SubCriteria() { Name = "51-60", Percentage = 10, TenantId = "antwerpen" };
        var subCrit7 = new SubCriteria() { Name = "60+", Percentage = 10, TenantId = "antwerpen" };
        var subCrit8 = new SubCriteria() { Name = "Fiets", Percentage = 30, TenantId = "antwerpen" };
        var subCrit9 = new SubCriteria() { Name = "Auto", Percentage = 70, TenantId = "antwerpen" };
        var subCrit10 = new SubCriteria() { Name = "Hoog opgeleid", Percentage = 50, TenantId = "antwerpen" };
        var subCrit11 = new SubCriteria() { Name = "Laag opgeleid", Percentage = 50, TenantId = "antwerpen" };
        
        panelDbContext.SubCriteria.AddRange(subCrit1, subCrit2, subCrit3, subCrit4, subCrit5, subCrit6, subCrit7, subCrit8, subCrit9, subCrit10, subCrit11);

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
        panelDbContext.Criteria.AddRange(crit1, crit2, crit3, crit4);


        // Create panel objects
        var panel1 = new Panel()
        {
            Name = "Panel Antwerpen",
            Description = "Dit is de omschrijving van het panel.",
            StartDate = new DateOnly(2025, 1, 12),
            EndDate = new DateOnly(2025, 7, 22),
            DrawStatus = DrawStatus.FirstPhaseActive,
            TenantId = "antwerpen",
            TotalAvailablePotentialPanelmembers = 10000,
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
        panelDbContext.Panels.Add(panel1);
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
        panelDbContext.Panels.Add(panel2);
        
        var panel3 = new Panel()
        {
            Name = "Panel Antwerpen 2",
            Description = "Dit is ook nog een omschrijving van een panel",
            StartDate = new DateOnly(2025, 3, 11),
            EndDate = new DateOnly(2025, 7, 17),
            TenantId = "antwerpen"
        };
        panelDbContext.Panels.Add(panel3);
        
        var antwerpen = panelDbContext.Users
            .Include(u => u.OrganizationProfile)
            .SingleOrDefault(u => u.UserName == "antwerpen@example.com");
        var brussel = panelDbContext.Users
            .Include(u => u.OrganizationProfile)
            .SingleOrDefault(u => u.UserName == "brussel@example.com");
        var paul = panelDbContext.Users
            .Include(u => u.MemberProfile)
            .SingleOrDefault(u => u.UserName == "paul@example.com");
        panel1.Members.Add(paul?.MemberProfile);
        //panel3.Members.Add(paul?.MemberProfile);
        paul?.MemberProfile.Panels.Add(panel1);
        //paul?.MemberProfile.Panels.Add(panel3);
        
        

        var members = new List<ApplicationUser>
        {
new ApplicationUser
            {
                Email = "kate@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Kate",
                    Gender = Gender.Female,
                    Age = 65,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria>()
                    {
                        subCrit9,subCrit10
                    }
                }
            },
            new ApplicationUser
            {
                Email = "rozie@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Rozie",
                    Gender = Gender.Female,
                    Age = 77,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria>()
                    {
                        subCrit9,subCrit10
                    }
                }
            },
            new ApplicationUser
            {
                Email = "rozalinda@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Rozalinda",
                    Gender = Gender.Female,
                    Age = 69,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria>()
                    {
                        subCrit8,subCrit10
                    }
                }
            },
            new ApplicationUser
            {
                Email = "alice2@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Alice",
                    Gender = Gender.Female,
                    Age = 73,
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria>()
                    {
                        subCrit8,subCrit10
                    }
                }
            },
            new ApplicationUser
            {
            Email = "emma3@example.com",
            UserType = UserType.Member,
            MemberProfile = new MemberProfile
            {
                FirstName = "Emma",
                Gender = Gender.Female,
                Age = 24,
                Town = "Antwerpen",
                Panels = new List<Panel> { panel1 },
                TenantId = "antwerpen",
                SelectedCriteria = new List<SubCriteria>()
                {
                    subCrit8,subCrit10
                }
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
        
        panelDbContext.AddRange(members);
    }

    private void SeedInvitations()
    {
        
        // Create initial list with members for Panel 1 and Panel 2
        var invitations = new List<Invitation>
        {
            new Invitation()
            {
                Email = "jan@zonderid.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            
            // Panel 1 members
            new Invitation()
            {
                Email = "els@example.com",
                Gender = Gender.Female,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "1wer-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "bart@example.com",
                Gender = Gender.Male,
                Age = 36,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "q1er-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "sophie2@example.com",
                Gender = Gender.Female,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qw1r-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 11]),
            },
            new Invitation()
            {
                Email = "tom@example.com",
                Gender = Gender.Male,
                Age = 36,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwe1-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },

            // Additional members for Panel 1
            new Invitation()
            {
                Email = "jan@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-1yui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "peter@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-t1ui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "david@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-ty1i-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "johan@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyu1-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "koen@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-1pas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "simon@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-o1as-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },

            // Men 26-40
            new Invitation()
            {
                Email = "thomas@example.com",
                Gender = Gender.Male,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-op1s-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "maarten@example.com",
                Gender = Gender.Male,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opa1-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "jeroen@example.com",
                Gender = Gender.Male,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-1fgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "pieter@example.com",
                Gender = Gender.Male,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-d1gh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "wouter@example.com",
                Gender = Gender.Male,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-df1h-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "michel@example.com",
                Gender = Gender.Male,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfg1-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },

            // Men 41-60
            new Invitation()
            {
                Email = "frank@example.com",
                Gender = Gender.Male,
                Age = 36,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfgh-1klz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "marc@example.com",
                Gender = Gender.Male,
                Age = 51,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfgh-j1lz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "patrick@example.com",
                Gender = Gender.Male,
                Age = 51,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfgh-jk1z",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "dirk@example.com",
                Gender = Gender.Male,
                Age = 36,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfgh-jkl1",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "hans@example.com",
                Gender = Gender.Male,
                Age = 36,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "2wer-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "erik@example.com",
                Gender = Gender.Male,
                Age = 51,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "q2er-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },

            // Men 60+
            new Invitation()
            {
                Email = "jozef@example.com",
                Gender = Gender.Male,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qw2r-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "willem@example.com",
                Gender = Gender.Male,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwe2-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "gerard@example.com",
                Gender = Gender.Male,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-2yui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "robert@example.com",
                Gender = Gender.Male,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-t2ui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "hugo@example.com",
                Gender = Gender.Male,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-ty2i-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "albert@example.com",
                Gender = Gender.Male,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyu2-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },

            // Women 18-25
            new Invitation()
            {
                Email = "anna@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-2pas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "lisa2@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-o2as-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 11]),
            },
            new Invitation()
            {
                Email = "emma2@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-op2s-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 11]),
            },
            new Invitation()
            {
                Email = "sara@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opa2-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 11]),
            },
            new Invitation()
            {
                Email = "laura@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-2fgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 11]),
            },
            new Invitation()
            {
                Email = "nina@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-d2gh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 11]),
            },

            // Women 26-40
            new Invitation()
            {
                Email = "eva@example.com",
                Gender = Gender.Female,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-df2h-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 11]),
            },
            new Invitation()
            {
                Email = "sophie@example.com",
                Gender = Gender.Female,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfg2-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 11]),
            },
            new Invitation()
            {
                Email = "julie@example.com",
                Gender = Gender.Female,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfgh-2klz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 11]),
            },
            new Invitation()
            {
                Email = "els2@example.com",
                Gender = Gender.Female,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfgh-j2lz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 11]),
            },
            new Invitation()
            {
                Email = "lieve@example.com",
                Gender = Gender.Female,
                Age = 26,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfgh-jk2z",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "anja@example.com",
                Gender = Gender.Female,
                Age = 36,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opas-dfgh-jkl2",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },

            // Women 41-60
            new Invitation()
            {
                Email = "maria@example.com",
                Gender = Gender.Female,
                Age = 36,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "3wer-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "ann@example.com",
                Gender = Gender.Female,
                Age = 51,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "q3er-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "ingrid@example.com",
                Gender = Gender.Female,
                Age = 51,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qw3r-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "martine@example.com",
                Gender = Gender.Female,
                Age = 36,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwe3-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "hilde@example.com",
                Gender = Gender.Female,
                Age = 36,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-3yui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "sonja@example.com",
                Gender = Gender.Female,
                Age = 51,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-t3ui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },

            // Women 60+
            new Invitation()
            {
                Email = "helena@example.com",
                Gender = Gender.Female,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-ty3i-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "godelieve@example.com",
                Gender = Gender.Female,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyu3-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "rosa@example.com",
                Gender = Gender.Female,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-3pas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "margareta@example.com",
                Gender = Gender.Female,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-o3as-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([9, 10]),
            },
            new Invitation()
            {
                Email = "mariette@example.com",
                Gender = Gender.Female,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-op3s-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "alice@example.com",
                Gender = Gender.Female,
                Age = 61,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Code = "qwer-tyui-opa3-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            }
            
        };
        //invitations
        var invitation1 = new Invitation()
        {
            Email = "invi1@example.com",
            Age = 26,
            Code = "blLy-Tvxl-1TXG-nYg3-CBtD",
            Gender = Gender.Female,
            PanelId = 1,
            Town = "Antwerpen",
            QRCodeString =
                "",
            TenantId = "antwerpen"
        };

        var invitation2 = new Invitation
        {
            Email = "invi2@example.com",
            Age = 18,
            Code = "tDK6-KFhD-ipIi-tQjz-m6cZ",
            Gender = Gender.Male,
            PanelId = 1,
            Town = "Antwerpen",
            QRCodeString =
                "",
            TenantId = "antwerpen"
        };

        var invitation3 = new Invitation
        {
            Email = "invi3@example.com",
            Age = 36,
            Code = "Iga7-FHyw-yVfo-8wvN-Bxa8",
            Gender = Gender.Male,
            PanelId = 1,
            Town = "Antwerpen",
            QRCodeString =
                "",
            TenantId = "antwerpen"
        };

        var invitation4 = new Invitation
        {
            Email = "invi4@example.com",
            Age = 36,
            Code = "FIcV-zyXs-reXO-I6dh-rMCB",
            Gender = Gender.Female,
            PanelId = 2,
            Town = "Antwerpen",
            QRCodeString =
                "",
            TenantId = "antwerpen"
        };

        var invitation5 = new Invitation
        {
            Age = 26,
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
        panelDbContext.Invitations.AddRange(invitation1, invitation2, invitation3, invitation4, invitation5);
        panelDbContext.Invitations.AddRange(invitations);
    }

    private void SeedQuestionnaires()
    {
        var questionnaire = new Questionnaire
        {
            Title = "Verkenningsmodule",
            Subtitle = "Is een panel voor u?"
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
        
        panelDbContext.Questionnaires.Add(questionnaire);
        panelDbContext.Questions.AddRange(question1, question2, question3);
        panelDbContext.Answers.AddRange(answer1, answer2, answer3, answer4, answer5, answer6, answer7);
        
        var questionnaire2 = new Questionnaire
        {
            Title = "Procesbepalingsmodule",
            Subtitle = "Hoe wilt u uw panel aanpakken?"
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
            Description = "Hoe wilt u de resultaten communiceren?",
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

        panelDbContext.Questionnaires.Add(questionnaire2);
        panelDbContext.Questions.AddRange(question4, question5, question6, question7, question8, question9, question10);
        panelDbContext.Answers.AddRange(answer11, answer12, answer13, answer8, answer9, answer10, answer11, answer12, answer13, answer14, answer15, answer16, answer17, answer18, answer19, answer20, answer21, answer22, answer23, answer24);
    }
}