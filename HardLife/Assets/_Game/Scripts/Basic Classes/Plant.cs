using System;
using UnityEngine;

public class Plant: Alive
{
    internal float growthRate = 1;
    internal bool replicate = false;
    internal Vector2 replicateLocation;
    internal float birthPercent;
    internal static float birthFactor = .0011f; //This allows the replicate number to have a chance of landing that many times in one year

    internal int maxFruit;
    internal int fruit = 0;
    internal int maxLeaves;
    internal int leaves;

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

            if (fruit > 0)
            {
                fruit -= UnityEngine.Random.Range(0, 2);
            }
            else
            {
                if (leaves > 0)
                    leaves -= UnityEngine.Random.Range(0, 2);
                if (state == "Normal")
                {
                    state = "Dying";
                    leaves = maxLeaves / 2;
                    updateTexture = true;
                }
                else if (leaves < 1 && state == "Dying")
                {
                    state = "Dead";
                    updateTexture = true;
                }
            }
        }
        else
        {
            growthRate = 1;

            //Update state text
            if (leaves < maxLeaves)
                leaves += UnityEngine.Random.Range(0, 2);

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

            //update growth level and create fruit
            if (growthLevel < matureLevel)
                growthLevel.AddTime(Date.Hour * growthRate);
            else
            {
                if (leaves < maxLeaves)
                {

                }
                else
                {
                    if (fruit < maxFruit)
                    {
                        fruit += UnityEngine.Random.Range(0, 2);
                    }

                    if (!replicate)
                    {
                        replicate = (birthPercent * (fruit/(float)maxFruit) > UnityEngine.Random.Range(0f, 1f)) ? true : false; //Replication decision based on birthpercent, fruit count, and luck
                        if (replicate)
                        {
                            replicateLocation = UnityEngine.Random.insideUnitCircle * 3;
                            if (Vector2.SqrMagnitude(replicateLocation) < Vector2.SqrMagnitude(Vector2.one))
                            {
                                replicate = false;
                            }
                        }
                    }
                }
                    

                
            }
        }

        

    }

    public override string GetInfo()
    {
        return "Fruit: " + fruit + "\nState: " + state + "\nLeaves: " + leaves + "/" + maxLeaves
            + "\nMin. Temperature: " + Math.Round(minTemp,1) + " C\nReplicate: " + replicate
            + "\nGrowth Level: " + Mathf.FloorToInt(growthLevel.time/matureLevel.time * 100)
            + " %\nGrowth Rate: " + growthRate + "\n" + base.GetInfo();
    }
}