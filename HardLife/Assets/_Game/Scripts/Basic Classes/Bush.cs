using UnityEngine;
using System.Collections;

public class Bush : Plant
{
    internal int maxStick;
    
    

    public Bush(string _type, Date _birthTime, Vector3 _worldPosition, int x, int y)
        :base(_type, _birthTime, _worldPosition, x, y)
    {
        classType = "Bush";

        if (type == "bush")
        {
            maxFruit = 15;
            maxStick = 10;
            maxLeaves = 20;
            leaves = maxLeaves;
            birthPercent = 2 * birthFactor;

            minTemp = 5;

            maxAge = new Date(25 * Date.Year);
            matureLevel = new Date(20 * Date.Day);
        }

        if (_birthTime.time < 0)
            fruit = Random.Range(0, maxFruit);
    }
}
