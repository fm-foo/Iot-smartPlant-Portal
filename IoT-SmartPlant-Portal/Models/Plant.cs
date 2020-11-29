using Newtonsoft.Json;

namespace IoT_SmartPlant_Portal.Models {
    public class Plant {

        [JsonProperty("soil_humidity")]
        public float SoilHumidity { get; set; }

        [JsonProperty("humidity")]
        public float Humidity { get; set; }

        [JsonProperty("temperature_c")]
        public float TemperatureC { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }
    }
}
