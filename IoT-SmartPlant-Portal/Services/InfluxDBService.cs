using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using IoT_SmartPlant_Portal.Application.Configuration;
using IoT_SmartPlant_Portal.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace IoT_SmartPlant_Portal.Services {
    public class InfluxDBService {

        private LaunchConfiguration launchConfig;
        public InfluxDBClient influxDBClient { get; set; }
        public List<InfluxQuery> InfluxQueryData = new List<InfluxQuery>();

        public InfluxDBService(LaunchConfiguration launchConfiguration) {
            launchConfig = launchConfiguration;
            ConnectInflux();

        }

        public void ConnectInflux() {
            influxDBClient = InfluxDBClientFactory.Create(launchConfig.InfluxConfig.InfluxAddress,
                                                          launchConfig.InfluxConfig.InfluxUsername,
                                                          launchConfig.InfluxConfig.InfluxPassword.ToCharArray());
        }

        public PointData ConvertToInflux(PlantData plant) {
            var point = PointData.Measurement("Fern")
            .Tag("Device ID", plant.DeviceId)
            .Field("Temperature", plant.TemperatureC)
            .Field("Soil Humidity", plant.SoilHumidity)
            .Field("Humidity Level", plant.Humidity)
            .Timestamp(DateTime.UtcNow, WritePrecision.S);

            return point;
        }

        public void WritePoint(PlantData plant) {
            if (influxDBClient == null) {
                ConnectInflux();
            }

            PointData convertedMessage = ConvertToInflux(plant);

            using (var writeApi = influxDBClient.GetWriteApi()) {
                writeApi.WritePoint(launchConfig.InfluxConfig.InfluxDatabase, "org", convertedMessage);
            }
            influxDBClient.Dispose();
        }

        public async Task<List<InfluxQuery>> QueryInfluxAsync(string deviceID) {
            var fluxQuery = $"from(bucket: \"test\") |> range(start: -5m) |> filter(fn: (r) => (r[\"Device ID\"] == \"{deviceID}\"))";

            var queryApi = influxDBClient.GetQueryApi();

            InfluxQueryData.Clear();

            await queryApi.QueryAsync(fluxQuery, "org", (cancellable, record) => {
                //
                // The callback to consume a FluxRecord.
                //
                // cancelable - object has the cancel method to stop asynchronous query
                //
                InfluxQueryData.Add(new InfluxQuery(record.GetTime(), record.GetField(), record.GetValueByKey("_value")));

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


            return InfluxQueryData;
        }

        public List<PlantData> GetPlantDataFromQuery(string deviceId) {

            List<PlantData> plantDataList = new List<PlantData>();

            // order by timestamp, the list we obtain will be order in a way where every 3 in 3 values are 1 plant object
            var data = InfluxQueryData.OrderBy(x => x.TimeStamp).ToArray();

            for (int i = 0; i < data.Length / 3; i++) {

                int index = i * 3;

                object soilHumidity = null;
                object humidity = null;
                object temperature = null;

                for (int j = 0; j < 3; j++) {

                    int indexes = 0;
                    indexes = index + j;

                    switch (data[indexes].Field) {
                        case "Soil Humidity":
                            soilHumidity = data[indexes].Value;
                            break;
                        case "Temperature":
                            temperature = data[indexes].Value;
                            break;
                        case "Humidity Level":
                            humidity = data[indexes].Value;
                            break;
                        default:
                            break;
                    }
                }

                PlantData plantdata = new PlantData {
                    DeviceId = deviceId,
                    SoilHumidity = Convert.ToDouble(soilHumidity),
                    Humidity = Convert.ToDouble(humidity),
                    TemperatureC = Convert.ToDouble(temperature)
                };

                plantDataList.Add(plantdata);
            }

            return plantDataList;
        }


    }
}
