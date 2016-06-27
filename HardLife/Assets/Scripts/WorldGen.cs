using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WorldGen : MonoBehaviour { //TODO Fix World Generation
    #region "Declarations"
    
    public bool useRandomSeed = true;
    bool tileSelected = false;
    SpriteRenderer selectedTile;
    internal GameManager gameManager;
    public Camera mainCam;

    Transform biomeEmpty;
    Transform mountEmpty;

    SpriteRenderer[,] biomeMap;
    SpriteRenderer[,] mountMap;

    //UI Setup
    public Text infoText;
    public Text worldNameText;
    public InputField seedInput;
    public Button createLocalMapButton;
    
    //string[] tType = { "Flat", "Hills", "Mountains" };
    string[] aveRain = { "Little", "Normal", "Lots" };
    private float tempYearRange = 10;

    //private GameObject[][] biomeSprites;
    //public Dictionary<string, Transform> layers = new Dictionary<string, Transform> { };


    #endregion

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>() ;
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //biomeSprites = new GameObject[][] { water, ice, grass, jungle, desert };
        

    }
    //void OnMouseEnter()
    //{
    //	if (!tileSelected && !EventSystem.current.IsPointerOverGameObject())
    //	{
    //		gameManager.SendMessage("SendInfo", coord);
    //		//print("Found Mouse");
    //		//print(coord);
    //		//gameObject.transform.localScale = new Vector3(1.25f, 1.25f);
    //		gameObject.GetComponent<SpriteRenderer>().color = new Color(.8f, .8f, .8f);
    //	}

    //}
    //void OnMouseExit()
    //{
    //	if (!tileSelected && !EventSystem.current.IsPointerOverGameObject())
    //	{
    //		gameObject.transform.localScale = new Vector3(1, 1);
    //		gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
    //	}

    //}
    public void LeftMouseDown()
    {

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedTile != null)
            {
                selectedTile.color = new Color(1f, 1f, 1f);
            }
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Coord coord = gameManager.WorldCoordFromWorldPosition(worldPosition);
            biomeMap[coord.x, coord.y].color = new Color(.5f, .5f, .5f);
            
            gameManager.world.localMap = gameManager.world.localMaps[coord.x, coord.y];

            selectedTile = biomeMap[coord.x, coord.y];
            TileSelected();

            SendInfo(coord);

        }


    }

    public void CreateWorld()
    {
        gameManager.setup = true;
        DestroyWorld();

        mainCam.orthographicSize = gameManager.worldSize.x / 2f;
        gameManager.maxCamSize = gameManager.worldSize.y / 2f;
        mainCam.transform.position = new Vector3(0, 0, -10f);

        gameManager.world = new World(gameManager.worldSize, gameManager.localSize, gameManager.nodeRadius); //new world

        if (useRandomSeed) //determine seed
        {
            gameManager.world.seed = Time.time.ToString();
            seedInput.text = gameManager.world.seed;
        }
        else
        {
            gameManager.world.seed = seedInput.text;
        }

        gameManager.world.GenerateMap(gameManager.world.seed);
        seedInput.text = gameManager.world.seed; // just in case GenerateMap changes the gameManager.world.seed

        worldNameText.text = gameManager.world.name;
        //buildBasicLayers();
        buildBiome();
        buildMountains();
        gameManager.setup = false;
    }
    public void loadWorld()
    {

        mainCam.orthographicSize = gameManager.world.worldSize.x / 2f;
        gameManager.maxCamSize = gameManager.world.worldSize.y / 2f;
        mainCam.transform.position = new Vector3(gameManager.world.worldSize.x / 2, gameManager.world.worldSize.y / 2, -10f);
        seedInput.text = gameManager.world.seed; // just in case GenerateMap changes the gameManager.world.seed

        worldNameText.text = gameManager.world.name;
        //buildBasicLayers();
        buildBiome();
        buildMountains();
    }

    //public void PreviewWorld(string layerName, int max = 1)
    //{
    //    int[,] map = gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, layerName)];
    //    layers["gameManager.world"] = new GameObject("gameManager.world").transform;

    //    mainCam.orthographicSize = gameManager.world.worldSize.y / 2f;
    //    mainCam.transform.position = new Vector3(gameManager.world.worldSize.x / 2, gameManager.world.worldSize.y / 2, -10f);

    //    for (int x = 0; x < gameManager.world.worldSize.x; x++)
    //    {
    //        for (int y = 0; y < gameManager.world.worldSize.y; y++)
    //        {
    //            float num = (float)map[x, y] / (float)max;
    //            Color col = new Color(num, num, num);
    //            SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
    //            rend.color = col;

    //            GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

    //            instance.transform.SetParent(layers["gameManager.world"]);
    //        }
    //    }

    //}
    /// <summary>
    /// Preview Map with the string name
    /// </summary>
    /// <param name="name"></param>
    public void PreviewWorld()
    {

        mainCam.orthographicSize = gameManager.world.worldSize.y / 2f;
        mainCam.transform.position = Vector2.zero; //new Vector3(gameManager.world.worldSize.x / 2, gameManager.world.worldSize.y / 2, -10f);

        buildBasicLayers();
        buildBiome();
        buildMountains();
    }

    public void CreateLocalMap()
    {
        gameManager.setup = true;
        SceneManager.LoadScene("local_map");
    } 

    private void buildMountains()
    {

        mountEmpty = new GameObject("Mountains").transform;
        mountMap = new SpriteRenderer[gameManager.world.worldSizeX, gameManager.world.worldSizeY];

        for (int x = 0; x < gameManager.world.worldSizeX; x++)
        {
            for (int y = 0; y < gameManager.world.worldSizeY; y++)
            {
                if (gameManager.world.localMaps[x, y].mountainLevel == "Hills" && gameManager.world.localMaps[x, y].biome != "Water")
                {
                    Vector3 worldPoint = gameManager.world.worldBottomLeft + Vector3.right * (x * gameManager.world.nodeDiameter + gameManager.world.nodeRadius) + Vector3.up * (y * gameManager.world.nodeDiameter + gameManager.world.nodeRadius);
                    GameObject tile = new GameObject("Hill");
                    tile.transform.position = worldPoint;
                    SpriteRenderer instance = tile.AddComponent<SpriteRenderer>();
                    instance.sprite = gameManager.spriteManager.GetSprite("silvercoin");
                    instance.sortingOrder = 1;
                    instance.transform.SetParent(mountEmpty);
                    mountMap[x, y] = instance;
                }
                else if (gameManager.world.localMaps[x, y].mountainLevel == "Mountains" && gameManager.world.localMaps[x, y].biome != "Water")
                {
                    Vector3 worldPoint = gameManager.world.worldBottomLeft + Vector3.right * (x * gameManager.world.nodeDiameter + gameManager.world.nodeRadius) + Vector3.up * (y * gameManager.world.nodeDiameter + gameManager.world.nodeRadius);
                    GameObject tile = new GameObject("Mountain");
                    tile.transform.position = worldPoint;
                    SpriteRenderer instance = tile.AddComponent<SpriteRenderer>();
                    instance.sprite = gameManager.spriteManager.GetSprite("boulder");
                    instance.sortingOrder = 1;
                    instance.transform.SetParent(mountEmpty);
                    mountMap[x, y] = instance;
                }
            }
        }
    }

    void SendInfo(Coord coord)
    {
        string terrain = gameManager.world.localMaps[coord.x, coord.y].mountainLevel;
        string rain = gameManager.world.localMaps[coord.x, coord.y].rain.ToString();
        string info = "Region Name: " + gameManager.world.localMaps[coord.x, coord.y].region;
        string aveTemp = Mathf.RoundToInt(gameManager.world.localMaps[coord.x, coord.y].aveTemp) + " C (" + (Mathf.RoundToInt(gameManager.world.localMaps[coord.x, coord.y].aveTemp) - tempYearRange) + " - " + (Mathf.RoundToInt(gameManager.world.localMaps[coord.x, coord.y].aveTemp) + tempYearRange) + ")";
        info += "\nTerrain Type: " + terrain;
        info += "\nAverage Temperature: " + aveTemp;
        info += "\nAverage Rain: " + rain + " in";
        infoText.text = info;
    }

    void TileSelected()
    {

        //Previously TOggleTileSelected
        if (tileSelected)
        {
            //createLocalMapButton.interactable = false;
            //tileSelected = false;
        }
        else
        {
            createLocalMapButton.interactable = true;
            tileSelected = true;
        }

    }

    private void buildBiome()
    {
        biomeEmpty = new GameObject("Biomes").transform;
        biomeMap = new SpriteRenderer[gameManager.world.worldSizeX, gameManager.world.worldSizeY];
        //int[,] map = gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Biome Map")];

        //SetupUsedTles

        for (int x = 0; x < gameManager.world.worldSizeX; x++)
        {
            for (int y = 0; y < gameManager.world.worldSizeY; y++)
            {                
                Vector3 worldPoint = gameManager.world.worldBottomLeft + Vector3.right * (x * gameManager.world.nodeDiameter + gameManager.world.nodeRadius) + Vector3.up * (y * gameManager.world.nodeDiameter + gameManager.world.nodeRadius);
                GameObject tile = new GameObject("BiomeTile");
                tile.transform.position = worldPoint;
                SpriteRenderer instance = tile.AddComponent<SpriteRenderer>();
                instance.sprite = gameManager.spriteManager.GetSprite(gameManager.world.localMaps[x, y].biome);
                gameManager.world.localMaps[x, y].worldPosition = worldPoint;
                instance.transform.SetParent(biomeEmpty);
                biomeMap[x, y] = instance;
            }
        }
        //layers["Biomes"].gameObject.SetActive(false);
    }

    private void buildBasicLayers()
    {
        throw new NotImplementedException();
        //for (int i = 0; i < gameManager.world.layerNames.Length; i++)
        //{
        //    int[,] map = gameManager.world.mapLayers[i];
        //    int max = 2;

        //    layers[gameManager.world.layerNames[i]] = new GameObject(gameManager.world.layerNames[i]).transform;

        //    for (int x = 0; x < gameManager.world.worldSize.x; x++)
        //    {
        //        for (int y = 0; y < gameManager.world.worldSize.y; y++)
        //        {
        //            float num = (float)map[x, y] / (float)max;
        //            Color col = new Color(num, num, num);
        //            SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
        //            rend.color = col;

        //            GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

        //            instance.transform.SetParent(layers[gameManager.world.layerNames[i]]);
        //        }
        //    }

        //    layers[gameManager.world.layerNames[i]].gameObject.SetActive(false);

        //}
    }
    
    public void DestroyWorld()
    {

        if (biomeEmpty != null)
        {
            Destroy(biomeEmpty.gameObject);
        }
        if (mountEmpty != null)
        {
            Destroy(mountEmpty.gameObject);
        }


    }

    public void ToggleRandomSeed()
    {
        if (useRandomSeed)
        {
            useRandomSeed = false;
            seedInput.interactable = true;
        }
        else
        {
            useRandomSeed = true;
            seedInput.interactable = false;
        }
            
    }

}
