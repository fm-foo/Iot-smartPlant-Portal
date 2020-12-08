using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using IoT_SmartPlant_Portal.Application.Configuration;
using IoT_SmartPlant_Portal.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoT_SmartPlant_Portal.Services {
    public class InfluxDBService {

        private LaunchConfiguration launchConfig;
        public InfluxDBClient influxDBClient { get; set; }


        public InfluxDBService(LaunchConfiguration launchConfiguration) {
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

        public async Task QueryInfluxAsync() {
            var fluxQuery = "from(bucket: \"test\") |> range(start: -1h)" + " |> sample(n: 5, pos: 1)";


            var queryApi = influxDBClient.GetQueryApi();


            // OPTION 2.
            await queryApi.QueryAsync(fluxQuery, "org", (cancellable, record) => {
                //
                // The callback to consume a FluxRecord.
                //
                // cancelable - object has the cancel method to stop asynchronous query
                //
                Console.WriteLine($"{record.GetTime()}: {record.GetField()} {record.GetValueByKey("_value")}");
            }, exception => {
                //
                // The callback to consume any error notification.
                //

                Console.WriteLine($"Error occurred: {exception.Message}");
            }, () => {
                //
                // The callback to consume a notification about successfully end of stream.
                //
                Console.WriteLine("Query completed");
            });

        }
    }
}
