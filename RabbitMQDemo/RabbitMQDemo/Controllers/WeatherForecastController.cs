using Microsoft.AspNetCore.Mvc;

namespace RabbitMQDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sonraki 5 gün için örnek hava durumu tahmin verilerini getirir.
        /// </summary>
        /// <remarks>
        /// Bu endpoint demo amaçlý rastgele hava durumu verileri üretmektedir.
        /// Gerçek meteorolojik veriler kullanýlmamaktadýr.
        /// </remarks>
        /// <returns>Hava durumu tahmin bilgilerini içeren liste.</returns>
        /// <response code="200">Hava durumu tahminleri baþarýyla getirildi.</response>
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
