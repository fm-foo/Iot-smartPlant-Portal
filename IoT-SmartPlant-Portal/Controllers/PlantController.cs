using IoT_SmartPlant_Portal.Services;
using Microsoft.AspNetCore.Mvc;

namespace IoT_SmartPlant_Portal.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class PlantController : ControllerBase {

        public MQTTBroker subscriber;

        public PlantController(MQTTBroker subscriber) {
            subscriber = this.subscriber;
        }

        [HttpGet]
        public int Get() {
            return 0;
        }


    }
}
