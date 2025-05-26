using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models.Draws;

public class FinalDrawViewModel
{
    public int PanelId { get; set; }

    [Required]
    public string SelectedSubject { get; set; } = "U bent geselecteerd voor het burgerpanel";

    [Required]
    public string SelectedMessage { get; set; } = @"Beste heer/mevrouw,

Goed nieuws! U bent geselecteerd om deel te nemen aan het burgerpanel.
Wij zijn verheugd u te kunnen meedelen dat u, uit alle aanmeldingen, bent geselecteerd om deel te nemen aan het burgerpanel waarvoor u zich heeft aangemeld. Uw deelname is van grote waarde voor dit democratische proces waarin burgers directe invloed kunnen uitoefenen op beleidsbeslissingen.

Wat kunt u verwachten? 
Tijdens de panelbijeenkomsten gaat u samen met andere geselecteerde burgers in gesprek. U krijgt informatie van experts, kunt vragen stellen en discussiëren met medepanelleden. Uiteindelijk werkt u samen aan aanbevelingen voor de organisatie.";
    
    [Required]
    public string NotSelectedSubject { get; set; } = "U bent niet geselecteerd voor het burgerpanel";

    [Required]
    public string NotSelectedMessage { get; set; } = @"Beste heer/mevrouw,

Hartelijk dank voor uw interesse in het burgerpanel.

Wij hebben een overweldigend aantal aanmeldingen ontvangen voor deelname aan dit burgerpanel. Via een zorgvuldig selectieproces, waarbij we streven naar een representatieve afspiegeling van de samenleving, is de definitieve samenstelling van het panel bepaald.

Helaas moeten wij u meedelen dat u niet bent geselecteerd voor deelname aan dit burgerpanel. Dit betekent niet dat uw inbreng niet waardevol zou zijn, maar is het resultaat van het streven naar een diverse samenstelling op basis van factoren zoals leeftijd, gender, woonplaats en achtergrond.

Wij willen u hartelijk bedanken voor uw bereidheid om bij te dragen aan dit democratische proces en hopen in de toekomst mogelijk alsnog een beroep op u te mogen doen. U kunt de voortgang van het panel terug vinden op de project pagina op de website.";
}