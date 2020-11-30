using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using IoT_SmartPlant_Portal.Application.Configuration;
using IoT_SmartPlant_Portal.Models;
using System;

namespace IoT_SmartPlant_Portal.Services {
    public class InfluxDB {

        private LaunchConfiguration launchConfig;
        public InfluxDBClient influxDBClient { get; set; }


        public InfluxDB(LaunchConfiguration launchConfiguration) {
            launchConfig = launchConfiguration;
            ConnectInflux();

        }

        public void ConnectInflux() {
            influxDBClient = InfluxDBClientFactory.Create(launchConfig.InfluxConfig.InfluxAddress,
                                                          launchConfig.InfluxConfig.InfluxUsername,
                                                          launchConfig.InfluxConfig.InfluxPassword.ToCharArray());
        }

        public PointData ConvertToInflux(Plant plant) {
            var point = PointData.Measurement("Fern")
            .Tag("Device ID", plant.DeviceId)
            .Field("Temperature", plant.TemperatureC)
            .Field("Soil Humidity", plant.SoilHumidity)
            .Field("Humidity Level", plant.Humidity)
            .Timestamp(DateTime.UtcNow, WritePrecision.S);

            return point;
        }

        public void WritePoint(Plant plant) {
            if (influxDBClient == null) {
                ConnectInflux();
            }

            PointData convertedMessage = ConvertToInflux(plant);

            using (var writeApi = influxDBClient.GetWriteApi()) {
                writeApi.WritePoint(launchConfig.InfluxConfig.InfluxDatabase, "org", convertedMessage);
            }
            influxDBClient.Dispose();
        }
    }
}
