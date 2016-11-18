using UnityEngine;
using System.Collections;
using System;

public class Tree : Bush {

    
    internal int maxWood;
    
    internal string ageText = "Young";
    
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

            maxLeaves = 40;
            leaves = maxLeaves;

            birthPercent = 3 * birthFactor;

            maxAge = new Date(250 * Date.Year);
            matureLevel = new Date(3 * Date.Year);
        }

    }

    public override void UpdateAge(Date _currentTime)
    {
        base.UpdateAge(_currentTime);
    }
    public override void UpdateGrowth(float temp) //Hourly Update
    {

        base.UpdateGrowth(temp);

        if (age > new Date(5 * Date.Year) && ageText == "Young")
        {
            ageText = "Mature";
            updateTexture = true;
        }
    }
}
