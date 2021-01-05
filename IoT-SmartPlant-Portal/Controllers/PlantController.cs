using IoT_SmartPlant_Portal.Models;
using IoT_SmartPlant_Portal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoT_SmartPlant_Portal.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class PlantController : ControllerBase {
        public IMQTTBroker MQTTBroker;

        public PlantController(IMQTTBroker MQTT) {
            MQTTBroker = MQTT;
        }

        [HttpGet()]
        public async Task<int> GetAsync() {
            string deviceID = "87761e13-d509-4aa4-8dca-6e0915f6645b";
            var test = await MQTTBroker.GetInfluxClient().QueryInfluxAsync(deviceID);
            return 0;
        }

        [HttpPost("Change/{id}/{isActive}")]
        public void ChangeAutomaticWatering(string deviceID, bool isActive) {

            deviceID = "87761e13-d509-4aa4-8dca-6e0915f6645b";

            if (isActive) {
                MQTTBroker.Publish("ESP8266/Pump", "ON");
            } else {
                MQTTBroker.Publish("ESP8266/Pump", "OFF");
            }

        }

        [HttpGet("QueryData")]
        public async Task<List<PlantData>> QueryData(string deviceID) {
            deviceID = "87761e13-d509-4aa4-8dca-6e0915f6645b";
            List<InfluxQuery> test = await MQTTBroker.GetInfluxClient().QueryInfluxAsync(deviceID);
            if (test != null) {
                List<PlantData> plantData = MQTTBroker.GetInfluxClient().GetPlantDataFromQuery(deviceID);
                return plantData;
            }
            return null;
        }


    }
}
