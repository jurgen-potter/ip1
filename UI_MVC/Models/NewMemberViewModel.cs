using CitizenPanel.BL.Domain.Recruitment;
using CitizenPanel.BL.Domain.User;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class NewMemberViewModel
{
    [Required(ErrorMessage = "Voornaam is verplicht")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Achternaam is verplicht")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Email adres is verplicht")]
    [EmailAddress(ErrorMessage = "Email adres moet geldig zijn")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Wachtwoord is verplicht")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Wachtwoordbevestiging is verplicht")]
    [Compare("Password", ErrorMessage = "Het wachtwoord en de bevestiging komen niet overeen")]
    public string ConfirmPassword { get; set; }
    
    public Gender Gender { get; set; }
    
    public DateOnly BirthDate { get; set; }
    
    public string Town { get; set; }
    
    public List<SubCriteria> SelectedCriteria { get; set; }
}