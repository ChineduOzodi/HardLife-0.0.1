using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class WorldGen : MonoBehaviour { //TODO Fix World Generation
    #region "Declarations"
    
    public bool useRandomSeed = true;
    bool tileSelected = false;
    SpriteRenderer selectedTile;
    internal GameManager gameManager;
    public Camera mainCam;


    //texturing
    //private Sprite whiteBlock;
    //private Sprite[] water;
    //private Sprite[] ice;
    //private Sprite[] grass;
    //private Sprite[] jungle;
    //private Sprite[] desert;
    //private Sprite[] hill;
    //private Sprite[] mountain;

    //UI Setup
    public Text infoText;
    public Text worldNameText;
    public InputField seedInput;
    public Button createLocalMapButton;
    
    string[] tType = { "Flat", "Hills", "Mountains" };
    string[] aveRain = { "Little", "Normal", "Lots" };
    
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
	//void OnMouseDown()
	//{

	//	if (!EventSystem.current.IsPointerOverGameObject())
	//	{
	//		gameObject.transform.localScale = new Vector3(1, 1);
	//		gameObject.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
	//		gameManager.worldGen.layers["Biomes"].BroadcastMessage("ToggleTileSelected");
	//		gameManager.worldGen.SendMessage("ToggleTileSelected");
	//		//gameObject.GetComponentInParent<Transform>().BroadcastMessage("ToggleTileSelected");
	//	}


	//}

    public void CreateWorld()
    {
        //DestroyWorld();

        //mainCam.orthographicSize = gameManager.gridWorldSize.x / 2f;
        //gameManager.maxCamSize = gameManager.gridWorldSize.y / 2f;
        //mainCam.transform.position = new Vector3(0, 0, -10f);

        if (useRandomSeed)
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
        throw new NotImplementedException();
        //gameManager.world.localMaps[selectedTile.x, selectedTile.y] = localMapGen.CreateLocalMap(selectedTile);
        //layers["Biomes"].gameObject.SetActive(false);
        //layers["Mountains"].gameObject.SetActive(false);
        //localMapGen.buildBaseMap(gameManager.world.localMaps[selectedTile.x, selectedTile.y]);
    } 

    private void buildMountains()
    {
        throw new NotImplementedException();
        //layers["Mountains"] = new GameObject("Mountains").transform;
        //int[,] map = gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Mountain Map")];
        //for (int x = 0; x < gameManager.world.worldSize.x; x++)
        //{
        //    for (int y = 0; y < gameManager.world.worldSize.y; y++)
        //    {
        //        if (map[x,y] == 1)
        //        {
        //            int num = UnityEngine.Random.Range(0, hill.Length);
        //            GameObject tile = hill[num].gameObject;
        //            GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;
        //            instance.transform.SetParent(layers["Mountains"]);
        //        }
        //        else if(map[x,y] == 2)
        //        {
        //            int num = UnityEngine.Random.Range(0, mountain.Length);
        //            GameObject tile = mountain[num].gameObject;
        //            GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;
        //            instance.transform.SetParent(layers["Mountains"]);
        //        }
        //    }
        //}
    }

    void SendInfo(int[] coord)
    {
        throw new NotImplementedException();
        //selectedTile = new Coord(coord[0], coord[1]);
        //string terrain = tType[gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Mountain Map")][coord[0], coord[1]]];
        //string rain = aveRain[gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Rain Map")][coord[0], coord[1]]];
        //string info = "Region Name: " + regionNames[coord[0], coord[1]];
        //info += "\nTerrain Type: " + terrain;
        //info += "\nAverage Temperature: " + gameManager.world.aveTempMap[coord[0], coord[1]] + " C (" + (gameManager.world.aveTempMap[coord[0], coord[1]] - tempYearRange) + " - " + (gameManager.world.aveTempMap[coord[0], coord[1]] + tempYearRange) + ")";
        //info += "\nAverage Rain: " + rain;
        //infoText.text = info;
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
        throw new NotImplementedException();
        //layers["Biomes"] = new GameObject("Biomes").transform;
        //int[,] map = gameManager.world.mapLayers[Array.IndexOf(gameManager.world.layerNames, "Biome Map")];

        //for (int x = 0; x < gameManager.world.worldSize.x; x++)
        //{
        //    for (int y = 0; y < gameManager.world.worldSize.y; y++)
        //    {
        //        int sprite = map[x,y];
        //        int num = UnityEngine.Random.Range(0, biomeSprites[sprite].Length);
        //        GameObject tile = biomeSprites[sprite][num].gameObject;
        //        GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;

        //        instance.transform.SetParent(layers["Biomes"]);
        //        instance.AddComponent<WorldTile>();
        //        instance.GetComponent<WorldTile>().SetupTile(gameManager, x,y,gameManager.world.seed);
        //    }
        //}
        ////layers["Biomes"].gameObject.SetActive(false);
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
    /// <summary>
    /// 
    /// </summary>
    
    public void DestroyWorld()
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
