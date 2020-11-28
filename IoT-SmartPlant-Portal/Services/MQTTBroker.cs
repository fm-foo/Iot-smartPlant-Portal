using IoT_SmartPlant_Portal.Application.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IoT_SmartPlant_Portal.Services {

    public interface IMQTTBroker {
        void Subscribe(string topic);
        void Publish(string topic, string messageBody);
        MqttClient GetMQTTClient();
    }


    public class MQTTBroker : IMQTTBroker {
        public MqttClient client;
        public LaunchConfiguration launchConfig;
        public string clientId;

        static InfluxSubscriber _influx = new InfluxSubscriber("http://165.227.149.153:8086", "admin", "admin", "test");

        public MQTTBroker(LaunchConfiguration launchConfiguration) {
            launchConfig = launchConfiguration;
            Subscribe("ESP8266/sensor");
        }

        private bool EnsureConnection() {
            try {
                if (client != null) {
                    if (!client.IsConnected) {
                        client.Connect(clientId,
                                       launchConfig.MqttConfig.FlespiUsername,
                                       launchConfig.MqttConfig.FlespiPassword);
                    }
                    return client.IsConnected;
                } else {
                    client = new MqttClient(launchConfig.MqttConfig.FlespiAddress,
                                       launchConfig.MqttConfig.FlespiPort,
                                       true,
                                       null,
                                       null,
                                       MqttSslProtocols.TLSv1_1);

                    clientId = Guid.NewGuid().ToString();
                    client.Connect(clientId,
                                   launchConfig.MqttConfig.FlespiUsername,
                                   launchConfig.MqttConfig.FlespiPassword);
                }
                return client.IsConnected;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public MqttClient GetMQTTClient() {
            return client;
        }

        public void Subscribe(string topic) {
            try {
                if (EnsureConnection()) {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

                    client.MqttMsgSubscribed += Client_MqttMsgSubscribed;

                    ushort msgId2 = client.Subscribe(new string[] { "ESP8266/sensor" },
                        new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        public void Publish(string topic, string messageBody) {
            try {
                if (EnsureConnection()) {
                    client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                    ushort msgId = client.Publish(topic, // topic
                       Encoding.UTF8.GetBytes(messageBody), // message body
                       MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                       true);
                }

            } catch (Exception ex) {
                throw ex;
            }
        }

        public static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) {
            // handle message received
            Console.WriteLine("message=" + Encoding.UTF8.GetString(e.Message));

            string valuesFromBroker = Encoding.UTF8.GetString(e.Message);
            Plant _plant = JsonConvert.DeserializeObject<Plant>(valuesFromBroker);
            _influx.WritePoint(_plant);
        }

        public void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e) {
            Debug.WriteLine("Subscribed for id = " + e.MessageId);
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) {
            Debug.WriteLine("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);
        }

        private void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e) {
            Debug.WriteLine("Subscribed for id = " + e.MessageId);
        }
    }
}
