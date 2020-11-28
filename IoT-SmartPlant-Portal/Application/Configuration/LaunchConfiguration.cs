using System;

namespace IoT_SmartPlant_Portal.Application.Configuration {
    public class LaunchConfiguration {
        public string FlespiAddress { get; }
        public int FlespiPort { get; }
        public string FlespiUsername { get; }
        public string FlespiPassword { get; }

        public LaunchConfiguration() {
            FlespiAddress = Environment.GetEnvironmentVariable("FLESPI_ADDRESS") ?? string.Empty;
            FlespiPort = Convert.ToInt32(Environment.GetEnvironmentVariable("FLESPI_PORT"));
            FlespiUsername = Environment.GetEnvironmentVariable("FLESPI_USERNAME") ?? string.Empty;
            FlespiPassword = Environment.GetEnvironmentVariable("FLESPI_PASSWORD") ?? string.Empty;
        }
    }
}
