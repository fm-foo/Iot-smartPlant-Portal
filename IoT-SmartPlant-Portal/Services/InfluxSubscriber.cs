using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoT_SmartPlant_Portal.Services
{
    public class InfluxSubscriber
    {
        public InfluxSubscriber(string HostName, string UserName, string Password, string DatabaseName)
        {
            this.HostName = HostName;
            this.UserName = UserName;
            this.Password = Password;
            this.DatabaseName = DatabaseName;
        }

        public string HostName = "http://165.227.149.153:8086";
        public string UserName = "admin";
        public string Password = "admin";
        public string DatabaseName = "test";

        public PointData ConvertToInflux(Plant plant)
        {
            /*var point = new InfluxDatapoint<InfluxValueField>();

            point.UtcTimestamp = DateTime.UtcNow;
            point.MeasurementName = "Fern";
            point.Fields.Add("Temperature", new InfluxValueField(plant.TemperatureC));
            point.Fields.Add("Soil Humidity", new InfluxValueField(plant.SoilHumidity));
            point.Fields.Add("Humidity Level", new InfluxValueField(plant.Humidity));
            point.Precision = TimePrecision.Seconds;

            using (var client = new AdysTech.InfluxDB.Client.Net.InfluxDBClient(HostName, UserName, Password))
            {
                    var r = await client.PostPointAsync(DatabaseName, point);
                }*/

                
                    var point = PointData.Measurement("Fern")
                    .Field("Temperature",  plant.TemperatureC)
                    .Field("Soil Humidity", plant.SoilHumidity)
                    .Field("Humidity Level", plant.Humidity)

                    .Timestamp(DateTime.UtcNow, WritePrecision.S);

                    return point;
            }

            public void WritePoint(Plant plant)
        {

            InfluxDBClient influxDBClient = InfluxDBClientFactory.Create(HostName, UserName, Password.ToCharArray());
            PointData convertedMessage = ConvertToInflux(plant);

            using (var writeApi = influxDBClient.GetWriteApi())
            {
                writeApi.WritePoint(DatabaseName, "org", convertedMessage);
                //writeApi.WritePoint(convertedMessage);
            }
            influxDBClient.Dispose();
        }
    }
}
