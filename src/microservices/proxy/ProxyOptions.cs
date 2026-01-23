namespace proxy;

public class ProxyOptions
{
    public List<ProxyRouteConfig> Routes { get; set; } = new();
}

public class ProxyRouteConfig
{
    public string PathPrefix { get; set; } = "";
    public int MigrationPercent { get; set; } = 0;
    public string MigrationUrl { get; set; }
    public string OldUrl { get; set; }
}