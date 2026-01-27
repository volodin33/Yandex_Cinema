using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace proxy;

public class RouteSelector(IOptions<ProxyOptions> config)
{
    private static readonly ConcurrentDictionary<string, long> RequestCounters = new();
    private const int WindowSize = 10;

    public string? GetRoute(HttpContext ctx)
    {
        ProxyRouteConfig? routeCfg = null;
        var path = ctx.Request.Path;
        PathString tail = path;
        
        foreach (var route in config.Value.Routes)
        {
            if (route.PathPrefix == "/")
            {
                routeCfg = route;
            }

            if (path.StartsWithSegments(route.PathPrefix, out var t))
            {
                tail = t;
                routeCfg = route;
                break;
            }
        }

        if (routeCfg is null)
            return null;

        var targetRoute = routeCfg.MigrationUrl;
        
        if (routeCfg.MigrationPercent <= 0)
        {
            targetRoute = routeCfg.OldUrl;
        }
        else if (routeCfg.MigrationPercent < 100)
        {
            var n = RequestCounters.AddOrUpdate(routeCfg.PathPrefix, 1L, (_, cur) => cur + 1);
            var requestSlot = (int) ((n - 1) % WindowSize);
            var oldUrlSlot = routeCfg.MigrationPercent * WindowSize / 100;
            targetRoute = requestSlot < oldUrlSlot ? routeCfg.MigrationUrl : routeCfg.OldUrl;
        }

        return BuildUrl(targetRoute, tail, ctx.Request.QueryString);
    }
    
    private static string BuildUrl(string targetRoute, PathString tail, QueryString query)
    {
        var baseUri = new Uri(targetRoute);                 
        var basePath = new PathString(baseUri.AbsolutePath);
        return $"{baseUri.Scheme}://{baseUri.Authority}{basePath.Add(tail).Add(query)}";
    }
}