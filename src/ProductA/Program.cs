using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string productName = "Hell i am ProductA and  i  am live";
string productConfig = Environment.GetEnvironmentVariable("PRODUCT_CONFIG") ?? "{}";

app.MapGet("/info", () => Results.Ok(new
{
    product = productName,
    config = JsonSerializer.Deserialize<object>(productConfig)
}));

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
