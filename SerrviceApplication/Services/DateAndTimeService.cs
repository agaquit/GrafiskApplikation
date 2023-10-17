using System;
using System.Timers;

namespace SerrviceApplication.Services;

public class DateAndTimeService
{
    private readonly Timer _timer;

    public event Action? TimeUpdated;

    public string? CurrentDate { get; set; }
    public string? CurrentTime { get; set; }

    public DateAndTimeService()
    {
        SetCurrentDateAndTime();

        _timer = new Timer(1000);
        _timer.Elapsed += (s, e) => SetCurrentDateAndTime();
        _timer.Start();
    }

 

    private void SetCurrentDateAndTime()
    {

        CurrentTime = DateTime.Now.ToString("HH:mm");
        CurrentDate = DateTime.Now.ToString("dddd, d MMMM yyyy");
       

        TimeUpdated?.Invoke();

    }


}
