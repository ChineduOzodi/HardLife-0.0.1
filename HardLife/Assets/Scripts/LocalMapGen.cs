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

    World world;
    internal LocalMap local;
    internal GameManager gameManager;

    GameObject baseMapEmpty;
    GameObject itemMapEmpty;

    SpriteRenderer[,] baseMap;
    SpriteRenderer[,] objectMap;
    SpriteRenderer selectedTile;
    private bool tileSelected;

    // Use this for initialization
    void Awake () {

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        world = gameManager.world;
        local = world.localMap;

        CreateLocalMap();

    }
	
	// Update is called once per frame
	void Update () {

	}

    public void CreateLocalMap()
    {

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
        itemMapEmpty = new GameObject("ItemMap");
        objectMap = new SpriteRenderer[world.localSizeX, world.localSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * world.localSize.x / 2 - Vector3.up * world.localSize.y / 2;

        for (int x = 0; x < world.localSizeX; x++)
        {
            for (int y = 0; y < world.localSizeY; y++)
            {
                if (world.localMap.objectMap[x, y] != null)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * world.nodeDiameter + world.nodeRadius) + Vector3.up * (y * world.nodeDiameter + world.nodeRadius);
                    GameObject tile = new GameObject("ItemTile");
                    tile.transform.position = worldPoint;
                    SpriteRenderer instance = tile.AddComponent<SpriteRenderer>();
                    instance.sprite = gameManager.spriteManager.GetSprite(world.localMap.objectMap[x, y]);
                    world.localMap.objectMap[x, y].worldPostition = worldPoint;
                    instance.transform.SetParent(itemMapEmpty.transform);
                    instance.sortingOrder = world.localMap.objectMap[x, y].stackOrder;
                    objectMap[x, y] = instance;
                }  
            }
        }
    }

    public void BuildBaseMap()
    {
        //throw new NotImplementedException();
        //DestroyLocalMap();

        baseMapEmpty = new GameObject("BaseMap");
        baseMap = new SpriteRenderer[world.localSizeX, world.localSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * world.localSize.x / 2 - Vector3.up * world.localSize.y / 2;

        for (int x = 0; x < world.localSizeX; x++)
        {
            for (int y = 0; y < world.localSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * world.nodeDiameter + world.nodeRadius) + Vector3.up * (y * world.nodeDiameter + world.nodeRadius);
                GameObject tile = new GameObject("BaseTile");
                tile.transform.position = worldPoint;
                SpriteRenderer instance = tile.AddComponent<SpriteRenderer>();
                instance.sprite = gameManager.spriteManager.GetSprite(world.localMap.baseMap[x,y].type);
                instance.transform.SetParent(baseMapEmpty.transform);
                baseMap[x, y] = instance;
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
            if (world.localMap.objectMap[coord.x, coord.y] != null)
            {
                objectMap[coord.x, coord.y].color = new Color(.5f, .5f, .5f);

                selectedTile = objectMap[coord.x, coord.y];

                objectNameText.text = world.localMap.objectMap[coord.x, coord.y].type.ToUpper();
                objectInfoText.text = world.localMap.objectMap[coord.x, coord.y].GetInfo();
            }
            else
            {
                baseMap[coord.x, coord.y].color = new Color(.5f, .5f, .5f);

                selectedTile = baseMap[coord.x, coord.y];

                objectNameText.text = world.localMap.baseMap[coord.x, coord.y].type.ToUpper();
                objectInfoText.text = world.localMap.baseMap[coord.x, coord.y].GetInfo();
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
        foreach (GObject item in local.objectMap)
        {
            if (item != null && item.classType  == "Tree")
            {
                Tree tree = (Tree)item;

                tree.UpdateAge(world.date);
            }
        }
    }

    private void DestroyLocalMap()
    {
        throw new NotImplementedException();
        //if (layers != null)
        //{
        //    foreach (Transform layer in layers.Values)
        //    {
        //        Destroy(layer.gameObject);
        //    }
        //}
    }
}
