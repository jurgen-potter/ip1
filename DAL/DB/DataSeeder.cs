using CitizenPanel.BL.Domain.PanelManagement;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL.EF;

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

        _panelDbContext.SaveChanges();
        _panelDbContext.ChangeTracker.Clear();
    }
}