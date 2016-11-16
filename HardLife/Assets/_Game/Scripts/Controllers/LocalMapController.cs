using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LocalMapController : MonoBehaviour {

    public GameObject infoBoxPrefab;

    public Text localMapText;
    public Text objectNameText;
    public Text objectInfoText;
    public Text statisticsText;

    WorldModel world;
    internal LocalMapModel model;
    internal MyGameManager gameManager;

    PlayMakerFSM updateFSM;
    public int hour;
    public int day;

    GameObject baseMapEmpty;
    GameObject objectMapEmpty;

    SpriteRenderer[,] baseMap;
    SpriteRenderer[,] objectMap;
    internal SpriteRenderer selectedTile;
    private bool tileSelected;
    private BaseObjectModel selectedObject;

    internal Queue<SpriteRenderer> objectQueue = new Queue<SpriteRenderer>();

    // Use this for initialization
   void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MyGameManager>();
        world = gameManager.world;
        model = world.currentLocalMap.Model;
        updateFSM = GetComponent<PlayMakerFSM>();
        hour = world.date.hour;
        day = world.date.day;
        CreateLocalMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.setup)
        {
            world.date.AddTime(Time.deltaTime * gameManager.gameSpeed); //Update Time
            localMapText.text = "<b>" + model.region + "</b>\n" + world.date.GetDateTime() + "\nTemperature: " + Math.Round(model.curTemp, 1) + " C";

            if (selectedObject != null)
            {
                objectNameText.text = selectedObject.type.ToUpper();
                objectInfoText.text = selectedObject.GetInfo();
            }


            if (Input.GetMouseButtonDown(0)) //Mouse Left Click
            {
                LeftMouseDown();
            }

            //Create Update Events for each day and each hour

            if (world.date.day != day)
            {
                day = world.date.day;
                updateFSM.SendEvent("DayUpdate");

            }
            else if (world.date.hour != hour)
            {
                hour = world.date.hour;
                updateFSM.SendEvent("HourUpdate");
            }

        }
    }

    public void CreateLocalMap()
    {
        DestroyLocalMap();

        int x = model.worldMapPositionX;
        int y = model.worldMapPositionY;

        string seed = model.seed;

        //Camera Setup
        Camera.main.orthographicSize = gameManager.worldSize.x / 2f;
        Camera.main.GetComponent<CameraControl>().maxCamSize = gameManager.worldSize.y / 2f;
        Camera.main.transform.position = new Vector3(0, 0, -10f);

        //Generate LocalMap if not generated
        if (model.lastUpdated < 1)
        {
            LocalMapGen.GenerateLocalMap(model);
            model.lastUpdated = 1;
        }

        BuildBaseMap();
        BuildObjectMap();
        gameManager.setup = false;
    }

    private void BuildObjectMap()
    {
        objectMapEmpty = new GameObject("ObjectMap");
        objectMap = new SpriteRenderer[world.localSizeX, world.localSizeY];

        for (int x = 0; x < world.localSizeX; x++)
        {
            for (int y = 0; y < world.localSizeY; y++)
            {
                if (model.objectMap[x, y] != null)
                {
                    SpriteRenderer instance = CreateObject(model.objectMap[x, y], x, y, objectMapEmpty.transform);
                    instance.sortingOrder = model.objectMap[x, y].renderOrder + y;
                    objectMap[x, y] = instance;
                }
            }
        }
    }

    public void BuildBaseMap()
    {

        baseMapEmpty = new GameObject("BaseMap");
        baseMap = new SpriteRenderer[world.localSizeX, world.localSizeY];



        for (int x = 0; x < world.localSizeX; x++)
        {
            for (int y = 0; y < world.localSizeY; y++)
            {
                baseMap[x, y] = CreateObject(model.baseMap[x, y], x, y, baseMapEmpty.transform);
            }
        }
    }

    public void LeftMouseDown()
    {

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedTile != null)
            {
                selectedTile.color = new Color(1f, 1f, 1f);
            }
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Coord coord = gameManager.LocalCoordFromWorldPosition(worldPosition);
            if (model.objectMap[coord.x, coord.y] != null)
            {
                objectMap[coord.x, coord.y].color = new Color(.5f, .5f, .5f);

                selectedTile = objectMap[coord.x, coord.y];
                selectedObject = model.objectMap[coord.x, coord.y];
            }
            else
            {
                baseMap[coord.x, coord.y].color = new Color(.5f, .5f, .5f);

                selectedTile = baseMap[coord.x, coord.y];
                selectedObject = model.objectMap[coord.x, coord.y];
            }

        }


    }

    public SpriteRenderer CreateObject(BaseObjectModel name, int x, int y, Transform parent = null)
    {
        Vector3 worldPoint = model.worldBottomLeft + Vector3.right * (x + .5f) + Vector3.up * (y + .5f);

        if (objectQueue.Count == 0)
        {
            GameObject obj = new GameObject(name.type);
            obj.transform.position = worldPoint;
            SpriteRenderer instance = obj.AddComponent<SpriteRenderer>();
            instance.sprite = gameManager.spriteManager.GetSprite(name);
            if (parent != null)
                instance.transform.SetParent(baseMapEmpty.transform);
            return instance;
        }
        else
        {
            SpriteRenderer instance = objectQueue.Dequeue();
            instance.transform.position = worldPoint;
            instance.name = name.type;
            instance.sprite = gameManager.spriteManager.GetSprite(name);
            if (parent != null)
                instance.transform.SetParent(baseMapEmpty.transform);
            return instance;
        }
    }

    void ToggleTileSelected() //Currently doesn't have function
    {

        //Previously TOggleTileSelected
        if (tileSelected)
        {
            //createLocalMapButton.interactable = false;
            //tileSelected = false;
        }
        else
        {
            //createLocalMapButton.interactable = true;
            tileSelected = true;
        }

    }

    private void DestroyLocalMap()
    {
        if (baseMapEmpty != null)
        {
            Destroy(baseMapEmpty);
        }
        if (objectMapEmpty != null)
        {
            Destroy(objectMapEmpty);
        }
    }

    internal void UpdateTemperature()
    {
        int yearTempRange = 7;
        int dayTempRange = 3;

        model.curTemp = model.aveTemp - yearTempRange * Mathf.Cos(5 / (Mathf.PI * 2)) - yearTempRange * Mathf.Cos((world.date.day + 5) / (2 * Mathf.PI)) - dayTempRange * Mathf.Cos((world.date.hour) * Mathf.PI / 12);

    }

    internal void UpdatePlantGrowth()
    {
        foreach (BaseObjectModel item in model.objectMap)
        {
            if (item != null)
            {
                if (item.classType == "Tree" || item.classType == "Bush")
                {
                    Plant plant = (Plant)item;

                    plant.UpdateGrowth(model.curTemp);
                    plant.UpdateAge(world.date);

                    if (plant.replicate)
                    {
                        Vector3 newWorldPosition = plant.worldPostition + new Vector3(plant.replicateLocation.x, plant.replicateLocation.y);
                        Coord coord = gameManager.LocalCoordFromWorldPosition(newWorldPosition);
                        bool withinBorder = LocalMapGen.IsInLocalMapRange(model, coord.x, coord.y);


                        if (withinBorder)
                        {
                            bool clear = true;
                            foreach (BaseObjectModel obj in LocalMapGen.AdjacentObjects(model, coord.x, coord.y)) //checks for objects
                            {
                                if (obj != null && obj.type == plant.type)
                                {
                                    clear = false;
                                }
                            }

                            if (clear)
                            {
                                if (item.classType == "Tree")
                                {
                                    Tree newTree = new Tree(plant.type, world.date, newWorldPosition, coord.x, coord.y);
                                    model.objectMap[coord.x, coord.y] = newTree;
                                    objectMap[coord.x, coord.y] = CreateObject(newTree, coord.x, coord.y, objectMapEmpty.transform);
                                }
                                else if (item.classType == "Bush")
                                {
                                    Bush newBush = new Bush(plant.type, world.date, newWorldPosition, coord.x, coord.y);
                                    model.objectMap[coord.x, coord.y] = newBush;
                                    objectMap[coord.x, coord.y] = CreateObject(newBush, coord.x, coord.y, objectMapEmpty.transform);
                                }

                            }
                        }

                        plant.replicate = false;
                    }
                }

                if (item.updateTexture)
                {
                    objectMap[item.localMapPositionX, item.localMapPositionY].sprite = gameManager.spriteManager.GetSprite(item);
                    item.updateTexture = false;
                }
            }
        }
    }

    internal string CompileStats()
    {
        int treeCount = 0;
        int bushCount = 0;

        for (int x = 0; x < model.localSizeX; x++)
        {
            for (int y = 0; y < model.localSizeY; y++)
            {
                if (model.biome != "Water") //water local map
                {
                    if (model.objectMap[x, y] != null)
                    {
                        if (model.objectMap[x, y].type == "oak tree")
                        {
                            treeCount++;
                        }
                        else if (model.objectMap[x, y].type == "bush")
                        {
                            bushCount++;
                        }

                    }
                }

            }
        }
        return "Oak Trees: " + treeCount + "\nBushes: " + bushCount;
    }
}
