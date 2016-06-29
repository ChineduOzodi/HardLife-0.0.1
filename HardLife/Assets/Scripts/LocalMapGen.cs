using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LocalMapGen : MonoBehaviour {

    public Text localMapText;
    public Text objectNameText;
    public Text objectInfoText;
    public Text statisticsText;

    World world;
    internal LocalMap local;
    internal MyGameManager gameManager;

    GameObject baseMapEmpty;
    GameObject objectMapEmpty;

    SpriteRenderer[,] baseMap;
    SpriteRenderer[,] objectMap;
    internal SpriteRenderer selectedTile;
    private bool tileSelected;
    private GObject selectedObject;

    internal Queue<SpriteRenderer> objectQueue = new Queue<SpriteRenderer>();

    // Use this for initialization
    void Awake () {

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MyGameManager>();
        world = gameManager.world;
        local = world.localMap;

        CreateLocalMap();

    }
	
	// Update is called once per frame
	void Update () {
        if (!gameManager.setup)
        {
            world.date.AddTime(Time.deltaTime * gameManager.gameSpeed); //Update Time
            localMapText.text = "<b>" + world.localMap.region + "</b>\n" + world.date.GetDateTime() + "\nTemperature: " + Math.Round(world.localMap.curTemp, 1) + " C";

            if (selectedObject != null)
            {
                objectNameText.text = selectedObject.type.ToUpper();
                objectInfoText.text = selectedObject.GetInfo();
            }


            if (Input.GetMouseButtonDown(0)) //Mouse Left Click
            {
                LeftMouseDown();
            }
            if (Input.GetKeyDown(",") && gameManager.gameSpeed > 1)
            {
                gameManager.GameSpeedChange(.5f);

            }
            if (Input.GetKeyDown(".") && gameManager.gameSpeed < 20)
            {
                gameManager.GameSpeedChange(2);
            }
        }
    }

    public void CreateLocalMap()
    {
        DestroyLocalMap();

        int x = world.localMap.worldMapPositionX;
        int y = world.localMap.worldMapPositionY;

        string seed = world.localMap.seed;

        //Camera Setup
        Camera.main.orthographicSize = gameManager.worldSize.x / 2f;
        gameManager.maxCamSize = gameManager.worldSize.y / 2f;
        Camera.main.transform.position = new Vector3(0, 0, -10f);

        //Generate LocalMap if not generated
        if (world.localMap.lastUpdated < 1)
        {
            world.localMap.GenerateLocalMap(world);
            world.localMap.lastUpdated = 1;
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
                if (world.localMap.objectMap[x, y] != null)
                {
                    SpriteRenderer instance = CreateObject(local.objectMap[x, y], x, y,objectMapEmpty.transform);
                    instance.sortingOrder = world.localMap.objectMap[x, y].renderOrder + y;
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
                baseMap[x, y] = CreateObject(local.baseMap[x, y], x, y, baseMapEmpty.transform);
            }
        }
    }

    public SpriteRenderer CreateObject(GObject name,int x, int y, Transform parent = null)
    {
        Vector3 worldPoint = world.localMap.worldBottomLeft + Vector3.right * (x * world.nodeDiameter + world.nodeRadius) + Vector3.up * (y * world.nodeDiameter + world.nodeRadius);

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
            if (world.localMap.objectMap[coord.x, coord.y] != null)
            {
                objectMap[coord.x, coord.y].color = new Color(.5f, .5f, .5f);

                selectedTile = objectMap[coord.x, coord.y];
                selectedObject = local.objectMap[coord.x, coord.y];         
            }
            else
            {
                baseMap[coord.x, coord.y].color = new Color(.5f, .5f, .5f);

                selectedTile = baseMap[coord.x, coord.y];
                selectedObject = local.objectMap[coord.x, coord.y];
            }

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

    internal void UpdateAge()
    {
        //Currently using update growth to update age

        //foreach (GObject item in local.objectMap)
        //{
        //    if (item != null && item.classType  == "Tree")
        //    {
        //        Tree tree = (Tree)item;

        //        tree.UpdateAge(world.date);
        //    }
        //}
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

    internal void UpdateTemperature(LocalMap localMap)
    {
        int yearTempRange = 7;
        int dayTempRange = 3;

        localMap.curTemp = localMap.aveTemp - yearTempRange * Mathf.Cos(5/(Mathf.PI * 2)) -yearTempRange * Mathf.Cos((localMap.world.date.day + 5) / (2 * Mathf.PI))  - dayTempRange * Mathf.Cos((localMap.world.date.hour) * Mathf.PI / 12);

    }

    internal void UpdatePlantGrowth()
    {
        foreach (GObject item in local.objectMap)
        {
            if (item != null )
            {
                if (item.classType == "Tree" || item.classType == "Bush")
                {
                    Plant plant = (Plant)item;

                    plant.UpdateGrowth(world.localMap.curTemp);
                    plant.UpdateAge(world.date);

                    if (plant.replicate)
                    {
                        Vector3 newWorldPosition = plant.worldPostition + new Vector3(plant.replicateLocation.x, plant.replicateLocation.y);
                        Coord coord = gameManager.LocalCoordFromWorldPosition(newWorldPosition);
                        bool withinBorder = local.IsInLocalMapRange(coord.x,coord.y);


                        if (withinBorder)
                        {
                            bool clear = true;
                            foreach (GObject obj in local.AdjacentObjects(coord.x, coord.y)) //checks for objects
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
                                    local.objectMap[coord.x, coord.y] = newTree;
                                    objectMap[coord.x, coord.y] = CreateObject(newTree, coord.x, coord.y, objectMapEmpty.transform);
                                }
                                else if (item.classType == "Bush")
                                {
                                    Bush newBush = new Bush(plant.type, world.date, newWorldPosition, coord.x, coord.y);
                                    local.objectMap[coord.x, coord.y] = newBush;
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
}
