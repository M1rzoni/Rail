using Microsoft.AspNetCore.Cors;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

// Konfiguriši JSON serializaciju da bude case-insensitive
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

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
app.MapPost("/item", async (HttpContext context) =>
{
    try
    {
        // Čitaj raw body kao string
        string body;
        using (var reader = new StreamReader(context.Request.Body))
        {
            body = await reader.ReadToEndAsync();
        }

        Console.WriteLine($"Raw Body: {body}");

        // Deserijalizuj JSON
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var request = JsonSerializer.Deserialize<ItemRequest>(body, jsonOptions);
        Console.WriteLine($"Parsed SifraArtikla: '{request?.SifraArtikla}'");

        if (request?.SifraArtikla == null)
        {
            Console.WriteLine("SifraArtikla je NULL!");
            return Results.BadRequest(new { message = "SifraArtikla je obavezno polje" });
        }

        // Baza artikala
        var items = new object[]
        {
            new { ArSif = "5290047000940", ArNa1 = "Sok 330ML Pepsi Max", MPC = 0.9 },
            new { ArSif = "8713439241518", ArNa1 = "Primo Keyboard Spill-Resistant", MPC = 30.0 },
            new { ArSif = "3858881088306", ArNa1 = "Caj Kamilica & Đumbir", MPC = 2.8 },
            new { ArSif = "5397184621165", ArNa1 = "Dell Wireless Keyboard and Mou", MPC = 70 },
            new { ArSif = "3877002204067", ArNa1 = "Rukavice Lateks Bez Pudera", MPC = 13.7 }
        };

        // Očisti barcode
        string cleanedBarcode = request.SifraArtikla.Trim();
        Console.WriteLine($"Tražim: '{cleanedBarcode}'");

        // Pretraga
        var item = items.FirstOrDefault(i =>
        {
            var prop = i.GetType().GetProperty("ArSif");
            return prop?.GetValue(i)?.ToString() == cleanedBarcode;
        });

        if (item == null)
        {
            Console.WriteLine($"Artikal NOT FOUND za: {cleanedBarcode}");
            return Results.Json(null);
        }

        Console.WriteLine($"✓ Artikal pronađen");
        return Results.Ok(item);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ ERROR: {ex.Message}");
        Console.WriteLine($"Stack: {ex.StackTrace}");
        return Results.Json(null);
    }
});

// Debug endpoint
app.MapGet("/items", () =>
{
    return new object[]
    {
        new { ArSif = "5290047000940", ArNa1 = "Sok 330ML Pepsi Max", MPC = 0.9 },
        new { ArSif = "8713439241518", ArNa1 = "Primo Keyboard Spill-Resistant", MPC = 30.0 }
    };
});

app.Run();

public record ItemRequest(string SifraArtikla);