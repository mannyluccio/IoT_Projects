using System;
using System.Configuration;
using System.Threading;
using Newtonsoft.Json;


namespace WS_Project
{
    class Program
    {

        private static int _portaBroker = Convert.ToInt32(ConfigurationManager.AppSettings.Get("portaBroker"));
        private static String _indirizzoBroker = ConfigurationManager.AppSettings.Get("indirizzoBroker");
        private static String weatherDataTopic = ConfigurationManager.AppSettings.Get("weatherDataTopic");
        private static int _retryInterval = Convert.ToInt32(ConfigurationManager.AppSettings.Get("retryInterval"));
        private static MqttClientWrapper _mqttClient;
        private static ModelDBContainer db;


        static void Main(string[] args)
        {

            //Provo a connettermi al broker
            _mqttClient = new MqttClientWrapper();
            Thread t = new Thread(connettiAlBroker);
            t.Start();
            //Instanzio il database
            db = new ModelDBContainer();

            Console.ReadLine();

        }


        private static void connettiAlBroker()
        {
            _mqttClient.Init("WS_Server", _indirizzoBroker, _portaBroker, _retryInterval);
            _mqttClient.OnConnectionStateChange += OnConnectionStateChange;
            _mqttClient.Connect();
        }

        private static void OnNewWeatherDataReceived(string ClientID, string Data, string Topic, byte QualityOfService, bool IsRetained)
        {


            if (Topic == weatherDataTopic)
            {
                try
                {
                    Console.WriteLine("Dati Meteo: " + Data);
                    WeatherData wData = JsonConvert.DeserializeObject<WeatherData>(Data);
                    db.DatiMeteo.Add(WeatherDataToDbEntity(wData));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Eccezione: " + ex.Message);
                    Console.WriteLine("Dati ricevuti non validi: " + Data);
                }

            }
            else if (Topic == "test")
            {
                string dataTest = @"{""Temperatura"":22,""Umidità"": 60,""Aria"": '12',""Pressione"": '999',""Pioggia"": 48}";

                try
                {
                    Console.WriteLine("Dati Meteo di test: " + dataTest);
                    WeatherData wData = JsonConvert.DeserializeObject<WeatherData>(dataTest);
                    db.DatiMeteo.Add(WeatherDataToDbEntity(wData));
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Eccezione: " + ex.Message);
                    Console.WriteLine("Dati ricevuti non validi: " + dataTest);
                }
            }

        }


        private static void OnConnectionStateChange(bool isConnected)
        {
            if (isConnected)
            {
                Console.WriteLine("Connesso al broker");
                _mqttClient.Subscribe(new string[] { weatherDataTopic},
                                         new byte[] { 0 });
                _mqttClient.Subscribe(new string[] { "test" },
                                         new byte[] { 0 });
                Console.WriteLine("Sottoscritto il topic " + weatherDataTopic);
                _mqttClient.OnMqttMsgPublishReceived += OnNewWeatherDataReceived;

                //Instanzio un timer per il keep alive
                //timerKeepAlive_Init();
            }
            else
            {
                Console.WriteLine("Disconnesso dal broker, tentativo riconessione...");

            }
        }

        private static DatiMeteo WeatherDataToDbEntity(WeatherData wd)
        {
            DatiMeteo dm = new DatiMeteo();

            dm.Aria = wd.Aria;
            dm.Data = DateTime.Now;
            dm.Pioggia = wd.Pioggia;
            dm.Pressione = wd.Pressione;
            dm.Temperatura = wd.Temperatura;
            dm.Umidità = wd.Umidità;
            return dm;

        }
    }
}
