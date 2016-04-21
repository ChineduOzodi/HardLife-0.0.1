using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LocalMapGen : MonoBehaviour {

    World world;
    public LocalMap local;
    public WorldGen worldGen;
    public string seed;
    public int width = 100;
    public int height = 100;

    public GameObject whiteBlock;

    int aveTemp;

    private Dictionary<string, Transform> layers = new Dictionary<string, Transform> { };

	// Use this for initialization
	void Awake () {

        worldGen = GetComponent<WorldGen>();
        world = worldGen.world;

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

        worldGen.mainCam.orthographicSize = world.height / 2f;
        worldGen.gameManager.maxCamSize = world.height / 2f;
        worldGen.mainCam.transform.position = new Vector3(world.width / 2, world.height / 2, -10f);

        layers["BaseMap"] = new GameObject("LocalBaseMap").transform;

        //int[,] map = [width, height];
        float[,] map = local.elevationMap;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //map[x, y] = noise.ScaleFloatToInt(local.elevationMap[x,y],local.baseMapNC);
                float num = map[x, y]; // (float)map[x, y] / (float)max;
                Color col = new Color(num, num, num);
                SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
                rend.color = col;

                GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

                instance.transform.SetParent(layers["BaseMap"]);
            }
        }



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
                    adj[x+1-nbrX, y+1-nbrY] = baseMap[nbrX, nbrY];
                }
                else
                {
                    adj[x + 1 - nbrX, y + 1 - nbrY] = -1; 
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
                    adj[nbrX, nbrY] = baseMap[nbrX, nbrY];
                }
                else
                {
                    adj[nbrX, nbrY] = baseMap[x,y];
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
                    adj[nbrX, nbrY] = baseMap[nbrX, nbrY];
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
