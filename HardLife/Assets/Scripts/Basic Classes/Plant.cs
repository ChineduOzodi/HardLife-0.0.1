using System;
using UnityEngine;

public class Plant: Alive
{
    internal float growthRate = 1;
    
    internal float minTemp;
    internal Date growthLevel = new Date(0);
    internal Date matureLevel;
    internal string state = "Normal";

    public Plant(string _type, Date _birthTime, Vector3 _worldPosition, int x, int y)
        :base(_type, _birthTime, _worldPosition, x, y)
    {

    }

    public virtual void UpdateGrowth(float temp) //Hourly Update
    {
        if (temp < minTemp)
        {
            growthRate = 0;
        }
        else
        {
            growthRate = 1;
        }

        if (growthLevel < matureLevel)
            growthLevel.AddTime(Date.Hour * growthRate);

    }

    public override string GetInfo()
    {
        return "Min. Temperature: " + Math.Round(minTemp,1) + " C\nGrowth Level: " + Mathf.FloorToInt(growthLevel.time/matureLevel.time * 100) + " %\nGrowth Rate: " + growthRate + "\n" + base.GetInfo();
    }
}