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
        public List<TestInluxModel> list = new List<TestInluxModel>();

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

        public class TestInluxModel {
            public DateTime TimeStamp { get; set; }
            public string Field { get; set; }
            public object Value { get; set; }

            public TestInluxModel(DateTime timeStamp, string field, object value) {
                TimeStamp = timeStamp;
                Field = field;
                Value = value;
            }
        }

        public async Task<List<TestInluxModel>> QueryInfluxAsync(string deviceID) {
            var fluxQuery = $"from(bucket: \"test\") |> range(start: -5m) |> filter(fn: (r) => (r[\"Device ID\"] == \"{deviceID}\"))";

            var queryApi = influxDBClient.GetQueryApi();

            list.Clear();

            await queryApi.QueryAsync(fluxQuery, "org", (cancellable, record) => {
                //
                // The callback to consume a FluxRecord.
                //
                // cancelable - object has the cancel method to stop asynchronous query
                //
                list.Add(new TestInluxModel(Convert.ToDateTime(record.GetTime().ToString()), record.GetField(), record.GetValueByKey("_value")));

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


            return list;
        }
    }
}
