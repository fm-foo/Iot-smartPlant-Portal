﻿using System;

namespace IoT_SmartPlant_Portal.Application.Configuration {
    public class LaunchConfiguration {
        public MQTTBrokerConfiguration MqttConfig { get; set; }
        public InfluxDBConfiguration InfluxConfig { get; set; }

        public LaunchConfiguration() {
            InfluxConfig = new InfluxDBConfiguration();
            MqttConfig = new MQTTBrokerConfiguration();
        }
    }

    public class MQTTBrokerConfiguration {
        public string FlespiAddress { get; }
        public int FlespiPort { get; }
        public string FlespiUsername { get; }
        public string FlespiPassword { get; }

        public MQTTBrokerConfiguration() {
            FlespiAddress = Environment.GetEnvironmentVariable("FLESPI_ADDRESS");
            FlespiPort = Convert.ToInt32(Environment.GetEnvironmentVariable("FLESPI_PORT"));
            FlespiUsername = Environment.GetEnvironmentVariable("FLESPI_USERNAME");
            FlespiPassword = Environment.GetEnvironmentVariable("FLESPI_PASSWORD") ?? string.Empty;
        }
    }

    public class InfluxDBConfiguration {
        public string InfluxAddress { get; }
        public string InfluxUsername { get; }
        public string InfluxPassword { get; }
        public string InfluxDatabase { get; }

        public InfluxDBConfiguration() {
            InfluxAddress = Environment.GetEnvironmentVariable("INFLUX_ADDRESS");
            InfluxUsername = Environment.GetEnvironmentVariable("INFLUX_USERNAME");
            InfluxPassword = Environment.GetEnvironmentVariable("INFLUX_PASSWORD");
            InfluxDatabase = Environment.GetEnvironmentVariable("INFLUX_DATABASE");
        }

    }


}
