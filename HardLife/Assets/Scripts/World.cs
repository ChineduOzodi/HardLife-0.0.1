using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class World : MonoBehaviour {

    public string worldName;
    public int width;
    public int height;
    [Range(0,100)]
    public int randomFillPercent;
    public string seed;
    public bool useRandomSeed;
    public float mountainScale;
    public float rainScale;

    int[,] baseMap;
    int[,] mountainMap;
    int[,] rainMap;
    int[,] tempMap;
    public GameObject whiteBlock;
    public GameObject[] water;
    public GameObject[] dirt;

    private Transform worldHolder = null;
    private int tileCount = 12;
    private List<Region> regions;

    public World()
    {
        randomFillPercent = 52;
        seed = null;
        height = 80;
        width = 100;
        useRandomSeed = true;
        mountainScale = 1f;
        rainScale = 1f;
        GenerateMap();

    }
    public void GenerateMap()
    {
        
        mainCam.orthographicSize = height / 2f;

        baseMap = new int[width, height];
        RandomFillMap();
        SmoothMap(tileCount,2);
        SmoothMap(tileCount,1);

        FresNoise noise = new FresNoise();

        mountainMap = new int[width, height];
        mountainMap = noise.CalcNoise(width, height,new float[] { .5f, .8f, 1f }, seed,mountainScale);
        rainMap = new int[width, height];
        rainMap = noise.CalcNoise(width, height, new float[] { .33f, .66f, 1f }, seed, rainScale);
        tempMap = new int[width, height];
        GenerateTempMap(new float[] { .33f, .66f, 1f });

    }

    private void GenerateTempMap(float[] heightMap)
    {
        FresNoise noise = new FresNoise();
        int max = height / 2;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y <= height / 2)
                {
                    tempMap[x, y] = noise.ScaleFloatToInt(y/max,heightMap);
                }
                else
                {
                    tempMap[x, y] = noise.ScaleFloatToInt((y - (y - max))/max, heightMap);
                }
            }
        }
    }

    List<Tile> GetRegionTiles(int startX, int StartY)
    {
        List<Tile> tiles = new List<Tile>();
        int[,] mapFlags = new int[width, height];
        int tileType = baseMap[startX, StartY];

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(new Tile(startX, StartY));
        mapFlags[startX, StartY] = 1;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x,y) && (x == tile.tileX || y == tile.tileY))
                    {
                        if (mapFlags[x,y] == 0 && baseMap[x,y] == tileType)
                        {
                            mapFlags[x,y] = 1;
                            queue.Enqueue(new Tile(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    List<List<Tile>> GetRegions(int tileType)
    {
        List<List<Tile>> regions = new List<List<Tile>>();
        int[,] mapFlags = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x,y] == 0 && baseMap[x,y] == tileType) 
                {
                    List<Tile> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    
                    foreach (Tile tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    void ProcessMap()
    {
        List<List<Tile>> groundRegions = GetRegions(1);
        int groundThresholdSize = 2;

        foreach (List<Tile> groundRegion in groundRegions)
        {
            if( groundRegion.Count < groundThresholdSize)
            {
                foreach (Tile tile in groundRegion)
                {
                    baseMap[tile.tileX, tile.tileY] = 0;
                }
            }
        }

    }

    bool IsInMapRange (int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return true;
        else
            return false;
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random randNum = new System.Random(seed.GetHashCode());
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    baseMap[x, y] = 0;
                else
                    baseMap[x, y] = (randNum.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    public void SmoothMap(int tileCount, int lDist)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int nbrWaterTiles = GetSurroundingWaterCount(x,y, lDist);
                double powX = lDist * 2f + 1;
                tileCount = (int)Math.Pow(powX, 2) / 2;
                if (nbrWaterTiles > tileCount+1)
                    baseMap[x, y] = 0;
                else if (nbrWaterTiles < tileCount-1)
                    baseMap[x, y] = 1;
            }
        }
    }

    int GetSurroundingWaterCount (int gridX, int gridY, int lDist)
    {
        int waterCount = 0;
        for (int nbrX = gridX - lDist; nbrX <=gridX + lDist; nbrX++)
        {
            for (int nbrY = gridY - lDist; nbrY <= gridY + lDist; nbrY++)
            {
                if (IsInMapRange(nbrX,nbrY))
                {
                    if (nbrX != gridX || nbrY != gridY)
                    {
                        if (baseMap[nbrX, nbrY] == 0)
                            waterCount++;
                    }
                }
                else
                {
                    waterCount++;
                }

            }
        }

        return waterCount;
    }

    public void CreateWorld()
    {
        worldHolder = new GameObject("World").transform;

        for (int x = 0; x < width; x++)
        {
                for (int y = 0; y < height; y++)
                {

                    GameObject toInstantiate = water[UnityEngine.Random.Range(0, water.Length)];

                    if (baseMap[x,y] == 1)
                    {
                        toInstantiate = dirt[UnityEngine.Random.Range(0, dirt.Length)];
                    }

                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y), Quaternion.identity) as GameObject;

                    instance.transform.SetParent(worldHolder);
                }
        }
        
    }

    public void PreviewWorld(int[,] map = null, int max = 1)
    {
        if (map == null)
        {
            map = baseMap;
        }
        worldHolder = new GameObject("World").transform;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float num = (float)map[x, y] / (float)max;
                Color col = new Color(num, num, num);
                SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
                rend.color = col;

                GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

                instance.transform.SetParent(worldHolder);
            }
        }

    }

    public void PreviewWorld(String name)
    {
        int[,] map = new int[width, height];
        if (name == "Base")
            map = baseMap;
        else if (name == "Mountains")
            map = mountainMap;
        else if (name == "Rain")
            map = rainMap;
        else if (name == "Temp")
            map = tempMap;

        int max = 1;
        worldHolder = new GameObject("World").transform;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float num = (float)map[x, y] / (float)max;
                Color col = new Color(num, num, num);
                SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
                rend.color = col;

                GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

                instance.transform.SetParent(worldHolder);
            }
        }

    }

    public void DestroyWorld()
    {
        if (worldHolder != null)
            Destroy(worldHolder.gameObject);
    }
}
