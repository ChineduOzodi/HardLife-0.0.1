using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class TreeController : Controller<TreeModel> {

    protected override void OnInitialize()
    {
        Message.AddListener("Update Growth", UpdateGrowth);
    }

    // Update is called once per frame
    void Update () {
	
	}

    public static void UpdateGrowth() //Hourly Update
    {

        if (model.localMap.Model.curTemp < model.minTemp)
        {
            model.growthRate = 0;

            if (model.fruit > 0)
            {
                model.fruit -= UnityEngine.Random.Range(0, 2);
            }
            else
            {
                if (model.leaves > 0)
                    model.leaves -= UnityEngine.Random.Range(0, 2);
                if (model.state == "Normal")
                {
                    model.state = "Dying";
                    model.leaves = model.maxLeaves / 2;
                    model.updateTexture = true;
                }
                else if (model.leaves < 1 && model.state == "Dying")
                {
                    model.state = "Dead";
                    model.updateTexture = true;
                }
            }
        }
        else
        {
            model.growthRate = 1;

            //Update state text
            if (model.leaves < model.maxLeaves)
                model.leaves += UnityEngine.Random.Range(0, 2);

            if (model.state == "Dying" && model.leaves == model.maxLeaves)
            {
                model.state = "Normal";
                model.updateTexture = true;
            }
            else if (model.state == "Dead" && model.leaves == model.maxLeaves)
            {
                model.state = "Normal";
                model.updateTexture = true;
            }

            //update growth level and create fruit
            if (model.growthLevel < model.matureLevel)
                model.growthLevel.AddTime(Date.Hour * model.growthRate);
            else
            {
                if (model.leaves < model.maxLeaves)
                {

                }
                else
                {
                    if (model.fruit < model.maxFruit)
                    {
                        model.fruit += UnityEngine.Random.Range(0, 2);
                    }

                    if (!model.replicate)
                    {
                        model.replicate = (model.birthPercent * (model.fruit / (float)model.maxFruit) > UnityEngine.Random.Range(0f, 1f)) ? true : false; //Replication decision based on birthpercent, fruit count, and luck
                        if (model.replicate)
                        {
                            model.replicateLocation = UnityEngine.Random.insideUnitCircle * 3;
                            if (Vector2.SqrMagnitude(model.replicateLocation) < Vector2.SqrMagnitude(Vector2.one))
                            {
                                model.replicate = false;
                            }
                        }
                    }
                }



            }
        }

        //Tree Specific Part
        if (model.age > new Date(5 * Date.Year) && model.ageText == "Young")
        {
            model.ageText = "Mature";
            model.updateTexture = true;
        }
    }

}
