using IoT_SmartPlant_Portal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IoT_SmartPlant_Portal.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class PlantController : ControllerBase {
        public IMQTTBroker MQTTBroker;

        public PlantController(IMQTTBroker MQTT) {
            MQTTBroker = MQTT;
        }

        [HttpGet]
        public async Task<int> GetAsync(string deviceID) {
            deviceID = "87761e13-d509-4aa4-8dca-6e0915f6645b";
            var test = await MQTTBroker.GetInfluxClient().QueryInfluxAsync(deviceID);
            return 0;
        }


    }
}
