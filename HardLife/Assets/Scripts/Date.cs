using System;
using UnityEngine;

internal struct Date
{
    private static int Seasons = 4;
    private static int Days = 10;
    private static int Hours = 24;
    private static int Minutes = 15;

    public static int Year = 14400; //Seconds in a year 14400
    public static int Season = 3600;
    public static int Day = 360;
    public static int Hour = 15;

    public float time;
    public float deltaTime;
    public int year;
    public int season;
    public int day;
    public int hour;
    private float minute;

    public Date(float _time)
    {
        time = _time;
        deltaTime = 0;

        year = Mathf.FloorToInt(_time / Year);
        _time = _time % Year;
        season = Mathf.FloorToInt(_time / Season);
        _time = _time % Season;
        day = Mathf.FloorToInt(_time / Day);
        _time = _time % Day;
        hour = Mathf.FloorToInt(_time / Hour);
        _time = _time % Hour;
        minute = _time;
    }

    public void AddTime(float _time)
    {
        deltaTime = _time;
        time += _time;

        minute += _time;
        if (minute > Minutes) //Setting the Add Time parts
        {
            hour += Mathf.FloorToInt(minute / Minutes);
            minute = minute % Minutes;

            if (hour > Hours)
            {
                day += Mathf.FloorToInt(hour / Hours);
                hour = hour % Hours;

                if (day > Days)
                {
                    season += Mathf.FloorToInt(day / Days);
                    day = day % Days;

                    if (season > Seasons)
                    {
                        year += Mathf.FloorToInt(season / Seasons);
                        season = season % Seasons;

                    }
                }
            }
        }



    }

    private void UpdateDate(float _time)
    {
        year = Mathf.FloorToInt(_time / Year);
        _time = _time % Year;
        season = Mathf.FloorToInt(_time / Season);
        _time = _time % Season;
        day = Mathf.FloorToInt(_time / Day);
        _time = _time % Day;
        hour = Mathf.FloorToInt(_time / Hour);
        _time = _time % Hour;
    }

    public string GetDate()
    {
        return year.ToString() + "/" + season.ToString() + "/" + day.ToString() + " " + hour.ToString() + " h";
    }
    public string GetDate(float _time)
    {
        float year = Mathf.FloorToInt(_time / Year);
        _time = _time % Year;
        float season = Mathf.FloorToInt(_time / Season);
        _time = _time % Season;
        float day = Mathf.FloorToInt(_time / Day);
        _time = _time % Day;
        float hour = Mathf.FloorToInt(_time / Hour);
        _time = _time % Hour;

        return year.ToString() + "/" + season.ToString() + "/" + day.ToString() + " " + hour.ToString() + " h";
    }
}