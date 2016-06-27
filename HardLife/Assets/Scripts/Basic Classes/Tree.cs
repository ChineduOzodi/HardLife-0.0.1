using UnityEngine;
using System.Collections;
using System;

public class Tree : Plant {

    internal bool isAlive = true;
    internal int maxWood;
    internal int maxFruit;
    internal Date maxAge;
    internal Date matureAge;
    internal string ageText;
    internal int fruit;
    public Tree(string _type, Date _birthTime, int x, int y)
    {
        type = _type;
        birthTime = _birthTime;
        classType = "Tree";

        localMapPositionX = x;
        localMapPositionY = y;

        if (type == "normaltree")
        {
            stackOrder = 2;
            maxFruit = 35;
            maxWood = 75;
            maxAge = new Date(250 * Date.Year);
            matureAge = new Date(5 * Date.Year);
            ageText = "young";
            
        }
        else if (type == "normalbush")
        {
            maxFruit = 15;
            maxWood = 10;
            maxAge = new Date(25 * Date.Year);
            matureAge = new Date(5 * Date.Year);
            fruit = UnityEngine.Random.Range(0, maxFruit);

        }

    }

    public override string GetInfo()
    {
        return "Alive: " + isAlive + "\nFruit: " + fruit + "\n"+ base.GetInfo();
    }

    public override void UpdateAge(Date _currentTime)
    {
        base.UpdateAge(_currentTime);

        if (age < matureAge)
        {
            ageText = "young";
        }
        else
        {
            ageText = "old";
        }
    }
}
