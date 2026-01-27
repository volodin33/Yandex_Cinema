using System.Collections.Concurrent;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.Extensions.Options;

namespace proxy;

public class Proxy(IHttpClientFactory clientFactory, RouteSelector routeSelector, ILogger<Proxy> logger)
{
    public async Task HandleAsync(HttpContext ctx)
    {
        using var response = await SendRequest(ctx);
        await SendResponse(ctx, response);
    }

    private async Task<HttpResponseMessage?> SendRequest(HttpContext ctx)
    {
        var client = clientFactory.CreateClient("proxy");
        var targetUri = routeSelector.GetRoute(ctx);
        
        if (targetUri is null)
        {
            return null;
        }
        
        logger.LogInformation($"Target request: {targetUri}");

        using var req = new HttpRequestMessage(new HttpMethod(ctx.Request.Method), targetUri);

        if (ctx.Request.ContentLength > 0)
        {
            req.Content = new StreamContent(ctx.Request.Body);
            if (ctx.Request.ContentType != null)
            {
                req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(ctx.Request.ContentType);
            }
        }

        return await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ctx.RequestAborted);
    }

    private async Task SendResponse(HttpContext ctx,HttpResponseMessage? response)
    {
        if (response is null)
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }
        
        ctx.Response.StatusCode = (int) response.StatusCode;

        if (response.Content.Headers.ContentType is not null)
        {
            ctx.Response.ContentType = response.Content.Headers.ContentType.ToString();
        }

        await response.Content.CopyToAsync(ctx.Response.Body, ctx.RequestAborted);
    }
}