using Microsoft.AspNetCore.Cors;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

// GET endpoint
app.MapGet("/", () =>
{
    return new[]
    {
        new
        {
            type = "video",
            url = "https://www.shutterstock.com/shutterstock/videos/1085118248/preview/stock-footage-ice-cubes-and-coke-poured-into-glass.mp4"
        }
    };
});

// POST endpoint za artikle
app.MapPost("/item", (ItemRequest request) =>
{
    // Baza artikala
    var items = new[]
    {
        new { ArSif = "5290047000940", ArNa1 = "Sok 330ML Pepsi Max", Price = 0.9 },
        new { ArSif = "8713439241518", ArNa1 = "Primo Keyboard Spill-Resistant", Price = 30.0 }
    };

    // Očisti barcode - ukloni razmake i nove redove
    string cleanedBarcode = request.SifraArtikla?.Trim() ?? "";

    Console.WriteLine($"Primljen barcode: '{request.SifraArtikla}'");
    Console.WriteLine($"Očišćen barcode: '{cleanedBarcode}'");
    Console.WriteLine($"Dužina: {cleanedBarcode.Length}");

    // Pretraga
    var item = items.FirstOrDefault(i => i.ArSif == cleanedBarcode);

    if (item == null)
    {
        Console.WriteLine($"Artikal NOT FOUND za: {cleanedBarcode}");
        return Results.NotFound(new
        {
            message = "Artikal nije pronađen",
            searchedBarcode = cleanedBarcode,
            availableBarcodes = items.Select(x => x.ArSif).ToArray()
        });
    }

    Console.WriteLine($"Artikal pronađen: {item.ArNa1}");
    return Results.Ok(item);
});

// Debug endpoint - vidi sve dostupne artikle
app.MapGet("/items", () =>
{
    var items = new[]
    {
        new { ArSif = "5290047000940", ArNa1 = "Sok 330ML Pepsi Max", Price = 0.9 },
        new { ArSif = "8713439241518", ArNa1 = "Primo Keyboard Spill-Resistant", Price = 30.0 }
    };
    return items;
});

app.Run();

public record ItemRequest(string SifraArtikla);