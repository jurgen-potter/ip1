namespace CitizenPanel.UI.MVC.Middleware;

public class TenantStore
{
    public HashSet<string> TenantIds { get; } = new();
}