using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQDemo.Controllers;

namespace RabbitMQDemo.Tests.Controllers;

public class WeatherForecastControllerTests
{
    private static readonly string[] KnownSummaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private static WeatherForecastController CreateController()
    {
        var logger = NullLogger<WeatherForecastController>.Instance;
        return new WeatherForecastController(logger);
    }

    [Fact]
    public void Get_ReturnsExactlyFiveItems()
    {
        var controller = CreateController();

        var result = controller.Get().ToList();

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Get_ReturnsNonNullResult()
    {
        var controller = CreateController();

        var result = controller.Get();

        Assert.NotNull(result);
    }

    [Fact]
    public void Get_AllItemsHaveNonNullSummary()
    {
        var controller = CreateController();

        var result = controller.Get().ToList();

        Assert.All(result, item => Assert.NotNull(item.Summary));
    }

    [Fact]
    public void Get_AllSummariesAreFromKnownList()
    {
        var controller = CreateController();

        var result = controller.Get().ToList();

        Assert.All(result, item =>
            Assert.Contains(item.Summary, KnownSummaries));
    }

    [Fact]
    public void Get_TemperatureCIsWithinExpectedRange()
    {
        var controller = CreateController();

        // Run multiple times to exercise the random range
        for (int i = 0; i < 20; i++)
        {
            var result = controller.Get().ToList();

            Assert.All(result, item =>
            {
                Assert.True(item.TemperatureC >= -20,
                    $"TemperatureC {item.TemperatureC} is below minimum -20");
                Assert.True(item.TemperatureC < 55,
                    $"TemperatureC {item.TemperatureC} is at or above maximum 55");
            });
        }
    }

    [Fact]
    public void Get_DatesAreInTheFuture()
    {
        var controller = CreateController();
        var today = DateOnly.FromDateTime(DateTime.Now);

        var result = controller.Get().ToList();

        Assert.All(result, item =>
            Assert.True(item.Date > today,
                $"Date {item.Date} is not in the future (today is {today})"));
    }

    [Fact]
    public void Get_DatesAreConsecutiveFutureDays()
    {
        var controller = CreateController();
        var today = DateOnly.FromDateTime(DateTime.Now);

        var result = controller.Get().ToList();

        for (int i = 0; i < result.Count; i++)
        {
            var expectedDate = today.AddDays(i + 1);
            Assert.Equal(expectedDate, result[i].Date);
        }
    }

    [Fact]
    public void Get_DatesSpanFiveDaysStartingTomorrow()
    {
        var controller = CreateController();
        var tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var fiveDaysFromNow = DateOnly.FromDateTime(DateTime.Now.AddDays(5));

        var result = controller.Get().ToList();

        Assert.Equal(tomorrow, result.First().Date);
        Assert.Equal(fiveDaysFromNow, result.Last().Date);
    }

    [Fact]
    public void Get_TemperatureFConversionIsCorrect()
    {
        var controller = CreateController();

        var result = controller.Get().ToList();

        Assert.All(result, item =>
        {
            int expectedFahrenheit = 32 + (int)(item.TemperatureC / 0.5556);
            Assert.Equal(expectedFahrenheit, item.TemperatureF);
        });
    }

    [Fact]
    public void Get_ReturnsWeatherForecastObjects()
    {
        var controller = CreateController();

        var result = controller.Get().ToList();

        Assert.All(result, item => Assert.IsType<WeatherForecast>(item));
    }

    [Fact]
    public void Get_NoDuplicateDates()
    {
        var controller = CreateController();

        var result = controller.Get().ToList();
        var distinctDates = result.Select(r => r.Date).Distinct().ToList();

        Assert.Equal(result.Count, distinctDates.Count);
    }

    [Fact]
    public void Get_ResultIsOrderedByDateAscending()
    {
        var controller = CreateController();

        var result = controller.Get().ToList();
        var sortedDates = result.Select(r => r.Date).OrderBy(d => d).ToList();
        var actualDates = result.Select(r => r.Date).ToList();

        Assert.Equal(sortedDates, actualDates);
    }

    // Regression/boundary: verify TemperatureF at known boundary TemperatureC values
    [Theory]
    [InlineData(-20)]
    [InlineData(0)]
    [InlineData(20)]
    [InlineData(54)]
    public void WeatherForecast_TemperatureFConversion_BoundaryValues(int temperatureC)
    {
        var forecast = new WeatherForecast { TemperatureC = temperatureC };

        int expected = 32 + (int)(temperatureC / 0.5556);

        Assert.Equal(expected, forecast.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_FreezingPoint_TemperatureFIsThirtyTwo()
    {
        // 0°C should be 32°F
        var forecast = new WeatherForecast { TemperatureC = 0 };

        Assert.Equal(32, forecast.TemperatureF);
    }
}