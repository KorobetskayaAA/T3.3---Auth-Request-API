using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherWebApi.Model;

namespace WeatherWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        const string requestUrl = "http://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&lang=ru&units=metric";
        private readonly ILogger<WeatherForecastController> _logger;
        private string weatherApiKey;
        static readonly string[] windDirections = new string[] {
            "северный", "северо-восточный",
            "восточный", "юго-восточный",
            "южный", "юго-западный",
            "западный", "северо-западный"
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            weatherApiKey = configuration["WeatherApi:Key"];
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get(string city = "Самара")
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format(requestUrl, city, weatherApiKey));
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = response.GetResponseStream())
                {
                    var weather = await JsonSerializer.DeserializeAsync<WeatherResponse>(stream);
                    string windDirection = windDirections[(weather.Wind.Deg + 30) % 360 / 60];
                    return $"В городе {city} {weather.Main.Temp}°С," +
                        $" {weather.Weather[0].Description}," +
                        $" ветер {windDirection} {weather.Wind.Speed} м/с";
                }
            }
            return StatusCode((int)response.StatusCode);
        }
    }
}
