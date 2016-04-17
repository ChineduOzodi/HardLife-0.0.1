using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class World : MonoBehaviour {
    /// <summary>
    /// The World Class that can generate and create a game world based on inputed parameters
    /// </summary>

    public string worldName;
    public int width = 100;
    public int height = 80;
    [Range(0,100)]
    public int randomFillPercent = 52;
    public string seed = null;
    public bool useRandomSeed;

    public int maxLakeSize = 50;
    public int maxIslandSize = 4;

    public float[] mountainNC; //Mountain noise conversion scale
    public float mountainScale;
    public float[] rainNC;
    public float rainScale;
    public int numLayers;
    public List<Region> regions;

    public int[][,] mapLayers;
    public string[] layerNames;

    private int tileCount = 12;
    private NameGen nameGen = new NameGen();

    ///-----initializer(s)
    public World()
    {
        useRandomSeed = true;

        float[] mountainNC = new float[] { .5f, .8f, 1f };
        mountainScale = 1f;
        float[] rainNC = new float[] { .33f, .66f, 1f };
        rainScale = 1f;

        string[] layerNames = { "Base Map", "Temperature Map", "Rain Map", "Mountain Map" };

        SetLayers(layerNames);
        GenerateMap();


    }
    //--------------Map Generation Functions----------------

    /// <summary>
    /// Generates Map based on layers, there currently needs to be at lease 4 layers
    ///This includes a Base Map, Temperature Map, Rain Map, and Mountain Map
    /// </summary>
    public void GenerateMap(string name = null)
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        if (name == null)
            name = nameGen.GenerateWorldName(seed);

        worldName = name;

        for (int i = 0; i < numLayers; i++)
        {
            string layerName = layerNames[i];
            int[,] map = mapLayers[i];
            FresNoise noise = new FresNoise();

            if ( layerName == "Base Map")
            {
                mapLayers[i] = RandomFillMap(map);
                SmoothMap(map, tileCount, 2);
                SmoothMap(map, tileCount, 1);
            }
            else if (layerName == "Temperature Map")
            {
                map = GenerateTempMap(map, new float[] { .33f, .66f, 1f });
            }
            else if (layerName == "Mountain Map")
            {
                map = noise.CalcNoise(width, height, mountainNC, seed, mountainScale);
            }
            else if (layerName == "Rain Map")
            {
                map = noise.CalcNoise(width, height, rainNC, seed, rainScale);
            }

        }

        GenerateRegions();
    }

    private int[,] GenerateTempMap(int[,] tempMap, float[] heightMap)
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

        return tempMap;
    }
    ///------- Region Functions
    ///
    private void GenerateRegions()
    {
        int[,] baseMap = mapLayers[Array.IndexOf(layerNames, "Base Map")];

        //Generate Land and Island Regions
        List<Region> groundRegions = GetRegions(baseMap,1);

        foreach (Region region in groundRegions)
        {
            if (region.tiles.Count <= maxIslandSize)
            {
                region.name += "Island";
            }
        }

        //Generate Ocea and Sea Regions
        List<Region> waterRegions = GetRegions(baseMap, 0);

        foreach (Region region in waterRegions)
        {
            if (region.tiles.Count <= maxLakeSize)
            {
                region.name = "Lake" + region.name;
            }
        }

        //foreach (List<Tile> groundRegion in groundRegions)
        //{
        //    if (groundRegion.Count < groundThresholdSize)
        //    {
        //        foreach (Tile tile in groundRegion)
        //        {
        //            baseMap[tile.tileX, tile.tileY] = 0;
        //        }
        //    }
        //}

    }
    List<Tile> GetRegionTiles(int[,] baseMap, int startX, int StartY)
    {
        List<Tile> tiles = new List<Tile>();
        int[,] mapFlags = new int[width, height];
        int tileType = baseMap[startX, StartY];

        string tileSeed = seed + startX + StartY;

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(new Tile(startX, StartY,tileSeed));
        mapFlags[startX, StartY] = 1; //Flaged as part of region

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
                            tileSeed = seed + x + y;
                            mapFlags[x,y] = 1;
                            queue.Enqueue(new Tile(x, y, tileSeed));
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
        int[,] mapFlags = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x,y] == 0 && baseMap[x,y] == tileType) 
                {
                    string regSeed = seed + x + y;
                    string newRegionName = nameGen.GenerateRegionName(regSeed);
                    Region newRegion = new Region(GetRegionTiles(baseMap, x, y), newRegionName,tileType);
                    regions.Add(newRegion);
                    
                    foreach (Tile tile in newRegion.tiles)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }
    ///----------Helper Functions
    ///
    private void SetLayers(string[] layers)
    {
        numLayers = layers.Length;

        for (int i = 0; i < numLayers; i++)
        {
            mapLayers[i] = new int[width, height];
        }
        
    }
    
    bool IsInMapRange (int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return true;
            else
                return false;
        }

    int[,] RandomFillMap(int[,] baseMap)
    {

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

        return baseMap;
    }

    public int[,] SmoothMap(int[,] baseMap, int tileCount, int lDist)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int nbrWaterTiles = GetSurroundingWaterCount(baseMap, x,y, lDist);
                double powX = lDist * 2f + 1;
                tileCount = (int)Math.Pow(powX, 2) / 2;
                if (nbrWaterTiles > tileCount+1)
                    baseMap[x, y] = 0;
                else if (nbrWaterTiles < tileCount-1)
                    baseMap[x, y] = 1;
            }
        }

        return baseMap;
    }

    int GetSurroundingWaterCount (int[,] baseMap, int gridX, int gridY, int lDist)
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

    
}
