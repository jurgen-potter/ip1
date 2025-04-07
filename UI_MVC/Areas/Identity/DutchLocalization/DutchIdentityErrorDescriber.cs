using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.UI.MVC.Areas.Identity.DutchLocalization;

public class DutchIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError()
        => new IdentityError { Code = nameof(DefaultError), Description = "Er is een onbekende fout opgetreden." };
    
    public override IdentityError PasswordMismatch()
        => new IdentityError { Code = nameof(PasswordMismatch), Description = "Onjuist wachtwoord." };
    
    public override IdentityError PasswordRequiresDigit()
        => new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Wachtwoord moet minstens een cijfer bevatten." };

    public override IdentityError PasswordRequiresLower()
        => new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Wachtwoord moet minstens een kleine letter bevatten." };

    public override IdentityError PasswordRequiresUpper()
        => new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Wachtwoord moet minstens een hoofdletter bevatten." };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Wachtwoord moet minstens een speciaal teken bevatten." };

    public override IdentityError PasswordTooShort(int length)
        => new IdentityError { Code = nameof(PasswordTooShort), Description = $"Wachtwoord moet minstens {length} karakters bevatten." };

    public override IdentityError DuplicateEmail(string email)
        => new IdentityError() { Code = nameof(DuplicateEmail), Description = $"Het e-mailadres '{email}' is al in gebruik." };
    
    public override IdentityError ConcurrencyFailure()
        => new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Concurrency-fout: het record is gewijzigd door een andere gebruiker." };
    
    public override IdentityError InvalidToken()
        => new IdentityError { Code = nameof(InvalidToken), Description = "Ongeldige token." };
}
