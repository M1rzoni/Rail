using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("*")
              .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
              .WithHeaders("Content-Type", "Authorization")
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();

// Get port from Railway environment variable or use default
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

app.MapGet("/", () =>
{
    var mediaItems = new object[]
    {
        new { type = "video", url = "https://www.shutterstock.com/shutterstock/videos/1085118248/preview/stock-footage-ice-cubes-and-coke-poured-into-glass.mp4" },
        new { type = "video", url = "https://www.shutterstock.com/shutterstock/videos/1051047043/preview/stock-footage-moscow-russia-cinematic-footage-of-coca-cola-glass-bottle-classic-rotate-on-gray.mp4" },
        new { type = "image", url = "https://adminbackend-production-d76a.up.railway.app/data/uploads/catalogue-86e82066-d735-4454-a16e-817b5447f7d7-1.jpg", durationInSeconds = 5 },
        new { type = "image", url = "https://adminbackend-production-d76a.up.railway.app/data/uploads/catalogue-86e82066-d735-4454-a16e-817b5447f7d7-2.jpg", durationInSeconds = 5 },
        new { type = "image", url = "https://adminbackend-production-d76a.up.railway.app/data/uploads/catalogue-86e82066-d735-4454-a16e-817b5447f7d7-3.jpg", durationInSeconds = 5 },
        new { type = "video", url = "https://videos.pexels.com/video-files/6092267/6092267-hd_1080_1920_30fps.mp4" }
    };

    return Results.Ok(mediaItems);
});

app.MapPost("/item", (ItemRequest request) =>
{
    var itemsData = new[]
    {
        new { ArSif = "5290047000940", ArNa1 = "Sok 330ML Pepsi Max", ArJmj = "KOM", MPC = 0.9 },
        new { ArSif = "8713439241518", ArNa1 = "Primo Keyboard Spill-Resistant", ArJmj = "KOM", MPC = 30.0 },
        new { ArSif = "3858881088306", ArNa1 = "Caj Kamilica & Đumbir", ArJmj = "KOM", MPC = 2.8 },
        new { ArSif = "5397184621165", ArNa1 = "Dell Wireless Keyboard and Mou", ArJmj = "KOM", MPC = 70 },
        new { ArSif = "3877002204067", ArNa1 = "Rukavice Lateks Bez Pudera", ArJmj = "KOM", MPC = 13.7 }
    };

    var itemData = itemsData.FirstOrDefault(item => item.ArSif == request.SifraArtikla);
    return Results.Ok(itemData);
});

app.Run();

public record ItemRequest(string SifraArtikla);