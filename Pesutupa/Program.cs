using Microsoft.AspNetCore.HostFiltering;
using Pesutupa;
using Pesutupa.Models;
using System.Net;
using System.Text.RegularExpressions;

var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Regex[] whitelistRegexes;
ConvertWhitelistToRegexes(conf);

app.UseDefaultFiles();
app.UseStaticFiles();

if (!string.IsNullOrEmpty(conf["CertFilePath"]))
{
    Console.WriteLine("Using HTTPS redirection");
    app.UseHttpsRedirection();
}

app.MapPost("/rsvp", ctx => IfWhitelistAllowsRequest(ctx, Rsvp.ProcessPostRsvp));

app.Run();

#region Whitelist stuff

async Task<IResult> IfWhitelistAllowsRequest(HttpContext ctx, Func<HttpContext, Task<IResult>> callIfWhitelistAllows)
{
    return await WhitelistAllowsRequest(ctx)
        ? await callIfWhitelistAllows(ctx)
        : Results.Unauthorized();
}

async Task<bool> WhitelistAllowsRequest(HttpContext ctx)
{
    var hostEntry = await Dns.GetHostEntryAsync(ctx.Request.Host.Host);
    return whitelistRegexes.Any(regex => regex.IsMatch(hostEntry.HostName));
}

static string WildCardToRegExStr(string value) => "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";

void ConvertWhitelistToRegexes(IConfiguration conf)
{
    var children = conf.GetChildren();

    var json = conf.GetSection("Whitelist").Value;
    
    if (string.IsNullOrEmpty(json))
    {
        throw new Exception(ConstantStrings.WhitelistMustExistAndHaveValues);
    };

    var wildcardEntries = json.Split(',');

    if (wildcardEntries == null)
    {
        throw new Exception(ConstantStrings.WhitelistMustHaveValues);
    }

    if (!(wildcardEntries?.Any() ?? false))
    {
        throw new Exception(ConstantStrings.WhitelistMustHaveValues);
    }

    whitelistRegexes = wildcardEntries.Select(entry => new Regex(WildCardToRegExStr(entry))).ToArray();
}

#endregion