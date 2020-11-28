using System;

namespace IoT_SmartPlant_Portal.Application.Configuration {
    public class LaunchConfiguration {
        public string FlespiAddress { get; }
        public string FlespiPort { get; }
        public string FlespiUsername { get; }
        public string FlespiPassword { get; }

        public LaunchConfiguration() {
            FlespiAddress = Environment.GetEnvironmentVariable("FLESPI_ADDRESS") ?? string.Empty;
            FlespiPort = Environment.GetEnvironmentVariable("FLESPI_PORT") ?? string.Empty;
            FlespiUsername = Environment.GetEnvironmentVariable("FLESPI_USERNAME") ?? string.Empty;
            FlespiPassword = Environment.GetEnvironmentVariable("FLESPI_PASSWORD") ?? string.Empty;
        }
    }
}
