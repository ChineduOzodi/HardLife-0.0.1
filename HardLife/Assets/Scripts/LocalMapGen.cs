using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LocalMapGen : MonoBehaviour {

    World world;
    public LocalMap local;
    public GameManager gameManager;
    public string seed;
    public int width = 100;
    public int height = 100;

    //public GameObject whiteBlock;
    //public GameObject[] water;
    //public GameObject[] sand;
    //public GameObject[] grass;
    //public GameObject[] jungle;
    //public GameObject[] desert;
    public GameObject[] rock;
    private Dictionary<String, GameObject[]> sprites;

    int aveTemp;

    public Dictionary<string, Transform> layers = new Dictionary<string, Transform> { };

	// Use this for initialization
	void Awake () {

        gameManager = GetComponent<GameManager>();
        world = gameManager.world;

        sprites = new Dictionary<string, GameObject[]>();
//        sprites.Add("water", worldGen.water);
//        sprites.Add("sand", worldGen.desert);
//        sprites.Add("grass", worldGen.grass);
//        sprites.Add("rock", rock);
//        sprites.Add("jungle", worldGen.jungle);

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public LocalMap CreateLocalMap(Coord coord)
    {      
        int x = coord.x;
        int y = coord.y;

        seed = world.seed + x + y;

        aveTemp = world.aveTempMap[x, y];
        int[,] baseMap = world.mapLayers[Array.IndexOf(world.layerNames, "Base Map")];
        int[,] biomeMap = world.mapLayers[Array.IndexOf(world.layerNames, "Biome Map")];
        int[,] mMap = world.mapLayers[Array.IndexOf(world.layerNames, "Mountain Map")];
        int biomeType = biomeMap[x, y];
        int mType = mMap[x, y];

        int[,] adjacentBaseTiles = AdjacentTiles(baseMap, x, y);
        int[,] adjacentMTiles = AdjacentTiles(mMap, x, y);

        local = new LocalMap(seed, biomeType, mType, adjacentBaseTiles, adjacentMTiles, width, height);
        //world.localMaps[x, y] = local;
        return local;

    }

    public void PreviewMap(LocalMap local)
    {
        DestroyLocalMap();

        FresNoise noise = new FresNoise();

//        worldGen.mainCam.orthographicSize = local.height / 2f;
//        worldGen.gameManager.maxCamSize = local.height / 2f;
//        worldGen.mainCam.transform.position = new Vector3(local.width / 2, local.height / 2, -10f);
//
//        worldGen.createWorldMenu.gameObject.SetActive(false);
//        worldGen.gameManager.localMapCanvas.gameObject.SetActive(true);

        layers["BaseMap"] = new GameObject("LocalBaseMap").transform;

        //int[,] map = [width, height];
        Tile[,] map = local.baseMap;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //map[x, y] = noise.ScaleFloatToInt(local.elevationMap[x,y],local.baseMapNC);
                float num = (float)map[x, y].id/5f; // (float)map[x, y] / (float)max;
                Color col = new Color(num, num, num);
                //SpriteRenderer rend = worldGen.whiteBlock.GetComponent<SpriteRenderer>();
                //rend.color = col;

                //GameObject instance = Instantiate(worldGen.whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

                //instance.transform.SetParent(layers["BaseMap"]);
            }
        }
     }
    public void buildBaseMap(LocalMap local)
    {
        DestroyLocalMap();
        this.local = local;

//        worldGen.mainCam.orthographicSize = local.height / 2f;
//        worldGen.gameManager.maxCamSize = local.height / 2f;
//        worldGen.mainCam.transform.position = new Vector3(local.width / 2, local.height / 2, -10f);
//
//        worldGen.createWorldMenu.gameObject.SetActive(false);
//        worldGen.gameManager.localMapCanvas.gameObject.SetActive(true);

        layers["BaseMap"] = new GameObject("LocalBaseMap").transform;
        Tile[,] map = local.baseMap;

        for (int x = 0; x < local.width; x++)
        {
            for (int y = 0; y < local.height; y++)
            {
                string sprite = map[x, y].type;
                int num = UnityEngine.Random.Range(0, sprites[sprite].Length);
                GameObject tile = sprites[sprite][num].gameObject;
                GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;

                instance.transform.SetParent(layers["BaseMap"]);
                instance.AddComponent<LocalTile>();
                //instance.GetComponent<LocalTile>().SetupTile(worldGen.gameManager, x, y, local.baseMap[x,y].type);
            }
        }
        //layers["Biomes"].gameObject.SetActive(false);
    }

    private int[,] AdjacentTiles(int[,] baseMap, int x, int y)
    {
        int[,] adj = new int[3, 3];

        for (int nbrX = x - 1; nbrX <= x + 1; nbrX++)
        {
            for (int nbrY = y - 1; nbrY <= y + 1; nbrY++)
            {
                if (IsInMapRange(nbrX, nbrY))
                {
                    adj[nbrX + 1-x, nbrY + 1-y] = baseMap[nbrX, nbrY];
                }
                else
                {
                    adj[nbrX + 1 - x, nbrY + 1 - y] = -1; 
                }

            }
        }

        return adj;
    }
    private float[,] AdjacentTiles(float[,] baseMap, int x, int y)
    {
        float[,] adj = new float[3, 3];

        for (int nbrX = x - 1; nbrX <= x + 1; nbrX++)
        {
            for (int nbrY = y - 1; nbrY <= y + 1; nbrY++)
            {
                if (IsInMapRange(nbrX, nbrY))
                {
                    adj[nbrX + 1 - x, nbrY + 1 - y] = baseMap[nbrX, nbrY];
                }
                else
                {
                    adj[nbrX + 1 - x, nbrY + 1 - y] = baseMap[x,y];
                }

            }
        }

        return adj;
    }
    private T[,] AdjacentTiles<T>(T[,] baseMap, int x, int y)
    {
        T[,] adj = new T[3, 3];

        for (int nbrX = x - 1; nbrX <= x + 1; nbrX++)
        {
            for (int nbrY = y - 1; nbrY <= y + 1; nbrY++)
            {
                if (IsInMapRange(nbrX, nbrY))
                {
                    adj[nbrX + 1 - x, nbrY + 1 - y] = baseMap[nbrX, nbrY];
                }

            }
        }

        return adj;
    }

    public bool IsInMapRange(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return true;
        else
            return false;
    }

    private void DestroyLocalMap()
    {
        if (layers != null)
        {
            foreach (Transform layer in layers.Values)
            {
                Destroy(layer.gameObject);
            }
        }
    }
}
