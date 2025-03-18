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
        // Create panel objects
        var panel1 = new Panel()
        {
            PanelId = 1,
            Name = "Panel 1",
        };

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
        
        
        //invitations
        var invitation1 = new Invitation()
        {
            Age = 30,
            Code = "2-xv1E-trl1-xml2-rw4-lms1-oz",
            Gender = Gender.Female,
            PanelId = 1,
            Postcode = "2140",
            QRCodeString = "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"
        };

        var invitation2 = new Invitation()
        {
            Age = 20,
            Code = "2-ur14-tll1-xvl1-lxl-ykn1-kx",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2100",
            QRCodeString = "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"
        };

        var invitation3 = new Invitation()
        {
            Age = 40,
            Code = "2-so28-znw1-zvo1-mn1-rqr1-ip",
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2110",
            QRCodeString = "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"
        };
                
        var invitation4 = new Invitation()
        {
            Age = 40,
            Code = "\t2-tu28-knq1-gzk2-ylu-mzv2-z5",
            Gender = Gender.Female,
            PanelId = 2,
            Postcode = "2105",
            QRCodeString = "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"
        };
        
        var invitation5 = new Invitation()
        {
            Age = 25,
            Code = "2-qq19-lxg1-nyz1-mt4-pno1-kg",
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