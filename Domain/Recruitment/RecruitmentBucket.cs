using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL.Domain.Recruitment;

public class RecruitmentBucket
{
    public Age Age { get; set; }
    public Gender Gender { get; set; }
    public int Signups { get; set; } // Aantal aanmeldingen
}       