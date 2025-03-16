using CitizenPanel.BL.Domain.Recruitment;

namespace CitizenPanel.DAL;

public class DataSeeder
{
    public static void Seed(PanelDbContext context)
    {
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
        context.SubCriteria.AddRange(subCrit1, subCrit2, subCrit3, subCrit4);

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
        context.ExtraCriteria.AddRange(extraCrit1, extraCrit2);
        
        context.SaveChanges();
        context.ChangeTracker.Clear();
    }
}