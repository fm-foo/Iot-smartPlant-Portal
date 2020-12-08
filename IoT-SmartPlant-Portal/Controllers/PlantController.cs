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
        public async Task<int> GetAsync() {
            var test = await MQTTBroker.GetInfluxClient().QueryInfluxAsync("87761e13-d509-4aa4-8dca-6e0915f6645b");
            return 0;
        }


    }
}
