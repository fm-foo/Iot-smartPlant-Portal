using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using IoT_SmartPlant_Portal.Services;
using Microsoft.AspNetCore.Mvc;

namespace IoT_SmartPlant_Portal.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase {
        public Subscriber subscriber;



        public WeatherForecastController(Subscriber subscriber) {
            subscriber = this.subscriber;
        }

        [HttpGet]
        public int Get() {
            return 0;
        }



    }
}
