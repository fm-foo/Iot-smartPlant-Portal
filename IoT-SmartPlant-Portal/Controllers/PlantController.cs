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
            var test = await MQTTBroker.GetInfluxClient().QueryInfluxAsync();
            return 0;
        }


    }
}
