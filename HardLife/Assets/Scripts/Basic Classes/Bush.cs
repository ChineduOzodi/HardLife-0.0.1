using UnityEngine;
using System.Collections;

public class Bush : Plant
{
    internal int maxStick;
    internal int maxFruit;
    internal int fruit;

    public Bush(string _type, Date _birthTime, Vector3 _worldPosition, int x, int y)
        :base(_type, _worldPosition, x, y)
    {
        
        birthTime = _birthTime;
        classType = "Bush";

        if (type == "bush")
        {
            maxFruit = 15;
            maxStick = 10;

            minTemp = 5;

            maxAge = new Date(25 * Date.Year);
            
            fruit = UnityEngine.Random.Range(0, maxFruit);

        }
    }
}
