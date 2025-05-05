namespace CitizenPanel.BL.Domain.Users;

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