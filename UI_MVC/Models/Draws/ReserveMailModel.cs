using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models.Draws;

public class ReserveMailModel
{
    [Required]
    public string Subject { get; set; } = "U bent geselecteerd voor het burgerpanel";
    [Required]
    public string Message { get; set; } = @"Beste heer/mevrouw,

Goed nieuws! U bent geselecteerd om deel te nemen aan het burgerpanel.
Wij zijn verheugd u te kunnen meedelen dat u, uit alle aanmeldingen, bent geselecteerd om deel te nemen aan het burgerpanel waarvoor u zich heeft aangemeld. Uw deelname is van grote waarde voor dit democratische proces waarin burgers directe invloed kunnen uitoefenen op beleidsbeslissingen.

Wat kunt u verwachten? 
Tijdens de panelbijeenkomsten gaat u samen met andere geselecteerde burgers in gesprek. U krijgt informatie van experts, kunt vragen stellen en discussiëren met medepanelleden. Uiteindelijk werkt u samen aan aanbevelingen voor de organisatie.

Om uw deelname te bevestigen en uw account te activeren, vragen wij u in te loggen via onderstaande link met de code waarmee u ook heeft geregistreerd:

<a href='https://localhost:7145/Identity/Account/Login' style='color: #080708; text-decoration: underline;'>https://localhost:7145/Identity/Account/Login</a>";

    public string SelectedInvitationIds { get; set; }
    public int PanelId { get; set; }
}