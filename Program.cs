using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Weather API - .NET 10")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithSidebar(true)
            .WithModels(true)
            .WithDownloadButton(true)
            .WithTestRequestButton(true);
    });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithSummary("Obtiene el pronóstico del clima")
.WithDescription("Retorna una lista de 5 días con pronóstico del clima incluyendo temperatura y descripción")
.WithTags("Weather")
.WithOpenApi();

// Endpoint adicional para demostrar más funcionalidad
app.MapGet("/weatherforecast/{days:int}", (int days) =>
{
    if (days <= 0 || days > 30)
        return Results.BadRequest("Los días deben estar entre 1 y 30");
        
    var forecast = Enumerable.Range(1, days).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    
    return Results.Ok(forecast);
})
.WithName("GetWeatherForecastByDays")
.WithSummary("Obtiene pronóstico por cantidad de días")
.WithDescription("Retorna pronóstico del clima para un número específico de días (1-30)")
.WithTags("Weather")
.WithOpenApi();

// Endpoint para obtener el clima de hoy
app.MapGet("/weatherforecast/today", () =>
{
    var todayWeather = new WeatherForecast
    (
        DateOnly.FromDateTime(DateTime.Now),
        Random.Shared.Next(-20, 55),
        summaries[Random.Shared.Next(summaries.Length)]
    );
    
    return Results.Ok(todayWeather);
})
.WithName("GetTodayWeather")
.WithSummary("Obtiene el clima de hoy")
.WithDescription("Retorna el pronóstico del clima para el día actual")
.WithTags("Weather")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}