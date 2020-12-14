using Newtonsoft.Json;

namespace IoT_SmartPlant_Portal.Models {
    public class PlantData {

        [JsonProperty("soil_humidity")]
        public double SoilHumidity { get; set; }

        [JsonProperty("humidity")]
        public double Humidity { get; set; }

        [JsonProperty("temperature_c")]
        public double TemperatureC { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }
    }
}
