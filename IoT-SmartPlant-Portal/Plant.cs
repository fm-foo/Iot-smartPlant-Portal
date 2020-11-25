using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IoT_SmartPlant_Portal
{
    public class Plant
    {
        [JsonProperty("soil_humidity")]
        public float SoilHumidity { get; set; }
        [JsonProperty("humidity")]
        public float Humidity { get; set; }
        [JsonProperty("temperature_c")]
        public float TemperatureC { get; set; }

    }
}
