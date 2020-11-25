using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IoT_SmartPlant_Portal.Services {
    public class Subscriber {
        static InfluxSubscriber _influx = new InfluxSubscriber("165.227.149.153:8086", "admin", "admin", "test");
        MqttClient client;
        public Subscriber() {
            InitialSetup();
        }

        private void InitialSetup() {
            string BrokerAddress = "mqtt.flespi.io";

            client = new MqttClient(BrokerAddress,
                                    8883,
                                    true,
                                    null,
                                    null,
                                    MqttSslProtocols.TLSv1_1);

            var clientId = Guid.NewGuid().ToString();
            client.Connect(clientId, "FlespiToken pUgcWLHBBPPcrVYPwXB8tXC8L4fTZAFejJg5lLxKoaMFWyjglIeEvEB7wEOl4jpd", "");

            try {
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                ushort msgId = client.Publish("/my_topic", // topic
                   Encoding.UTF8.GetBytes("MyMessageBody"), // message body
                   MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                   true);

                client.Subscribe(new string[] { "ESP8266/sensor" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

                client.MqttMsgSubscribed += Client_MqttMsgSubscribed;
                client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

                ushort msgId2 = client.Subscribe(new string[] { "ESP8266/sensor" },
                    new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            } catch (Exception ex) {
                throw ex;
            }
        }

        static void client_MqttMsgPublishReceived(
            object sender, MqttMsgPublishEventArgs e) {
            // handle message received
            string valuesFromBroker = Encoding.UTF8.GetString(e.Message);
            Plant _plant = JsonConvert.DeserializeObject<Plant>(valuesFromBroker);
            _influx.WritePoint(_plant);

        }

        void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e) {
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
