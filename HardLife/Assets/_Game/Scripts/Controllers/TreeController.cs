using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class TreeController : Controller<TreeModel> {

    internal int hour = 0;
    internal int day = 0;

    public Sprite child;
    public Sprite childFall;
    public Sprite childWinter;
    public Sprite young;
    public Sprite youngFall;
    public Sprite youngWinter;
    public Sprite matureNoFruit;
    public Sprite matureFruit;
    public Sprite matureFall;
    public Sprite matureWinter;
    public Sprite old;
    public Sprite oldFall;
    public Sprite oldWinter;

    internal MyGameManager gameManager;
    internal SpriteRenderer objectSprite;

    protected override void OnInitialize()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MyGameManager>();
        objectSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {
        
        if (model.localMap.Model.world.Model.date.hour != hour)
        {
            CreateObjectModel.UpdateAge(model, model.localMap.Model.world.Model.date);
            UpdateGrowth();
            ReplicatePlant();
        }

        if (model.updateTexture)
            SetSprite();
	
	}

    public void UpdateGrowth() //Hourly Update
    {
        if ( model.localMap.Model.curTemp < model.minTemp)
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
                if (model.state == State.Healthy)
                {
                    model.state = State.Browning;
                    model.leaves = model.maxLeaves / 2;
                    model.updateTexture = true;
                }
                else if (model.leaves < 1 && model.state == State.Browning)
                {
                    model.state = State.NoLeaves;
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

            if (model.state == State.Browning && model.leaves == model.maxLeaves)
            {
                model.state = State.Healthy;
                model.updateTexture = true;
            }
            else if (model.state == State.NoLeaves && model.leaves == model.maxLeaves)
            {
                model.state = State.Healthy;
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

        //Update growth stage

        if (model.age > model.youngAge && model.growthStage == GrowthStage.Child)
        {
            model.growthStage = GrowthStage.Young;
            model.updateTexture = true;
        }
        else if (model.age > model.matureAge && model.growthStage == GrowthStage.Young)
        {
            model.growthStage = GrowthStage.Mature;
            model.updateTexture = true;
        }
        else if (model.age > model.oldAge && model.growthStage == GrowthStage.Mature)
        {
            model.growthStage = GrowthStage.Old;
            model.updateTexture = true;
        }
    }

    internal void ReplicatePlant()
    {

        if (model.replicate)
        {
            model.replicate = false;
            Vector3 newWorldPosition = model.worldPostition + new Vector3(model.replicateLocation.x, model.replicateLocation.y);
            Coord coord = gameManager.LocalCoordFromWorldPosition(newWorldPosition);
            bool withinBorder = LocalMapGen.IsInLocalMapRange(model.localMap.Model, coord.x, coord.y);


            if (withinBorder)
            {
                bool clear = true;
                foreach (BaseObjectModel obj in LocalMapGen.AdjacentObjects(model.localMap.Model, coord.x, coord.y)) //checks for objects
                {
                    if (obj != null && obj.type == model.type)
                    {
                        clear = false;
                    }
                }

                if (clear)
                {
                    print("Should replicate");

                }
            }
        }

        if (model.updateTexture)
        {
            SetSprite();
            model.updateTexture = false;
        }
    }

    private void SetSprite()
    {
        if (model.growthStage == GrowthStage.Child)
        {
            if (model.state == State.Browning)
            {
                objectSprite.sprite = childFall;
            }
            else if (model.state == State.NoLeaves)
            {
                objectSprite.sprite = childWinter;
            }
            else
                objectSprite.sprite = child;
        }
        else if (model.growthStage == GrowthStage.Young)
        {
            if (model.state == State.Browning)
            {
                objectSprite.sprite = youngFall;
            }
            else if (model.state == State.NoLeaves)
            {
                objectSprite.sprite = youngWinter;
            }
            else
                objectSprite.sprite = young;
        }
        else if (model.growthStage == GrowthStage.Mature)
        {
            if (model.state == State.Browning)
            {
                objectSprite.sprite = matureFall;
            }
            else if (model.state == State.NoLeaves)
            {
                objectSprite.sprite = matureWinter;
            }
            else if (model.fruit == model.maxFruit)
            {
                objectSprite.sprite = matureFruit;
            }
            else
                objectSprite.sprite = matureNoFruit;
        }

    }
}
