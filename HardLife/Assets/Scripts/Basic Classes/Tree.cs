using UnityEngine;
using System.Collections;
using System;

public class Tree : Bush {

    
    internal int maxWood;
    
    internal string ageText = "young";
    
    public Tree(string _type, Date _birthTime, Vector3 _worldPosition, int x, int y)
        :base(_type,_birthTime,_worldPosition, x,y)
    {
        classType = "Tree";

        if (type == "oak tree")
        {
            renderOrder = 2;
            minTemp = 5;
            maxFruit = 35;
            maxWood = 75;
            maxStick = 20;

            maxAge = new Date(250 * Date.Year);
            matureLevel = new Date(3 * Date.Year);
        }

    }

    public override string GetInfo()
    {
        return "Alive: " + isAlive + "\nFruit: " + fruit + "\n"+ base.GetInfo();
    }

    public override void UpdateAge(Date _currentTime)
    {
        base.UpdateAge(_currentTime);

        
    }

    internal void UpdateGrowth(float temp) //Hourly Update
    {
        if (temp > minTemp)
        {
            growthLevel.AddTime( Date.Hour);
            if (growthLevel > matureLevel)
            {
                ageText = "mature";
                updateTexture = true;
            }
        }
        else
        {

        }
    }
}
