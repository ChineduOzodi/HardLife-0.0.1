﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class WorldGen : MonoBehaviour {

	public bool useRandomSeed = true;
    public LocalMapGen localMapGen;
    public GameObject whiteBlock;
    private Sprite[] water;
    private Sprite[] ice;
    private Sprite[] grass;
    private Sprite[] jungle;
    private Sprite[] desert;
    private Sprite[] hill;
    private Sprite[] mountain;
    public string[,] regionNames;

    public Canvas createWorldMenu;
    public Text infoText;
    public Text worldNameText;
    public InputField seedInput;
    public Button createLocalMapButton;
    bool tileSelected = false;
    Coord selectedTile;

    public WorldTile[,] tiles;
    string[] tType = { "Flat", "Hills", "Mountains" };
    string[] aveRain = { "Little", "Normal", "Lots" };
    private int tempYearRange = 10;
    private int maxLakeSize;
    private int maxIslandSize;
    private GameObject[][] biomeSprites;
    public Dictionary<string, Transform> layers = new Dictionary<string, Transform> { };
    public Camera mainCam;
    public GameManager gameManager;

    void Awake()
    {
        localMapGen = gameObject.GetComponent<LocalMapGen>();
        gameManager = gameObject.GetComponent<GameManager>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        biomeSprites = new GameObject[][] { water, ice, grass, jungle, desert };
        maxLakeSize = (gameManager.world.width * gameManager.world.height) / 200;
		maxIslandSize = (gameManager.world.width * gameManager.world.height) / 1000;

    }
	void OnMouseEnter()
	{
		if (!tileSelected && !EventSystem.current.IsPointerOverGameObject())
		{
			gameManager.SendMessage("SendInfo", coord);
			//print("Found Mouse");
			//print(coord);
			//gameObject.transform.localScale = new Vector3(1.25f, 1.25f);
			gameObject.GetComponent<SpriteRenderer>().color = new Color(.8f, .8f, .8f);
		}

	}
	void OnMouseExit()
	{
		if (!tileSelected && !EventSystem.current.IsPointerOverGameObject())
		{
			gameObject.transform.localScale = new Vector3(1, 1);
			gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
		}

	}
	void OnMouseDown()
	{

		if (!EventSystem.current.IsPointerOverGameObject())
		{
			gameObject.transform.localScale = new Vector3(1, 1);
			gameObject.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
			gameManager.worldGen.layers["Biomes"].BroadcastMessage("ToggleTileSelected");
			gameManager.worldGen.SendMessage("ToggleTileSelected");
			//gameObject.GetComponentInParent<Transform>().BroadcastMessage("ToggleTileSelected");
		}


	}

    public void CreateWorld()
    {
        DestroyWorld();

        mainCam.orthographicSize = gameManager.world.height / 2f;
        gameManager.maxCamSize = gameManager.world.height / 2f;
        mainCam.transform.position = new Vector3(gameManager.world.width / 2, gameManager.world.height / 2, -10f);

        if (gameManager.world.useRandomSeed)
        {
            gameManager.world.seed = Time.time.ToString();
            seedInput.text = gameManager.world.seed;
        }
        else
            gameManager.world.seed = seedInput.text;

        gameManager.world.GenerateMap();
        GenerateRegions();
        seedInput.text = gameManager.world.seed; // just in case GenerateMap changes the gameManager.world.seed

        worldNameText.text = gameManager.world.worldName;
        //buildBasicLayers();
        buildBiome();
        buildMountains();
    }
    public void loadWorld()
    {

        mainCam.orthographicSize = gameManager.world.height / 2f;
        gameManager.maxCamSize = gameManager.world.height / 2f;
        mainCam.transform.position = new Vector3(gameManager.world.width / 2, gameManager.world.height / 2, -10f);

        GenerateRegions();
        seedInput.text = gameManager.world.seed; // just in case GenerateMap changes the gameManager.world.seed

        worldNameText.text = gameManager.world.worldName;
        //buildBasicLayers();
        buildBiome();
        buildMountains();
    }

    public void PreviewWorld(string layerName, int max = 1)
    {
        int[,] map = gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, layerName)];
        layers["gameManager.world"] = new GameObject("gameManager.world").transform;

        mainCam.orthographicSize = gameManager.world.height / 2f;
        mainCam.transform.position = new Vector3(gameManager.world.width / 2, gameManager.world.height / 2, -10f);

        for (int x = 0; x < gameManager.world.width; x++)
        {
            for (int y = 0; y < gameManager.world.height; y++)
            {
                float num = (float)map[x, y] / (float)max;
                Color col = new Color(num, num, num);
                SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
                rend.color = col;

                GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

                instance.transform.SetParent(layers["gameManager.world"]);
            }
        }

    }
    /// <summary>
    /// Preview Map with the string name
    /// </summary>
    /// <param name="name"></param>
    public void PreviewWorld()
    {

        mainCam.orthographicSize = gameManager.world.height / 2f;
        mainCam.transform.position = new Vector3(gameManager.world.width / 2, gameManager.world.height / 2, -10f);

        buildBasicLayers();
        buildBiome();
        buildMountains();
    }

    public void CreateLocalMap()
    {
        gameManager.world.localMaps[selectedTile.x, selectedTile.y] = localMapGen.CreateLocalMap(selectedTile);
        layers["Biomes"].gameObject.SetActive(false);
        layers["Mountains"].gameObject.SetActive(false);
        localMapGen.buildBaseMap(gameManager.world.localMaps[selectedTile.x, selectedTile.y]);
    } 

    private void buildMountains()
    {
        layers["Mountains"] = new GameObject("Mountains").transform;
        int[,] map = gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Mountain Map")];
        for (int x = 0; x < gameManager.world.width; x++)
        {
            for (int y = 0; y < gameManager.world.height; y++)
            {
                if (map[x,y] == 1)
                {
                    int num = UnityEngine.Random.Range(0, hill.Length);
                    GameObject tile = hill[num].gameObject;
                    GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(layers["Mountains"]);
                }
                else if(map[x,y] == 2)
                {
                    int num = UnityEngine.Random.Range(0, mountain.Length);
                    GameObject tile = mountain[num].gameObject;
                    GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(layers["Mountains"]);
                }
            }
        }
    }

    void SendInfo(int[] coord)
    {
        selectedTile = new Coord(coord[0], coord[1]);
        string terrain = tType[gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Mountain Map")][coord[0], coord[1]]];
        string rain = aveRain[gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Rain Map")][coord[0], coord[1]]];
        string info = "Region Name: " + regionNames[coord[0], coord[1]];
        info += "\nTerrain Type: " + terrain;
        info += "\nAverage Temperature: " + gameManager.world.aveTempMap[coord[0], coord[1]] + " C (" + (gameManager.world.aveTempMap[coord[0], coord[1]] - tempYearRange) + " - " + (gameManager.world.aveTempMap[coord[0], coord[1]] + tempYearRange) + ")";
        info += "\nAverage Rain: " + rain;
        infoText.text = info;
    }

    void ToggleTileSelected()
    {
        if (tileSelected)
        {
            createLocalMapButton.interactable = false;
            gameObject.transform.localScale = new Vector3(1, 1);
            tileSelected = false;
        }
        else
        {
            createLocalMapButton.interactable = true;
            tileSelected = true;
        }
            
    }

    private void buildBiome()
    {
        layers["Biomes"] = new GameObject("Biomes").transform;
        int[,] map = gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Biome Map")];

        for (int x = 0; x < gameManager.world.width; x++)
        {
            for (int y = 0; y < gameManager.world.height; y++)
            {
                int sprite = map[x,y];
                int num = UnityEngine.Random.Range(0, biomeSprites[sprite].Length);
                GameObject tile = biomeSprites[sprite][num].gameObject;
                GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;

                instance.transform.SetParent(layers["Biomes"]);
                instance.AddComponent<WorldTile>();
                instance.GetComponent<WorldTile>().SetupTile(gameManager, x,y,gameManager.world.seed);
            }
        }
        //layers["Biomes"].gameObject.SetActive(false);
    }

    private void buildBasicLayers()
    {
        for (int i = 0; i < gameManager.world.layerNames.Length; i++)
        {
            int[,] map = gameManager.world.mapLayers[i];
            int max = 2;

            layers[gameManager.world.layerNames[i]] = new GameObject(gameManager.world.layerNames[i]).transform;

            for (int x = 0; x < gameManager.world.width; x++)
            {
                for (int y = 0; y < gameManager.world.height; y++)
                {
                    float num = (float)map[x, y] / (float)max;
                    Color col = new Color(num, num, num);
                    SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
                    rend.color = col;

                    GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

                    instance.transform.SetParent(layers[gameManager.world.layerNames[i]]);
                }
            }

            layers[gameManager.world.layerNames[i]].gameObject.SetActive(false);

        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void GenerateRegions()
    {
        tiles = new WorldTile[gameManager.world.width, gameManager.world.height];
        int[,] baseMap = gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Base Map")];
        regionNames = new string[gameManager.world.width, gameManager.world.height];
        //Generate Land and Island Regions
        List<Region> regions = GetRegions(baseMap, 1);
        regions.AddRange(GetRegions(baseMap, 0));

        foreach (Region region in regions)
        {
            if (region.tileType == 1)
            {
                if (region.tiles.Count <= maxIslandSize)
                {
                    region.name += " Island";
                }

            }
            else if (region.tileType == 0)
            {
                if (region.tiles.Count <= maxLakeSize)
                {
                    region.name = "Lake " + region.name;
                }
                else
                    region.name += " Ocean";
            }

            foreach (Coord tile in region.tiles)
            {
                regionNames[tile.x, tile.y] = region.name;
            }

        }

    }
    List<Coord> GetRegionTiles(int[,] baseMap, int startX, int StartY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[gameManager.world.width, gameManager.world.height];
        int tileType = baseMap[startX, StartY];

        string tileSeed = gameManager.world.seed + startX + StartY;

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, StartY));
        mapFlags[startX, StartY] = 1; //Flaged as part of region

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.x - 1; x <= tile.x + 1; x++)
            {
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    if (gameManager.world.IsInMapRange(x, y)) //&& (x == tile.x || y == tile.y))
                    {
                        if (mapFlags[x, y] == 0 && baseMap[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    List<Region> GetRegions(int[,] baseMap, int tileType)
    {
        List<Region> regions = new List<Region>();
        int[,] mapFlags = new int[gameManager.world.width, gameManager.world.height];
        for (int x = 0; x < gameManager.world.width; x++)
        {
            for (int y = 0; y < gameManager.world.height; y++)
            {
                if (mapFlags[x, y] == 0 && baseMap[x, y] == tileType)
                {
                    string regSeed = gameManager.world.seed + x + y;
                    string newRegionName = gameManager.world.nameGen.GenerateRegionName(regSeed);
                    Region newRegion = new Region(GetRegionTiles(baseMap, x, y), newRegionName, tileType);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion.tiles)
                    {
                        mapFlags[tile.x, tile.y] = 1;
                    }
                }
            }
        }

        return regions;
    }
    public void DestroyWorld()
    {
        if (layers != null)
        {
            foreach (Transform layer in layers.Values)
            {
                Destroy(layer.gameObject);
            }
        }

            
    }

    void ToggleRandomSeed()
    {
        if (gameManager.world.useRandomSeed)
        {
            gameManager.world.useRandomSeed = false;
            seedInput.interactable = true;
        }
        else
        {
            gameManager.world.useRandomSeed = true;
            seedInput.interactable = false;
        }
            
    }

}
