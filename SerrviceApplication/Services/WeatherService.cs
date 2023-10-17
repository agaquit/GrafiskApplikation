using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace SerrviceApplication.Services;

public class WeatherService
{
    private readonly string _url = "https://api.openweathermap.org/data/2.5/weather?lat=59.1875&lon=18.1232&appid=19085af05147b991a57e0241d4ec3735";

    private readonly Timer _timer;
    private readonly HttpClient _http;

    public string? CurrentWeatherCondition { get; private set; }
    public string? CurrentTemperature { get; private set; }
    public event Action? WeatherUpdated;

    public WeatherService(HttpClient http)
    {
        _http = http;
        Task.Run(SetCurrentWeatherAsync);

        _timer = new Timer(60000 * 15);
        _timer.Elapsed += async (s, e) => await SetCurrentWeatherAsync();
        _timer.Start();
        
    }

    private async Task SetCurrentWeatherAsync()
    {
        try
        {
            var data = JsonConvert.DeserializeObject<dynamic>(await _http.GetStringAsync(_url));
            CurrentTemperature = (data!.main.temp - 273.15).ToString("#");
            CurrentWeatherCondition = GetWeatherConditionIcon(data!.weather[0].description.ToString());
        }
        catch
        {
            CurrentTemperature = "--";
            CurrentWeatherCondition = GetWeatherConditionIcon("--");
        
        }

        WeatherUpdated?.Invoke();
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
            await Task.Delay(2000);
            var data = JsonConvert.DeserializeObject<dynamic>(await _http.GetStringAsync(_url));
            CurrentTemperature = (data!.main.temp - 273.15).ToString("#");

            


        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            CurrentTemperature = "--";
        }
    }


}
