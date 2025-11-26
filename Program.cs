using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

// Simple GET endpoint
app.MapGet("/", () =>
{
    return new[]
    {
        new
        {
            type = "video",
            url = "https://www.shutterstock.com/shutterstock/videos/1085118248/preview/stock-footage-ice-cubes-and-coke-poured-into-glass.mp4"
        },
        new
        {
            type = "video",
            url = "https://www.shutterstock.com/shutterstock/videos/1051047043/preview/stock-footage-moscow-russia-cinematic-footage-of-coca-cola-glass-bottle-classic-rotate-on-gray.mp4"
        }
    };
});

// Simple POST endpoint
app.MapPost("/item", (ItemRequest request) =>
{
    var items = new[]
    {
        new { ArSif = "5290047000940", ArNa1 = "Sok 330ML Pepsi Max", Price = 0.9 },
        new { ArSif = "8713439241518", ArNa1 = "Primo Keyboard Spill-Resistant", Price = 30.0 }
    };

    var item = items.FirstOrDefault(i => i.ArSif == request.SifraArtikla);
    return Results.Ok(item);
});

app.Run();

public record ItemRequest(string SifraArtikla);