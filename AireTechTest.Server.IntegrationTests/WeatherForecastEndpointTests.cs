using System.Net;
using System.Net.Http.Json;

namespace AireTechTest.Server.IntegrationTests;

public class WeatherForecastEndpointTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [Before(Test)]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [After(Test)]
    public async Task Cleanup()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Test]
    public async Task GetWeatherForecast_ReturnsSuccessStatusCode()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/weatherforecast");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetWeatherForecast_ReturnsJsonContent()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/weatherforecast");

        // Assert
        await Assert.That(response.Content.Headers.ContentType?.MediaType).IsEqualTo("application/json");
    }

    [Test]
    public async Task GetWeatherForecast_ReturnsFiveForecasts()
    {
        // Act
        WeatherForecast[]? forecasts = await _client.GetFromJsonAsync<WeatherForecast[]>("/api/weatherforecast");

        // Assert
        await Assert.That(forecasts).IsNotNull();
        await Assert.That(forecasts!.Length).IsEqualTo(5);
    }

    [Test]
    public async Task GetWeatherForecast_ForecastsHaveValidData()
    {
        // Act
        WeatherForecast[]? forecasts = await _client.GetFromJsonAsync<WeatherForecast[]>("/api/weatherforecast");

        // Assert
        await Assert.That(forecasts).IsNotNull();
        foreach (WeatherForecast forecast in forecasts!)
        {
            await Assert.That(forecast.Date).IsNotEqualTo(default(DateOnly));
            await Assert.That(forecast.TemperatureC).IsBetween(-20, 55);
            await Assert.That(forecast.Summary).IsNotNull();
        }
    }
}