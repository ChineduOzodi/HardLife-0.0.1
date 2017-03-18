using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LocalMapController : MonoBehaviour {

    public Text localMapText;
    public Text statisticsText;

    public GameObject oakTree;
    public GameObject bush;
    public GameObject infoBox;

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
            localMapText.text = "<b>" + model.region + "</b>  ||  " + world.date.GetDateTime() + "  ||  Temperature: " + Math.Round(model.curTemp, 1) + " C";

            if (selectedObject != null)
            {
                
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
                int index = ArrayHelper.ElementIndex(x, y, world.localSizeY);

                if (model.objectMap[index] != null)
                {
                    SpriteRenderer instance;
                    if (model.objectMap[index].name == "oak tree" || model.objectMap[index].name == "bush")
                    {
                        TreeController tree = Instantiate(oakTree, objectMapEmpty.transform).GetComponent<TreeController>();
                        tree.model = (TreeModel)model.objectMap[index];
                        instance = tree.GetComponent<SpriteRenderer>();
                    }
                    else
                        instance = CreateObject(model.objectMap[index], x,y, objectMapEmpty.transform);
                    instance.sortingOrder = model.objectMap[index].renderOrder + y;
                    instance.transform.position = model.objectMap[index].worldPostition;
                    objectMap[x,y] = instance;
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
                int index = ArrayHelper.ElementIndex(x, y, world.localSizeY);
                baseMap[x,y] = CreateObject(model.baseMap[index], x,y, baseMapEmpty.transform);
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
            if (model.objectMap[ArrayHelper.ElementIndex(coord.x, coord.y,world.localSizeY)] != null)
            {
                objectMap[coord.x, coord.y].color = new Color(.5f, .5f, .5f);

                selectedTile = objectMap[coord.x, coord.y];
                selectedObject = model.objectMap[ArrayHelper.ElementIndex(coord.x, coord.y,world.localSizeY)];

                InfoPanel obj = Instantiate(infoBox, transform.parent).GetComponent<InfoPanel>();

                obj.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(selectedObject.worldPostition);
            }
            else
            {
                baseMap[coord.x, coord.y].color = new Color(.5f, .5f, .5f);

                selectedTile = baseMap[coord.x, coord.y];
                selectedObject = null;
            }

            

        }


    }

    public SpriteRenderer CreateObject(BaseObjectModel name, int x, int y, Transform parent = null)
    {
        Vector3 worldPoint = model.worldBottomLeft + Vector3.right * (x + .5f) + Vector3.up * (y + .5f);

        if (objectQueue.Count == 0)
        {
            GameObject obj = new GameObject(name.type.ToString());
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
            instance.name = name.type.ToString();
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

    public void Save()
    {
        //try
        //{
        //    savePath = Application.persistentDataPath + "/World/" + localMap.world.worldName + "_Auto Save.sav";//"worldGen.world.saveNum"
        //}
        //catch (DirectoryNotFoundException)
        //{
        //    Directory.CreateDirectory(Application.persistentDataPath + "/World/");
        //    savePath = Application.persistentDataPath + "/World/" + worldGen.world.worldName + "_Auto Save.sav";//"worldGen.world.saveNum"
        //}

        string savePath = "Saves";
        WorldModel world = new WorldModel();
        world.name = "Hello";
        world.localSizeX = 200;
        world.localSizeY = 200;
        world.worldSize = new Vector2(200, 120);
        world.currentLocalMap = new ModelRef<LocalMapModel>(model);
        Model.Save(savePath, model);
    }

}
