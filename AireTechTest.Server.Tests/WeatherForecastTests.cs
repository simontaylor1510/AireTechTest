namespace AireTechTest.Server.Tests;

public class WeatherForecastTests
{
    [Test]
    public async Task WeatherForecast_TemperatureF_CalculatesCorrectly()
    {
        // Arrange
        WeatherForecast forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 0, "Cold");

        // Act
        int temperatureF = forecast.TemperatureF;

        // Assert
        await Assert.That(temperatureF).IsEqualTo(32);
    }

    [Test]
    public async Task WeatherForecast_TemperatureF_CalculatesCorrectlyForPositiveTemp()
    {
        // Arrange
        WeatherForecast forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 100, "Hot");

        // Act
        int temperatureF = forecast.TemperatureF;

        // Assert - Using the formula: 32 + (int)(100 / 0.5556) = 32 + 179 = 211
        await Assert.That(temperatureF).IsEqualTo(211);
    }

    [Test]
    public async Task WeatherForecast_TemperatureF_CalculatesCorrectlyForNegativeTemp()
    {
        // Arrange
        WeatherForecast forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), -40, "Freezing");

        // Act
        int temperatureF = forecast.TemperatureF;

        // Assert - Using the formula: 32 + (int)(-40 / 0.5556) = 32 + (-71) = -39
        await Assert.That(temperatureF).IsEqualTo(-39);
    }

    [Test]
    public async Task WeatherForecast_HasCorrectProperties()
    {
        // Arrange
        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        int tempC = 25;
        string summary = "Warm";

        // Act
        WeatherForecast forecast = new WeatherForecast(date, tempC, summary);

        // Assert
        await Assert.That(forecast.Date).IsEqualTo(date);
        await Assert.That(forecast.TemperatureC).IsEqualTo(tempC);
        await Assert.That(forecast.Summary).IsEqualTo(summary);
    }
}