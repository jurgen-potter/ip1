using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.PanelManagement;
using CitizenPanel.BL.Domain.Recruitment;
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
            Name = "Panel 1"
        };
        //panel1.ExtraCriteria.Add(extraCrit1);
        //panel1.ExtraCriteria.Add(extraCrit2);

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
                Id = "1", FirstName = "Jan", LastName = "Janssen", Email = "jan@example.com", Gender = Gender.Male,
                Age = 22, Town = "Antwerpen", Panel = panel1, IsSelected = false
            },
            new Member
            {
                Id = "2", FirstName = "Els", LastName = "Peeters", Email = "els@example.com", Gender = Gender.Female,
                Age = 35, Town = "Antwerpen", Panel = panel1, IsSelected = false
            },
            new Member
            {
                Id = "3", FirstName = "Bart", LastName = "Mertens", Email = "bart@example.com", Gender = Gender.Male,
                Age = 50, Town = "Gent", Panel = panel1, IsSelected = false
            },
            new Member
            {
                Id = "4", FirstName = "Sophie", LastName = "Vermeulen", Email = "sophie@example.com",
                Gender = Gender.Female, Age = 28, Town = "Antwerpen", Panel = panel1, IsSelected = false
            },
            new Member
            {
                Id = "5", FirstName = "Tom", LastName = "Wouters", Email = "tom@example.com", Gender = Gender.Male,
                Age = 45, Town = "Antwerpen", Panel = panel1, IsSelected = false
            },

            // Panel 2 members
            new Member
            {
                Id = "6", FirstName = "Lisa", LastName = "Jacobs", Email = "lisa@example.com", Gender = Gender.Female,
                Age = 33, Town = "Mechelen", Panel = panel2, IsSelected = false
            },
            new Member
            {
                Id = "7", FirstName = "Mark", LastName = "Stevens", Email = "mark@example.com", Gender = Gender.Male,
                Age = 67, Town = "Antwerpen", Panel = panel2, IsSelected = false
            },
            new Member
            {
                Id = "8", FirstName = "Lotte", LastName = "Maes", Email = "lotte@example.com", Gender = Gender.Female,
                Age = 72, Town = "Antwerpen", Panel = panel2, IsSelected = false
            },
            new Member
            {
                Id = "9", FirstName = "Pieter", LastName = "Vandenberghe", Email = "pieter@example.com",
                Gender = Gender.Male, Age = 19, Town = "Antwerpen", Panel = panel2, IsSelected = false
            },
            new Member
            {
                Id = "10", FirstName = "Emma", LastName = "De Smet", Email = "emma@example.com", Gender = Gender.Female,
                Age = 21, Town = "Leuven", Panel = panel2, IsSelected = false
            }
        };

        // Add additional members for Panel 1
        members.AddRange(new[]
        {
            new Member
            {
                Id = "101", FirstName = "Jan", LastName = "Janssen", Email = "jan@example.com", Gender = Gender.Male,
                Age = 22, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Id = "102", FirstName = "Peter", LastName = "Peters", Email = "peter@example.com", Gender = Gender.Male,
                Age = 19, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Id = "103", FirstName = "David", LastName = "Davids", Email = "david@example.com", Gender = Gender.Male,
                Age = 24, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Id = "104", FirstName = "Johan", LastName = "Johanssen", Email = "johan@example.com",
                Gender = Gender.Male, Age = 21, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Id = "105", FirstName = "Koen", LastName = "Koens", Email = "koen@example.com", Gender = Gender.Male,
                Age = 25, Town = "Antwerpen", Panel = panel1
            },
            new Member
            {
                Id = "106", FirstName = "Simon", LastName = "Simons", Email = "simon@example.com", Gender = Gender.Male,
                Age = 18, Town = "Antwerpen", Panel = panel1
            },
        });

        // Additional groups for Panel 1 (Men 26-40, 41-60, 60+; Women 18-25, 26-40, 41-60, 60+)
        // For brevity, the example below shows one additional group. Follow the same approach for the rest.
        // Men 26-40
        members.AddRange(new[]
        {
            new Member
            {
                Id = "107", FirstName = "Thomas", LastName = "Thomassen", Email = "thomas@example.com",
                Gender = Gender.Male, Age = 35, Town = "Gent", Panel = panel1
            },
            new Member
            {
                Id = "108", FirstName = "Maarten", LastName = "Maartens", Email = "maarten@example.com",
                Gender = Gender.Male, Age = 28, Town = "Gent", Panel = panel1
            },
            new Member
            {
                Id = "109", FirstName = "Jeroen", LastName = "Jeroens", Email = "jeroen@example.com",
                Gender = Gender.Male, Age = 37, Town = "Gent", Panel = panel1
            },
            new Member
            {
                Id = "110", FirstName = "Pieter", LastName = "Pieters", Email = "pieter@example.com",
                Gender = Gender.Male, Age = 30, Town = "Gent", Panel = panel1
            },
            new Member
            {
                Id = "111", FirstName = "Wouter", LastName = "Wouters", Email = "wouter@example.com",
                Gender = Gender.Male, Age = 33, Town = "Gent", Panel = panel1
            },
            new Member
            {
                Id = "112", FirstName = "Michel", LastName = "Michels", Email = "michel@example.com",
                Gender = Gender.Male, Age = 39, Town = "Gent", Panel = panel1
            },
        });
        // Men 41-60
        members
            .AddRange(new[]
            {
                new Member
                {
                    Id = "113", FirstName = "Frank", LastName = "Franks", Email = "frank@example.com",
                    Gender = Gender.Male,
                    Age = 45, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "114", FirstName = "Marc", LastName = "Marcs", Email = "marc@example.com",
                    Gender = Gender.Male,
                    Age = 58, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "115", FirstName = "Patrick", LastName = "Patricks", Email = "patrick@example.com",
                    Gender = Gender.Male, Age = 52, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "116", FirstName = "Dirk", LastName = "Dirks", Email = "dirk@example.com",
                    Gender = Gender.Male,
                    Age = 49, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "117", FirstName = "Hans", LastName = "Hansen", Email = "hans@example.com",
                    Gender = Gender.Male,
                    Age = 44, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "118", FirstName = "Erik", LastName = "Eriks", Email = "erik@example.com",
                    Gender = Gender.Male,
                    Age = 55, Town = "Leuven", Panel = panel1
                },
            });

        // Men 60+
        members
            .AddRange(new[]
            {
                new Member
                {
                    Id = "119", FirstName = "Jozef", LastName = "Jozefs", Email = "jozef@example.com",
                    Gender = Gender.Male,
                    Age = 68, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "120", FirstName = "Willem", LastName = "Willems", Email = "willem@example.com",
                    Gender = Gender.Male, Age = 71, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "121", FirstName = "Gerard", LastName = "Gerards", Email = "gerard@example.com",
                    Gender = Gender.Male, Age = 65, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "122", FirstName = "Robert", LastName = "Roberts", Email = "robert@example.com",
                    Gender = Gender.Male, Age = 77, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "123", FirstName = "Hugo", LastName = "Hugos", Email = "hugo@example.com",
                    Gender = Gender.Male,
                    Age = 69, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "124", FirstName = "Albert", LastName = "Alberts", Email = "albert@example.com",
                    Gender = Gender.Male, Age = 73, Town = "Mechelen", Panel = panel1
                },
            });

        // Women 18-25
        members
            .AddRange(new[]
            {
                new Member
                {
                    Id = "125", FirstName = "Anna", LastName = "Annas", Email = "anna@example.com",
                    Gender = Gender.Female,
                    Age = 22, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Id = "126", FirstName = "Lisa", LastName = "Lisas", Email = "lisa@example.com",
                    Gender = Gender.Female,
                    Age = 19, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Id = "127", FirstName = "Emma", LastName = "Emmas", Email = "emma@example.com",
                    Gender = Gender.Female,
                    Age = 24, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Id = "128", FirstName = "Sara", LastName = "Saras", Email = "sara@example.com",
                    Gender = Gender.Female,
                    Age = 21, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Id = "129", FirstName = "Laura", LastName = "Lauras", Email = "laura@example.com",
                    Gender = Gender.Female, Age = 25, Town = "Antwerpen", Panel = panel1
                },
                new Member
                {
                    Id = "130", FirstName = "Nina", LastName = "Ninas", Email = "nina@example.com",
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
                    Id = "131", FirstName = "Eva", LastName = "Evas", Email = "eva@example.com", Gender = Gender.Female,
                    Age = 35, Town = "Gent", Panel = panel1
                },
                new Member
                {
                    Id = "132", FirstName = "Sophie", LastName = "Sophies", Email = "sophie@example.com",
                    Gender = Gender.Female, Age = 28, Town = "Gent", Panel = panel1
                },
                new Member
                {
                    Id = "133", FirstName = "Julie", LastName = "Julies", Email = "julie@example.com",
                    Gender = Gender.Female, Age = 37, Town = "Gent", Panel = panel1
                },
                new Member
                {
                    Id = "134", FirstName = "Els", LastName = "Else", Email = "els@example.com", Gender = Gender.Female,
                    Age = 30, Town = "Gent", Panel = panel1
                },
                new Member
                {
                    Id = "135", FirstName = "Lieve", LastName = "Lieves", Email = "lieve@example.com",
                    Gender = Gender.Female, Age = 33, Town = "Gent", Panel = panel1
                },
                new Member
                {
                    Id = "136", FirstName = "Anja", LastName = "Anjas", Email = "anja@example.com",
                    Gender = Gender.Female,
                    Age = 39, Town = "Gent", Panel = panel1
                },
            });

        // Women 41-60
        members
            .AddRange(new[]
            {
                new Member
                {
                    Id = "137", FirstName = "Maria", LastName = "Marias", Email = "maria@example.com",
                    Gender = Gender.Female, Age = 45, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "138", FirstName = "Ann", LastName = "Anns", Email = "ann@example.com", Gender = Gender.Female,
                    Age = 58, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "139", FirstName = "Ingrid", LastName = "Ingrids", Email = "ingrid@example.com",
                    Gender = Gender.Female, Age = 52, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "140", FirstName = "Martine", LastName = "Martines", Email = "martine@example.com",
                    Gender = Gender.Female, Age = 49, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "141", FirstName = "Hilde", LastName = "Hildes", Email = "hilde@example.com",
                    Gender = Gender.Female, Age = 44, Town = "Leuven", Panel = panel1
                },
                new Member
                {
                    Id = "142", FirstName = "Sonja", LastName = "Sonjas", Email = "sonja@example.com",
                    Gender = Gender.Female, Age = 55, Town = "Leuven", Panel = panel1
                },
            });

        // Women 60+
        members
            .AddRange(new[]
            {
                new Member
                {
                    Id = "143", FirstName = "Helena", LastName = "Helenas", Email = "helena@example.com",
                    Gender = Gender.Female, Age = 68, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "144", FirstName = "Godelieve", LastName = "Godelieves", Email = "godelieve@example.com",
                    Gender = Gender.Female, Age = 71, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "145", FirstName = "Rosa", LastName = "Rosas", Email = "rosa@example.com",
                    Gender = Gender.Female,
                    Age = 65, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "146", FirstName = "Margareta", LastName = "Margaretas", Email = "margareta@example.com",
                    Gender = Gender.Female, Age = 77, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "147", FirstName = "Mariette", LastName = "Mariettes", Email = "mariette@example.com",
                    Gender = Gender.Female, Age = 69, Town = "Mechelen", Panel = panel1
                },
                new Member
                {
                    Id = "148", FirstName = "Alice", LastName = "Alices", Email = "alice@example.com",
                    Gender = Gender.Female, Age = 73, Town = "Mechelen", Panel = panel1
                },
            });
        
        _panelDbContext.AddRange(members);
        
        
        //invitations
        var invitation1 = new Invitation()
        {
            Age = 30,
            Code = "2-nq1E-hjv1-mij2-il4-gll1-io",
            Gender = Gender.Female,
            PanelId = 1,
            Postcode = "2140",
            QRCodeString = "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"
        };

        var invitation2 = new Invitation()
        {
            Age = 20,
            Code = "2-np14-vwj1-tuu1-ntv-lqh1-oq",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2100",
            QRCodeString = "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"
        };

        var invitation3 = new Invitation()
        {
            Age = 40,
            Code = "2-yj28-wvk1-uvu1-mt1-mwt1-pp",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2110",
            QRCodeString = "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"
        };
                
        var invitation4 = new Invitation()
        {
            Age = 40,
            Code = "2-nx28-tij1-tpw2-ihy-oyj2-w5",
            Gender = Gender.Female,
            PanelId = 2,
            Postcode = "2105",
            QRCodeString = "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"
        };
        
        var invitation5 = new Invitation()
        {
            Age = 25,
            Code = "2-si19-xik1-njk1-kj4-sgx1-wt",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2140",
            QRCodeString = "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"
        };
        _panelDbContext.Invitations.Add(invitation1);
        _panelDbContext.Invitations.Add(invitation2);
        _panelDbContext.Invitations.Add(invitation3);
        _panelDbContext.Invitations.Add(invitation4);
        _panelDbContext.Invitations.Add(invitation5);
        
        _panelDbContext.SaveChanges();
        _panelDbContext.ChangeTracker.Clear();
    }
}