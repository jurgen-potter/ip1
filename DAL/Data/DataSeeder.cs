using CitizenPanel.BL.Domain.Content;
using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Data;

public class DataSeeder(PanelDbContext panelDbContext)
{
    public void SeedTenants()
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

    public void Seed()
    {
        SeedFiles();
        SeedContent();
        SeedPanels();
        panelDbContext.SaveChanges();
        panelDbContext.ChangeTracker.Clear();

        SeedInvitations();
        SeedQuestionnaires();

        panelDbContext.SaveChanges();
        panelDbContext.ChangeTracker.Clear();
    }

    private void SeedFiles()
    {
        var bannerUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bannerUploads");
        var meetingUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "meetingUploads");
        var panelUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "panelUploads");
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        if (Directory.Exists(bannerUploads)) { Directory.Delete(bannerUploads, recursive: true); }
        if (Directory.Exists(meetingUploads)) { Directory.Delete(meetingUploads, recursive: true); }
        if (Directory.Exists(panelUploads)) { Directory.Delete(panelUploads, recursive: true); }
        if (Directory.Exists(uploads)) { Directory.Delete(uploads, recursive: true); }
    }

    private void SeedContent()
    {
        if (panelDbContext.InfoPageContents.Any()) return;

        var infoPageContent = new InfoPageContent
        {
            MainTitle = "Wat zijn BurgerPanels?",
            Sections = new List<InfoSection>
            {
                new InfoSection
                {
                    Title = "Wat zijn BurgerPanels?",
                    Text = "BurgerPanels zijn gestructureerde vormen van burgerparticipatie waarbij inwoners hun mening kunnen geven over actuele onderwerpen in hun gemeente. Ze helpen beleidsmakers betere beslissingen te nemen die gedragen worden door de gemeenschap."
                },
                new InfoSection
                {
                    Title = "Waarom deelnemen of organiseren?",
                    Text = "• Inspraak verhogen in het beleid\n• Verbinding tussen burgers en bestuur versterken\n• Nieuwe ideeën en perspectieven ophalen"
                },
                new InfoSection
                {
                    Title = "Hoe werkt het?",
                    Text = "1. De gemeente start een panel rond een thema\n2. Burgers worden uitgenodigd om deel te nemen\n3. Meningen worden verzameld via vragenlijsten of sessies\n4. Resultaten worden geanalyseerd en gedeeld"
                },
                new InfoSection
                {
                    Title = "Voorbeelden van thema's",
                    Text = "• Mobiliteit: Hoe kan verkeer veiliger of vlotter worden in jouw buurt?\n• Woonbeleid: Wat heb jij nodig om comfortabel te wonen in jouw stad?"
                }
            }
        };

        panelDbContext.InfoPageContents.Add(infoPageContent);
        panelDbContext.SaveChanges();
    }

    private void SeedPanels()
    {
        // Create panel objects
        var panel1 = new Panel()
        {
            Name = "Leefbare Binnenstad Antwerpen",
            Description = @"De stad Antwerpen wil haar historische binnenstad leefbaar houden voor bewoners, ondernemers en bezoekers. Toenemende drukte, mobiliteit, toerisme, woningprijzen, overlast en klimaatuitdagingen zetten de leefkwaliteit onder druk. Daarom organiseert de stad een burgerpanel over leefbaarheid.

Het panel formuleert op participatieve en representatieve wijze aanbevelingen voor een leefbare binnenstad op lange termijn. Zo benut de stad de kennis en ervaringen van burgers bij toekomstgericht beleid.

Kernvragen van het panel:
- Hoe houden we wonen betaalbaar en kwalitatief?
- Hoe combineren we economische activiteit met rust voor bewoners?
- Hoe maken we de binnenstad groener, gezonder en beter bereikbaar?

Het panel bestaat uit een aantal willekeurig geselecteerde Antwerpenaren, geloot met aandacht voor diversiteit in leeftijd, geslacht, woonplaats, opleiding en achtergrond. Deelnemers ontvangen een vergoeding en goede begeleiding.

Tijdens meerdere sessies krijgen deelnemers informatie van experten en stadsdiensten, waarna ze in dialoog beleidsaanbevelingen formuleren. Een onafhankelijke begeleider ondersteunt het proces.

De aanbevelingen worden publiek gedeeld en aan het stadsbestuur voorgelegd, dat belooft er binnen een afgesproken termijn op te reageren. Zo krijgt het panel echte impact.",
            StartDate = new DateOnly(2025, 1, 12),
            EndDate = new DateOnly(2025, 7, 22),
            DrawStatus = DrawStatus.Complete,
            TenantId = "antwerpen",
            TotalNeededPanelmembers = 100,
            IsActive = true,
            MemberCount = 11,
            Meetings = new List<Meeting>()
            {
                new Meeting()
                {
                    Title = "Eerste bijeenkomst",
                    PanelParticipants = 11,
                    Date = new DateOnly(2025, 4, 12),
                    Recommendations = new List<Recommendation>()
                    {
                        new Recommendation()
                        {
                            Title = "Meer bomen",
                            Description = "We willen graag meer bomen planten in de stad",
                            Votes = 7,
                            TenantId = "antwerpen",
                            IsVotable = false,
                            Accepted = true,
                            IsDone = true,
                            NeededPercentage = 50,
                        },
                        new Recommendation()
                        {
                            Title = "Minder afval",
                            Description = "Minder afval op de straat door meer vuilnisbakken te plaatsen",
                            Votes = 4,
                            TenantId = "antwerpen",
                            IsVotable = false,
                            Accepted = false,
                            NeededPercentage = 50,
                        }
                    },
                    TenantId = "antwerpen"
                },
                new Meeting()
                {
                    Title = "Tweede bijeenkomst",
                    PanelParticipants = 11,
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
                            NeededPercentage = 70,
                            IsAnonymous = true
                        }
                    },
                    TenantId = "antwerpen"
                },
                new Meeting()
                {
                    Title = "Derde bijeenkomst",
                    PanelParticipants = 11,
                    Date = new DateOnly(2025, 6, 12),
                    Recommendations = new List<Recommendation>(),
                    TenantId = "antwerpen"
                }
            }
        };
        panelDbContext.Panels.Add(panel1);
        panelDbContext.SaveChanges();

        var posts = new List<Post>()
        {
            new Post()
            {
                Title = "Eerste bomen geplant",
                Description = "Beste Antwerpenaren,\nWe hebben sinds het aannemen van het voorstel al 63 nieuwe bomen geplant op diverse plaatsen in de binnenstad. In het kader van een groener Antwerpen zullen er ook nog vele volgen",
                AuthorName = "antwerpen@example.com",
                DatePosted = new DateTime(2025, 4, 27, 14, 28, 58, 689, DateTimeKind.Utc).AddTicks(6340),
                PanelId = 1,
                TenantId = "antwerpen"
            },
            new Post()
            {
                Title = "Verdere bomen geplant",
                Description = "Beste Antwerpenaren,\nSinds onze laatste post zijn er nog eens 58 extra bomen geplant.",
                AuthorName = "antwerpen@example.com",
                DatePosted = new DateTime(2025, 5, 2, 14, 28, 58, 689, DateTimeKind.Utc).AddTicks(6340),
                PanelId = 1,
                TenantId = "antwerpen"
            }
        };
        panelDbContext.Posts.AddRange(posts);

        //criteria
        var subCrit21 = new SubCriteria() { Name = "Man", Percentage = 40, TenantId = "antwerpen" };
        var subCrit22 = new SubCriteria() { Name = "Vrouw", Percentage = 60, TenantId = "antwerpen" };
        var subCrit23 = new SubCriteria() { Name = "18-24", Percentage = 20, TenantId = "antwerpen" };
        var subCrit24 = new SubCriteria() { Name = "25-49", Percentage = 50, TenantId = "antwerpen" };
        var subCrit25 = new SubCriteria() { Name = "50-64", Percentage = 20, TenantId = "antwerpen" };
        var subCrit26 = new SubCriteria() { Name = "65+", Percentage = 10, TenantId = "antwerpen" };
        var subCrit27 = new SubCriteria() { Name = "Fiets", Percentage = 30, TenantId = "antwerpen" };
        var subCrit28 = new SubCriteria() { Name = "Auto", Percentage = 70, TenantId = "antwerpen" };
        var subCrit29 = new SubCriteria() { Name = "Hoog opgeleid", Percentage = 50, TenantId = "antwerpen" };
        var subCrit210 = new SubCriteria() { Name = "Laag opgeleid", Percentage = 50, TenantId = "antwerpen" };

        panelDbContext.SubCriteria.AddRange(subCrit21, subCrit22, subCrit23, subCrit24, subCrit25, subCrit26, subCrit27, subCrit28, subCrit29, subCrit210);

        var crit21 = new Criteria()
        {
            Name = "Geslacht",
            SubCriteria = { subCrit21, subCrit22 },
            TenantId = "antwerpen"
        };
        var crit22 = new Criteria()
        {
            Name = "Leeftijd",
            SubCriteria = { subCrit23, subCrit24, subCrit25, subCrit26 },
            TenantId = "antwerpen"
        };
        var crit23 = new Criteria()
        {
            Name = "Vervoer",
            SubCriteria = { subCrit27, subCrit28 },
            TenantId = "antwerpen"
        };
        var crit24 = new Criteria()
        {
            Name = "Opleiding",
            SubCriteria = { subCrit29, subCrit210 },
            TenantId = "antwerpen"
        };
        panelDbContext.Criteria.AddRange(crit21, crit22, crit23, crit24);

        var panel2 = new Panel()
        {
            Name = "Toegankelijke en Veilige Mobiliteit in Antwerpen",
            Description = @"De stad Antwerpen wil mobiliteit duurzaam, veilig en inclusief maken — voor voetgangers, fietsers, mensen met een beperking en andere weggebruikers. Ook wil ze de verkeersdruk verlagen en de leefkwaliteit verbeteren. Daarom wordt een burgerpanel opgericht dat meedenkt over betere en eerlijkere mobiliteit.

Het panel bestaat uit gelote Antwerpenaren die samen nadenken over vragen zoals:

    Hoe maken we verkeer veiliger voor kwetsbare weggebruikers?
    Hoe stimuleren we fietsen en stappen?
    Hoe maken we het openbaar vervoer toegankelijk?
    Hoe verdelen we de schaarse ruimte in de stad rechtvaardig?

Tijdens meerdere sessies krijgen deelnemers info van experten en belanghebbenden, en formuleren ze samen aanbevelingen. De stad engageert zich om deze grondig te bekijken en publiek te beantwoorden.",
            StartDate = new DateOnly(2025, 3, 11),
            EndDate = new DateOnly(2025, 7, 17),
            DrawStatus = DrawStatus.FirstPhaseActive,
            TenantId = "antwerpen",
            TotalNeededPanelmembers = 100
        };
        panelDbContext.Panels.Add(panel2);
        panel2.Criteria.Add(crit21);
        panel2.Criteria.Add(crit22);
        panel2.Criteria.Add(crit23);
        panel2.Criteria.Add(crit24);

        //criteria
        var subCrit31 = new SubCriteria() { Name = "Man", Percentage = 50, TenantId = "brussel" };
        var subCrit32 = new SubCriteria() { Name = "Vrouw", Percentage = 50, TenantId = "brussel" };
        var subCrit33 = new SubCriteria() { Name = "18-24", Percentage = 25, TenantId = "brussel" };
        var subCrit34 = new SubCriteria() { Name = "25-49", Percentage = 25, TenantId = "brussel" };
        var subCrit35 = new SubCriteria() { Name = "50-64", Percentage = 25, TenantId = "brussel" };
        var subCrit36 = new SubCriteria() { Name = "65+", Percentage = 25, TenantId = "brussel" };

        panelDbContext.SubCriteria.AddRange(subCrit31, subCrit32, subCrit33, subCrit34, subCrit35, subCrit36);

        var crit31 = new Criteria()
        {
            Name = "Geslacht",
            SubCriteria = { subCrit31, subCrit32 },
            TenantId = "brussel"
        };
        var crit32 = new Criteria()
        {
            Name = "Leeftijd",
            SubCriteria = { subCrit33, subCrit34, subCrit35, subCrit36 },
            TenantId = "brussel"
        };
        panelDbContext.Criteria.AddRange(crit31, crit32);


        var panel3 = new Panel()
        {
            Name = "Panel Brussel",
            Description = "Dit is een panel om te zien wat de mensen over Brussel denken.",
            StartDate = new DateOnly(2025, 3, 1),
            EndDate = new DateOnly(2025, 8, 14),
            TenantId = "brussel",
            DrawStatus = DrawStatus.FirstPhaseActive,
            TotalNeededPanelmembers = 20

        };
        panelDbContext.Panels.Add(panel3);
        panel3.Criteria.Add(crit31);
        panel3.Criteria.Add(crit32);
        
        //criteria
        var subCrit1 = new SubCriteria() { Name = "Man", Percentage = 40, TenantId = "antwerpen" };
        var subCrit2 = new SubCriteria() { Name = "Vrouw", Percentage = 60, TenantId = "antwerpen" };
        var subCrit3 = new SubCriteria() { Name = "18-24", Percentage = 20, TenantId = "antwerpen" };
        var subCrit4 = new SubCriteria() { Name = "25-49", Percentage = 50, TenantId = "antwerpen" };
        var subCrit5 = new SubCriteria() { Name = "50-64", Percentage = 20, TenantId = "antwerpen" };
        var subCrit6 = new SubCriteria() { Name = "65+", Percentage = 10, TenantId = "antwerpen" };
        var subCrit7 = new SubCriteria() { Name = "Fiets", Percentage = 30, TenantId = "antwerpen" };
        var subCrit8 = new SubCriteria() { Name = "Auto", Percentage = 70, TenantId = "antwerpen" };
        var subCrit9 = new SubCriteria() { Name = "Hoog opgeleid", Percentage = 50, TenantId = "antwerpen" };
        var subCrit10 = new SubCriteria() { Name = "Laag opgeleid", Percentage = 50, TenantId = "antwerpen" };

        panelDbContext.SubCriteria.AddRange(subCrit1, subCrit2, subCrit3, subCrit4, subCrit5, subCrit6, subCrit7, subCrit8, subCrit9, subCrit10);

        var crit1 = new Criteria()
        {
            Name = "Geslacht",
            SubCriteria = { subCrit1, subCrit2 },
            TenantId = "antwerpen"
        };
        var crit2 = new Criteria()
        {
            Name = "Leeftijd",
            SubCriteria = { subCrit3, subCrit4, subCrit5, subCrit6 },
            TenantId = "antwerpen"
        };
        var crit3 = new Criteria()
        {
            Name = "Vervoer",
            SubCriteria = { subCrit7, subCrit8 },
            TenantId = "antwerpen"
        };
        var crit4 = new Criteria()
        {
            Name = "Opleiding",
            SubCriteria = { subCrit9, subCrit10 },
            TenantId = "antwerpen"
        };
        panelDbContext.Criteria.AddRange(crit1, crit2, crit3, crit4);

        panel1.Criteria.Add(crit1);
        panel1.Criteria.Add(crit2);
        panel1.Criteria.Add(crit3);
        panel1.Criteria.Add(crit4);

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
        //panel2.Members.Add(paul?.MemberProfile);
        paul?.MemberProfile.Panels.Add(panel1);
        //paul?.MemberProfile.Panels.Add(panel2);

        List<Invitation> selectedInvitations = new List<Invitation>()
        {
            new Invitation
            {
                Email = "kate@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "inv-kate",
                IsRegistered = true,
                IsDrawn = true,
                SelectedCriteria = new List<int> { 18, 19 }
            },
            new Invitation
            {
                Email = "rozie@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "inv-rozie",
                IsRegistered = true,
                IsDrawn = true,
                SelectedCriteria = new List<int> { 18, 19 }
            },
            new Invitation
            {
                Email = "rozalinda@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "inv-rozalinda",
                IsRegistered = true,
                IsDrawn = true,
                SelectedCriteria = new List<int> { 17, 19 }
            },
            new Invitation
            {
                Email = "alice2@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "inv-alice",
                IsRegistered = true,
                IsDrawn = true,
                SelectedCriteria = new List<int> { 17, 19 }
            },
            new Invitation
            {
                Email = "emma3@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "inv-emma",
                IsRegistered = true,
                IsDrawn = true,
                SelectedCriteria = new List<int> { 17, 19 }
            },
            new Invitation
            {
                Email = "emilie@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "inv-emilie",
                IsRegistered = true,
                IsDrawn = true,
                SelectedCriteria = new List<int> { 17, 19 }
            },
            new Invitation
            {
                Email = "Jurgen@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "inv-jurgen",
                IsRegistered = true,
                IsDrawn = true,
                SelectedCriteria = new List<int> { 17, 19 }
            },
            new Invitation
            {
                Email = "xander@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "inv-xander",
                IsRegistered = true,
                IsDrawn = true,
                SelectedCriteria = new List<int> { 17, 19 }
            },
            new Invitation
            {
                Email = "Lars@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 1,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "inv-lars",
                IsRegistered = true,
                IsDrawn = true,
                SelectedCriteria = new List<int> { 17, 19 }
            }
        };
        panelDbContext.AddRange(selectedInvitations);

        DrawResult drawResult = new DrawResult()
        {
            TenantId = "antwerpen",
            SelectedInvitations = selectedInvitations
        };
        panelDbContext.DrawResults.Add(drawResult);
        panel1.DrawResult = drawResult;

        var members = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Email = "kate@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Kate",
                    LastName = "Jansen",
                    Gender = Gender.Female,
                    BirthDate = new DateOnly(1960, 8, 14),
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria> { subCrit8, subCrit9 }
                }
            },
            new ApplicationUser
            {
                Email = "rozie@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Rozie",
                    LastName = "de Jong",
                    Gender = Gender.Female,
                    BirthDate = new DateOnly(1947, 2, 23),
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria> { subCrit8, subCrit9 }
                }
            },
            new ApplicationUser
            {
                Email = "rozalinda@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Rozalinda",
                    LastName = "van Loon",
                    Gender = Gender.Female,
                    BirthDate = new DateOnly(1955, 11, 5),
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria> { subCrit7, subCrit9 }
                }
            },
            new ApplicationUser
            {
                Email = "alice2@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Alice",
                    LastName = "Veerman",
                    Gender = Gender.Female,
                    BirthDate = new DateOnly(1951, 6, 30),
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria> { subCrit7, subCrit9 }
                }
            },
            new ApplicationUser
            {
                Email = "emma3@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Emma",
                    LastName = "de Boer",
                    Gender = Gender.Female,
                    BirthDate = new DateOnly(2001, 3, 18),
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria> { subCrit7, subCrit9 }
                }
            },
            new ApplicationUser
            {
                Email = "emilie@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Emilie",
                    Gender = Gender.Female,
                    BirthDate = new DateOnly(1987, 3, 18),
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria> { subCrit7, subCrit9 }
                }
            },
            new ApplicationUser
            {
                Email = "Jurgen@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Jurgen",
                    Gender = Gender.Male,
                    BirthDate = new DateOnly(2003, 4, 11),
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria> { subCrit7, subCrit9 }
                }
            },
            new ApplicationUser
            {
                Email = "xander@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Xander",
                    Gender = Gender.Male,
                    BirthDate = new DateOnly(2001, 6, 18),
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria> { subCrit7, subCrit9 }
                }
            },
            new ApplicationUser
            {
                Email = "Lars@example.com",
                UserType = UserType.Member,
                MemberProfile = new MemberProfile
                {
                    FirstName = "Lars",
                    Gender = Gender.Male,
                    BirthDate = new DateOnly(2002, 9, 27),
                    Town = "Antwerpen",
                    Panels = new List<Panel> { panel1 },
                    TenantId = "antwerpen",
                    SelectedCriteria = new List<SubCriteria> { subCrit7, subCrit9 }
                }
            }
        };


        var userVotesRec1 = new List<UserVote>
        {
            new UserVote
            {
                Voter = members[0],
                Recommended = true,
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
                Recommended = true,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            },
            new UserVote
            {
                Voter = members[5],
                Recommended = true,
                VotedAt = DateTime.UtcNow,
                TenantId = "antwerpen"
            },
            new UserVote
            {
                Voter = members[6],
                Recommended = true,
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
                Recommended = false,
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
        panelDbContext.Panels.Update(panel1);
        panelDbContext.SaveChanges();
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
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 10]),
            },

            // Panel 2 members
            new Invitation()
            {
                Email = "els@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "1wer-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 10]),
            },
            new Invitation()
            {
                Email = "bart@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "q1er-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "sophie2@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qw1r-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "tom@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwe1-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },

            // Additional members for Panel 1
            new Invitation()
            {
                Email = "jan@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-1yui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 10]),
            },
            new Invitation()
            {
                Email = "peter@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-t1ui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "david@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-ty1i-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "johan@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyu1-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "koen@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-1pas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "simon@example.com",
                Gender = Gender.Male,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-o1as-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 10]),
            },

            // Men 25-49
            new Invitation()
            {
                Email = "thomas@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-op1s-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "maarten@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opa1-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "jeroen@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-1fgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 10]),
            },
            new Invitation()
            {
                Email = "pieter@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-d1gh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "wouter@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-df1h-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "michel@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfg1-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "frank@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfgh-1klz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "dirk@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfgh-jkl1",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "hans@example.com",
                Gender = Gender.Male,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "2wer-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },


            // Men 50-64
            new Invitation()
            {
                Email = "marc@example.com",
                Gender = Gender.Male,
                Age = 50,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfgh-j1lz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "patrick@example.com",
                Gender = Gender.Male,
                Age = 50,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfgh-jk1z",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "erik@example.com",
                Gender = Gender.Male,
                Age = 50,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "q2er-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },


            // Men 65+
            new Invitation()
            {
                Email = "jozef@example.com",
                Gender = Gender.Male,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qw2r-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 10]),
            },
            new Invitation()
            {
                Email = "willem@example.com",
                Gender = Gender.Male,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwe2-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "gerard@example.com",
                Gender = Gender.Male,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-2yui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "robert@example.com",
                Gender = Gender.Male,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-t2ui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "hugo@example.com",
                Gender = Gender.Male,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-ty2i-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "albert@example.com",
                Gender = Gender.Male,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyu2-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },

            // Women 18-24
            new Invitation()
            {
                Email = "anna@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-2pas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 10]),
            },
            new Invitation()
            {
                Email = "lisa2@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-o2as-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 10]),
            },
            new Invitation()
            {
                Email = "emma2@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-op2s-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "sara@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opa2-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "laura@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-2fgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "nina@example.com",
                Gender = Gender.Female,
                Age = 18,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-d2gh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },

            // Women 25-49
            new Invitation()
            {
                Email = "eva@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-df2h-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "sophie@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfg2-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "julie@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfgh-2klz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "els2@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfgh-j2lz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 10]),
            },
            new Invitation()
            {
                Email = "lieve@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfgh-jk2z",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "anja@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opas-dfgh-jkl2",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "maria@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "3wer-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "martine@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwe3-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "hilde@example.com",
                Gender = Gender.Female,
                Age = 25,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-3yui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },

            // Women 50-64
            new Invitation()
            {
                Email = "ann@example.com",
                Gender = Gender.Female,
                Age = 50,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "q3er-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "ingrid@example.com",
                Gender = Gender.Female,
                Age = 50,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qw3r-tyui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "sonja@example.com",
                Gender = Gender.Female,
                Age = 50,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-t3ui-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },

            // Women 65+
            new Invitation()
            {
                Email = "helena@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-ty3i-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "godelieve@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyu3-opas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "rosa@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-3pas-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "margareta@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-o3as-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([8, 9]),
            },
            new Invitation()
            {
                Email = "mariette@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-op3s-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            },
            new Invitation()
            {
                Email = "alice@example.com",
                Gender = Gender.Female,
                Age = 65,
                Town = "Antwerpen",
                PanelId = 2,
                TenantId = "antwerpen",
                Batch = 1,
                Code = "qwer-tyui-opa3-dfgh-jklz",
                IsRegistered = true,
                SelectedCriteria = new List<int>([7, 9]),
            }
        };
        //invitations
        var invitation1 = new Invitation()
        {
            Email = "invi1@example.com",
            Age = 25,
            Code = "blLy-Tvxl-1TXG-nYg3-CBtD",
            Gender = Gender.Female,
            PanelId = 2,
            Town = "Antwerpen",
            TenantId = "antwerpen",
            Batch = 1
        };

        var invitation2 = new Invitation
        {
            Email = "invi2@example.com",
            Age = 18,
            Code = "tDK6-KFhD-ipIi-tQjz-m6cZ",
            Gender = Gender.Male,
            PanelId = 2,
            Town = "Antwerpen",
            TenantId = "antwerpen",
            Batch = 1
        };

        var invitation3 = new Invitation
        {
            Email = "invi3@example.com",
            Age = 25,
            Code = "Iga7-FHyw-yVfo-8wvN-Bxa8",
            Gender = Gender.Male,
            PanelId = 2,
            Town = "Antwerpen",
            TenantId = "antwerpen",
            Batch = 1
        };

        var invitation4 = new Invitation
        {
            Email = "invi4@example.com",
            Age = 25,
            Code = "FIcV-zyXs-reXO-I6dh-rMCB",
            Gender = Gender.Female,
            PanelId = 2,
            Town = "Antwerpen",
            TenantId = "antwerpen",
            Batch = 1
        };

        var invitation5 = new Invitation
        {
            Age = 25,
            Code = "HEMf-Xu0L-1ETh-urZJ-26s9",
            Gender = Gender.Male,
            PanelId = 2,
            Town = "Antwerpen",
            TenantId = "antwerpen",
            Batch = 1
        };
        panelDbContext.Invitations.AddRange(invitation1, invitation2, invitation3, invitation4, invitation5);
        panelDbContext.Invitations.AddRange(invitations);
        
        var invitationsBrussel = new List<Invitation>
        {
            new Invitation { Email = "luc1@brussel.com", Gender = Gender.Male, Age = 18, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "j4ks-xm3z-29qi-a1pl", IsRegistered = true, SelectedCriteria = new List<int> {7, 9} },
            new Invitation { Email = "tom1@brussel.com", Gender = Gender.Male, Age = 18, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "pt94-bvqe-r8c7-lz1m", IsRegistered = true, SelectedCriteria = new List<int> {8, 10} },
            new Invitation { Email = "tim1@brussel.com", Gender = Gender.Male, Age = 18, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "fw2g-qz8a-x7nu-5mtr", IsRegistered = true, SelectedCriteria = new List<int> {7, 10} },
            new Invitation { Email = "michael1@brussel.com", Gender = Gender.Male, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "vj9b-mx7n-z43e-wy0q", IsRegistered = true, SelectedCriteria = new List<int> {7, 9} },
            new Invitation { Email = "kevin1@brussel.com", Gender = Gender.Male, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "ue7a-ck3z-lp01-vgmn", IsRegistered = true, SelectedCriteria = new List<int> {8, 10} },
            new Invitation { Email = "peter1@brussel.com", Gender = Gender.Male, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "k21n-bxyr-d9t7-fg8z", IsRegistered = true, SelectedCriteria = new List<int> {8, 9} },
            new Invitation { Email = "mark1@brussel.com", Gender = Gender.Male, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "m8rt-wz34-yfpa-nxq7", IsRegistered = true, SelectedCriteria = new List<int> {7, 9} },
            new Invitation { Email = "frank1@brussel.com", Gender = Gender.Male, Age = 50, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "d7f0-vqaz-mnp3-k5wl", IsRegistered = true, SelectedCriteria = new List<int> {7, 9} },
            new Invitation { Email = "henk1@brussel.com", Gender = Gender.Male, Age = 50, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "2hrq-pxem-a1gz-7ydn", IsRegistered = true, SelectedCriteria = new List<int> {8, 10} },
            new Invitation { Email = "jan1@brussel.com", Gender = Gender.Male, Age = 65, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "y5nv-jl07-xkmq-8azr", IsRegistered = true, SelectedCriteria = new List<int> {7, 10} },

            new Invitation { Email = "anna1@brussel.com", Gender = Gender.Female, Age = 18, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "gpe7-w29v-dxnl-h3fq", IsRegistered = true, SelectedCriteria = new List<int> {8, 10} },
            new Invitation { Email = "sophie1@brussel.com", Gender = Gender.Female, Age = 18, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "trm4-bpvy-x06z-nlqe", IsRegistered = true, SelectedCriteria = new List<int> {8, 9} },
            new Invitation { Email = "emma1@brussel.com", Gender = Gender.Female, Age = 18, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "zqaf-rxj3-0mpu-v4tw", IsRegistered = true, SelectedCriteria = new List<int> {7, 9} },
            new Invitation { Email = "julie1@brussel.com", Gender = Gender.Female, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "a1zu-nvm4-3qxf-2ybt", IsRegistered = true, SelectedCriteria = new List<int> {8, 10} },
            new Invitation { Email = "els1@brussel.com", Gender = Gender.Female, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "v42k-jg5y-mzpn-hl0r", IsRegistered = true, SelectedCriteria = new List<int> {7, 10} },
            new Invitation { Email = "sara1@brussel.com", Gender = Gender.Female, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "c9mp-ax07-vgze-3rqn", IsRegistered = true, SelectedCriteria = new List<int> {8, 9} },
            new Invitation { Email = "hilde1@brussel.com", Gender = Gender.Female, Age = 50, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "ltbn-50ey-q9uw-kxvm", IsRegistered = true, SelectedCriteria = new List<int> {8, 9} },
            new Invitation { Email = "maria1@brussel.com", Gender = Gender.Female, Age = 50, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "h20z-lbqr-mvxc-7fna", IsRegistered = true, SelectedCriteria = new List<int> {7, 9} },
            new Invitation { Email = "an1@brussel.com", Gender = Gender.Female, Age = 65, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "pxra-f3mn-zuqv-y90l", IsRegistered = true, SelectedCriteria = new List<int> {8, 9} },
            new Invitation { Email = "godelieve1@brussel.com", Gender = Gender.Female, Age = 65, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "wz4m-nk5r-18yx-qlva", IsRegistered = true, SelectedCriteria = new List<int> {7, 10} },

            // Extra gender-mixed entries
            new Invitation { Email = "kim1@brussel.com", Gender = Gender.Female, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "8nfj-qmxp-ztrc-40bw", IsRegistered = true, SelectedCriteria = new List<int> {8, 9} },
            new Invitation { Email = "alex1@brussel.com", Gender = Gender.Male, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "om2v-xe4z-fqnl-1yct", IsRegistered = true, SelectedCriteria = new List<int> {7, 9} },
            new Invitation { Email = "joke1@brussel.com", Gender = Gender.Female, Age = 25, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "vylg-72rm-k98n-jzqx", IsRegistered = true, SelectedCriteria = new List<int> {7, 10} },
            new Invitation { Email = "steven1@brussel.com", Gender = Gender.Male, Age = 50, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "xzvp-6a3q-wgfy-nm21", IsRegistered = true, SelectedCriteria = new List<int> {8, 10} },
            new Invitation { Email = "nathalie1@brussel.com", Gender = Gender.Female, Age = 50, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "edyq-14vm-zlcr-b78n", IsRegistered = true, SelectedCriteria = new List<int> {7, 9} },
            new Invitation { Email = "karen1@brussel.com", Gender = Gender.Female, Age = 50, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "0anx-rg2j-pzvy-mfek", IsRegistered = true, SelectedCriteria = new List<int> {8, 9} },
            new Invitation { Email = "bert1@brussel.com", Gender = Gender.Male, Age = 65, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "hy94-nqxb-vl1o-2mzt", IsRegistered = true, SelectedCriteria = new List<int> {7, 10} },
            new Invitation { Email = "wim1@brussel.com", Gender = Gender.Male, Age = 65, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "qtmr-7g0v-cfxa-nlzp", IsRegistered = true, SelectedCriteria = new List<int> {8, 10} },
            new Invitation { Email = "nicole1@brussel.com", Gender = Gender.Female, Age = 65, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "l2eb-zmk3-qw7n-vrpa", IsRegistered = true, SelectedCriteria = new List<int> {8, 9} },
            new Invitation { Email = "edith1@brussel.com", Gender = Gender.Female, Age = 65, Town = "brussel", PanelId = 3, TenantId = "brussel", Batch = 1, Code = "btxl-5dnr-qyom-7czg", IsRegistered = true, SelectedCriteria = new List<int> {7, 9} },
        };

        panelDbContext.Invitations.AddRange(invitationsBrussel);
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