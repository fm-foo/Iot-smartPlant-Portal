using IoT_SmartPlant_Portal.Services;
using Microsoft.AspNetCore.Mvc;

namespace IoT_SmartPlant_Portal.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class PlantController : ControllerBase {
        public IMQTTBroker MQTTBroker;

        public PlantController(IMQTTBroker MQTT) {
            MQTTBroker = MQTT;
        }

        [HttpGet]
        public int Get() {
            MQTTBroker.Subscribe("ESP8266/sensor");
            return 0;
        }


    }
}
