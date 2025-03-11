namespace CitizenPanel.BL.Domain.User;

public class Panelmember : User
{
    public string Email { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
}