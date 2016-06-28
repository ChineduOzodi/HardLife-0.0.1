using UnityEngine;
using System.Collections;

public class Bush : Plant
{
    internal int maxStick;
    internal int maxLeaves;
    internal int leaves;
    internal int maxFruit;
    internal int fruit = 0;

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

            minTemp = 5;

            maxAge = new Date(25 * Date.Year);
            matureLevel = new Date(20 * Date.Day);
        }

        if (_birthTime.time < 0)
            fruit = Random.Range(0, maxFruit);
    }
    public override void UpdateGrowth(float temp) //Hourly Update
    {
        base.UpdateGrowth(temp);

        if (temp < minTemp) //If below min temperature
        {
            if (fruit > 0)
            {
                fruit -= 1;  
            }
            else
            {
                if (leaves > 0)
                    leaves-= Random.Range(0,2);
                if (state == "Normal")
                {
                    state = "Dying";
                    updateTexture = true;
                }else if (leaves < 1 && state == "Dying")
                {
                    state = "Dead";
                    updateTexture = true;
                }
            }
        }
        else
        {
            if (leaves < maxLeaves)
                leaves += Random.Range(0, 2);

            if (state == "Dying" && leaves == maxLeaves)
            {
                state = "Normal";
                updateTexture = true;
            }
            else if (state == "Dead" && leaves == maxLeaves)
            {
                state = "Normal";
                updateTexture = true;
            }
            else if (growthLevel >= matureLevel && fruit < maxFruit)
            {
                fruit += Random.Range(0, 2);
            }

        }
    }
    public override string GetInfo()
    {
        return "Fruit: " + fruit + "\nAtate: " + state + "\nLeaves: " + leaves + "/" + maxLeaves+"\n" + base.GetInfo();
    }
}
