using System.Text.Json.Serialization;

namespace WeatherWebApi.Model
{
    public class Clouds
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }


}
