using System;
using System.Text.Json.Serialization;

namespace FriendsApp2.Api.Models
{
    public class WeatherForecast
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }

        // This property doesn't need to be serialized because it's computed dynamically.
        // This shows that the property evaluation logic runs on the client as well as
        // on the server.
        [JsonIgnore]
        public int TemperatureF
        {
            get
            {
                return 32 + (int)(TemperatureC / 0.5556);
            }
        }
    }
}