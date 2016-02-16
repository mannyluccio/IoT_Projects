using System;
using System.Collections;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;

namespace WS_Project
{
    class MqttClientWrapper
    {
        private MqttClient _myMqttClient;
        private string _clientId = "";
        private bool _clientIdOnDataTx;
        private Thread _reconnectionThread;
        private int _retryConnectionInterval;

        private const char ClientIdOnDataSeparator = '§';
        public event OnConnectionStateChangeEventHandler OnConnectionStateChange;

        public delegate void OnConnectionStateChangeEventHandler(bool isConnected);

        public event OnMqttMsgPublishedEventHandler OnMqttMsgPublished;

        public delegate void OnMqttMsgPublishedEventHandler(
            uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishedEventArgs e);

        public event OnMqttMsgPublishReceivedEventHandler OnMqttMsgPublishReceived;

        public delegate void OnMqttMsgPublishReceivedEventHandler(
            string clientId, string data, string topic, byte qualityOfService, bool isRetained);

        public event OnMqttMsgSubscribedEventHandler OnMqttMsgSubscribed;

        public delegate void OnMqttMsgSubscribedEventHandler(
            uPLibrary.Networking.M2Mqtt.Messages.MqttMsgSubscribedEventArgs e);

        public event OnMqttMsgUnsubscribedEventHandler OnMqttMsgUnsubscribed;

        public delegate void OnMqttMsgUnsubscribedEventHandler(
            uPLibrary.Networking.M2Mqtt.Messages.MqttMsgUnsubscribedEventArgs e);


        public void Init(string brokerHostName, int brokerIpPort = 1883, int retryConnectionTime = 10000)
        {
            Init(Guid.NewGuid().ToString(), brokerHostName, brokerIpPort, retryConnectionTime);
        }

        public void Init(string clientId, string brokerHostName, int brokerIpPort = 1883, int retryConnectionTime = 10000)
        {
            _myMqttClient = new MqttClient(brokerHostName, brokerIpPort, false, null, MqttSslProtocols.None);
            _clientId = clientId;
            _retryConnectionInterval = retryConnectionTime;

            _myMqttClient.ConnectionClosed += MyMQTTClient_MqttMsgDisconnected;
            _myMqttClient.MqttMsgPublished += MyMQTTClient_MqttMsgPublished;
            _myMqttClient.MqttMsgPublishReceived += MyMQTTClient_MqttMsgPublishReceived;
            _myMqttClient.MqttMsgSubscribed += MyMQTTClient_MqttMsgSubscribed;
            _myMqttClient.MqttMsgUnsubscribed += MyMQTTClient_MqttMsgUnsubscribed;


            _reconnectionThread = new Thread(Reconnect);

        }



        public bool IsConnected
        {
            get
            {
                if (IsInitialized)
                {
                    return _myMqttClient.IsConnected;
                }

                return false;
            }
        }


        public bool IsInitialized
        {
            get
            {
                return _myMqttClient != null;
            }
        }

        public void Connect(bool async = true)
        {
            if (!_myMqttClient.IsConnected)
            {
                try
                {
                    _myMqttClient.Connect(_clientId);
                }
                catch
                {
                    asyncReconnect();
                }
            }

            if (OnConnectionStateChange != null)
            {
                OnConnectionStateChange(_myMqttClient.IsConnected);
            }
        }


        public void Disconnect()
        {
            if (_myMqttClient.IsConnected)
            {
                _myMqttClient.Disconnect();
            }
        }

        public void Reconnect()
        {
            if (_myMqttClient != null)
            {
                Thread.Sleep(_retryConnectionInterval);
                Connect();
            }

        }

        public void asyncReconnect()
        {
            //Meccanismo riconessione
            if (_reconnectionThread != null)
            {
                if (!_reconnectionThread.IsAlive)
                {
                    _reconnectionThread.Start();
                }
                else
                {
                    _reconnectionThread = new Thread(Reconnect);
                    _reconnectionThread.Start();
                }
            }
            else
            {
                _reconnectionThread = new Thread(Reconnect);
                _reconnectionThread.Start();
            }

        }


        public void Subscribe(string[] topics, byte[] qosLevels)
        {
            _myMqttClient.Subscribe(topics, qosLevels);
        }

        public void Subscribe(ref ArrayList topics, ref ArrayList qualityOfService)
        {
            var aTopics = new[] { Convert.ToString(topics.ToArray(typeof(string))) };
            var aQoSs = new[] { Convert.ToByte(qualityOfService.ToArray(typeof(byte))) };

            _myMqttClient.Subscribe(aTopics, aQoSs);
        }

        public void Publish(string topic, string message, byte qosLevel = 0, bool retain = false)
        {
            string data;

            if (_clientIdOnDataTx)
            {
                data = _clientId + ClientIdOnDataSeparator + message;
            }
            else
            {
                data = message;
            }

            Publish(topic, Encoding.UTF8.GetBytes(data), 0, retain);
        }

        public void Publish(string topic, byte[] message, byte qosLevel = 0, bool retain = false)
        {
            if (_myMqttClient.IsConnected)
            {
                try
                {
                    _myMqttClient.Publish(topic, message, 0, retain);
                }
                catch
                {
                    try
                    {
                        _myMqttClient.Disconnect();
                    }
                    catch
                    {
                        //var a = "";
                    }
                }
            }
            else
            {
                Connect();
            }
        }

        private void MyMQTTClient_MqttMsgDisconnected(object sender, EventArgs e)
        {
            //if (OnConnectionStateChange != null)
            //{
            //    OnConnectionStateChange(MyMQTTClient.IsConnected);
            //}
            _reconnectionThread = null;
            Connect();
        }

        private void MyMQTTClient_MqttMsgPublished(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishedEventArgs e)
        {
            if (OnMqttMsgPublished != null)
            {
                OnMqttMsgPublished(e);
            }
        }

        private void MyMQTTClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            string clientIdSender = "";
            var Data = new string(Encoding.UTF8.GetChars(e.Message));

            if (Data.Contains(ClientIdOnDataSeparator.ToString()))
            {
                string[] arrData = Data.Split(ClientIdOnDataSeparator);
                clientIdSender = arrData[0];
                Data = arrData[1];
            }

            if (OnMqttMsgPublishReceived != null)
            {
                OnMqttMsgPublishReceived(clientIdSender, Data, e.Topic, e.QosLevel, e.Retain);
            }
        }

        private void MyMQTTClient_MqttMsgSubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgSubscribedEventArgs e)
        {
            if (OnMqttMsgSubscribed != null)
            {
                OnMqttMsgSubscribed(e);
            }
        }

        private void MyMQTTClient_MqttMsgUnsubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgUnsubscribedEventArgs e)
        {
            if (OnMqttMsgUnsubscribed != null)
            {
                OnMqttMsgUnsubscribed(e);
            }
        }
    }


}
