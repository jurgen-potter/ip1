namespace CitizenPanel.BL.Domain.User;

public enum Gender : byte
{
    Male = 0,
    Female
}

public static class GenderExtensions
{
    public static string ToDutch(this Gender gender)
    {
        return gender switch
        {
            Gender.Male => "Man",
            Gender.Female => "Vrouw",
            _ => "Onbekend"
        };
    }
}