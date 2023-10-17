using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace AzureFunctions.Methods
{
    public class WeatherDevice
    {
        private readonly string _url = "https://api.openweathermap.org/data/2.5/weather?lat=59.1875&lon=18.1232&appid=19085af05147b991a57e0241d4ec3735";
        private DeviceClient deviceClient = DeviceClient.CreateFromConnectionString("HostName=kyhstockholm-iothub.azure-devices.net;DeviceId=temperature_device;SharedAccessKey=Jzi9hz45HihGpAW2xU4xqkyWJLLgjHCKmAIoTPpGSOw=");
        private TemperatureData temperatureData = new TemperatureData();


        [Function("WeatherDevice")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] DeviceInfo myTimer)
        {
            try
            {
                using var _http = new HttpClient();
                var data = JsonConvert.DeserializeObject<dynamic>(await _http.GetStringAsync(_url));
                temperatureData.CurrentTemperature = (data!.main.temp - 273.15).ToString("#");
                temperatureData.CurrentWeatherCondition = GetWeatherConditionIcon(data!.weather[0].description.ToString());
            }
            catch
            {
                temperatureData.CurrentTemperature = "--";
                temperatureData.CurrentWeatherCondition = GetWeatherConditionIcon("--");

            }

            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(temperatureData)));
            await deviceClient.SendEventAsync(message);
        }



        private string GetWeatherConditionIcon(string value)
        {
            return value switch
            {
                "clear sky" => "\ue28f",
                "few clouds" => "\uf6c4",
                "overcast clouds" => "\uf744",
                "scattered clouds" => "\uf0c2",
                "broken clouds" => "\uf744",
                "shower rain" => "\uf738",
                "rain" => "\uf740",
                "thunderstorm" => "\uf76c",
                "snow" => "\uf742",
                "mist" => "\uf74e",
                _ => "\ue137",
            };
        }

        private async Task GetTemperature()
        {
            try
            {
                using var _http = new HttpClient();
                var data = JsonConvert.DeserializeObject<dynamic>(await _http.GetStringAsync(_url));
                temperatureData.CurrentTemperature = (data!.main.temp - 273.15).ToString("#");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                temperatureData.CurrentTemperature = "--";
            }
        }
    }

    public class TemperatureData
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string? CurrentWeatherCondition { get; set; }
        public string? CurrentTemperature { get;set; }

    }



    public class DeviceInfo
    {

    }
}
