﻿using InfluxDB.Client;
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

        public string HostName = "165.227.149.153:8086";
        public string UserName = "admin";
        public string Password = "admin";
        public string DatabaseName = "test";

        public PointData ConvertToInflux(Plant plant)
        {
                var point = PointData.Measurement("Plant")
                .Tag("PlantType", "Fern")

                .Field("Temperature", plant.TemperatureC)
                .Field("Soil Humidity", plant.SoilHumidity)
                .Field("Humidity Level", plant.Humidity)

                .Timestamp(DateTime.UtcNow, WritePrecision.S);

                return point;
        }

        public void WritePoint(Plant plant)
        {
            var influxDBClient = InfluxDBClientFactory.Create(HostName, UserName, Password.ToCharArray());
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