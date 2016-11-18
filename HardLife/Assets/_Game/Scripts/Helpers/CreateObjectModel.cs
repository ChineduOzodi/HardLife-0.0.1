using UnityEngine;
using System.Collections;
using System;

public struct CreateObjectModel {

    public ItemsModel SetItemsModel(ItemsModel model, string _type, Vector3 _worldPosition, int x, int y)
    {
        SetBaseObjectModel(model, _type, _worldPosition, x, y);

        return model;
    }

    public TreeModel SetTreeModel(TreeModel model, string _type, Date _birthTime, Vector3 _worldPosition, int x, int y)
    {
        model.classType = "Tree";

        SetPlantModel(model, _type, _birthTime, _worldPosition, x, y);

        if (model.type == "oak tree")
        {
            model.renderOrder = 2;
            model.minTemp = 5;
            model.maxFruit = 35;
            model.maxWood = 75;
            model.maxStick = 20;

            model.maxLeaves = 40;
            model.leaves = model.maxLeaves;

            model.birthPercent = 3 * model.birthFactor;

            model.maxAge = new Date(250 * Date.Year);
            model.matureLevel = new Date(3 * Date.Year);
        }
        else if (model.type == "bush")
        {
            model.maxFruit = 15;
            model.maxStick = 10;
            model.maxLeaves = 20;
            model.leaves = model.maxLeaves;
            model.birthPercent = 2 * model.birthFactor;

            model.minTemp = 5;

            model.maxAge = new Date(25 * Date.Year);
            model.matureLevel = new Date(20 * Date.Day);
        }

        if (_birthTime.time < 0)
            model.fruit = UnityEngine.Random.Range(0, model.maxFruit);

        return model;
    }

    public PlantModel SetPlantModel (PlantModel model, string _type, Date _birthTime, Vector3 _worldPosition, int x, int y)
    {
        SetAliveModel(model, _type, _birthTime, _worldPosition, x, y);
        return model;
    }
    public AliveModel SetAliveModel(AliveModel model, string _type, Date _birthTime, Vector3 _worldPosition, int x, int y)
    {
        SetBaseObjectModel(model, _type, _worldPosition, x, y);
        model.birthTime = _birthTime;

        return model;
    }

    public TileModel CreateTileModel(Vector3 _worldPosition, int x, int y, int _id, string _type = null)
    {
        TileModel model = new TileModel();

        model.id = _id;

        SetBaseObjectModel(model, _type, _worldPosition, x, y);

        return model;
    }
    public static BaseObjectModel SetBaseObjectModel(BaseObjectModel model, string _type, Vector3 _worldPosition, int x, int y)
    {
        model.type = _type;

        model.worldPostition = _worldPosition;
        model.localMapPositionX = x;
        model.localMapPositionY = y;

        return model;

    }

    public string GetInfo(PlantModel model)
    {
        return "Fruit: " + model.fruit + "\nState: " + model.state + "\nLeaves: " + model.leaves + "/" + model.maxLeaves
            + "\nMin. Temperature: " + Math.Round(model.minTemp, 1) + " C\nReplicate: " + model.replicate
            + "\nGrowth Level: " + Mathf.FloorToInt(model.growthLevel.time / model.matureLevel.time * 100)
            + " %\nGrowth Rate: " + model.growthRate + "\n" + GetInfo((AliveModel) model);
    }

    public static string GetInfo(AliveModel model)
    {
        return "Age: " + model.age.GetDate() + "\nAlive: " + model.isAlive + "\n" + GetInfo((BaseObjectModel) model);
    }
    public static string GetInfo(BaseObjectModel model)
    {
        return "Class: " + model.classType;
    }

    public static void UpdateAge(AliveModel model, Date _currentTime)
    {
        model.age = _currentTime - model.birthTime;
    }
    
}
