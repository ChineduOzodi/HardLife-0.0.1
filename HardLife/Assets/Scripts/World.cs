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
    public int maxIslandSize = 10;

    public float[] mountainNC; //Mountain noise conversion scale
    public float mountainScale;
    public float[] rainNC;
    public float rainScale;
    public int numLayers;
    public Tile[,] tiles;

    public int[][,] mapLayers;
    public string[] layerNames;

    private int tileCount = 12;
    private NameGen nameGen = new NameGen();

    ///-----initializer(s)
    public World()
    {
        useRandomSeed = true;

        mountainNC = new float[3] { .5f, .75f, 1f };
        mountainScale = 10f;
        rainNC = new float[3] { .33f, .66f, 1f };
        rainScale = 7f;
        layerNames = new [] { "Base Map", "Temperature Map", "Rain Map", "Mountain Map" };

        SetLayers(layerNames);


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

        SetLayers(layerNames);

        GenerateLayers();
        GenerateRegions();
        GenerateBiomes();
        

    }

    private void GenerateBiomes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(mapLayers[Array.IndexOf(layerNames, "Base Map")][width, height] == 1)
                {
                    
                    int tileTemp = mapLayers[Array.IndexOf(layerNames, "Temperature Map")][width, height];
                    int tileRain = mapLayers[Array.IndexOf(layerNames, "Rain Map")][width, height];

                    tiles[width, height].id = BiomeID(tileTemp,tileRain);
                    

                }  
            }
        }
    }
    /// <summary>
    /// Returns biome ID based on matrix
    /// </summary>
    /// <param name="tileTemp"> int of tile temp from 0 to 2</param>
    /// <param name="tileRain">int of tile rain amount from 0 to 2</param>
    /// <returns></returns>
    private int BiomeID(int tileTemp, int tileRain)
    {
        int biomeID = 10;
        int ice = 1;
        int grass = 2;
        int jungle = 3;
        int desert = 4;
        if (tileTemp == 0)
        {
            if (tileRain == 0)
            {
                return ice;
            }
            else if (tileRain == 1)
            {
                return grass;
            }
            else if (tileRain == 2)
            {
                return ice;
            }
        }
        else if (tileTemp == 1)
        {
            if (tileRain == 0)
            {
                return grass;
            }
            else if (tileRain == 1)
            {
                return grass;
            }
            else if (tileRain == 2)
            {
                return jungle;
            }
        }
        else if (tileTemp == 2)
        {
            if (tileRain == 0)
            {
                return desert;
            }
            else if (tileRain == 1)
            {
                return desert;
            }
            else if (tileRain == 2)
            {
                return jungle;
            }
        }
        return biomeID;
    }

    private void GenerateLayers()
    {
        for (int i = 0; i < numLayers; i++)
        {
            string layerName = layerNames[i];
            int[,] map = mapLayers[i];
            FresNoise noise = new FresNoise();

            if (layerName == "Base Map")
            {
                mapLayers[i] = RandomFillMap(map);
                SmoothMap(mapLayers[i], tileCount, 2);
                SmoothMap(mapLayers[i], tileCount, 1);
            }
            else if (layerName == "Temperature Map")
            {
                map = GenerateTempMap(mapLayers[i], new float[] { .33f, .75f, 1f });
            }
            else if (layerName == "Mountain Map")
            {
                mapLayers[i] = noise.CalcNoise(width, height, mountainNC, seed, mountainScale);
            }
            else if (layerName == "Rain Map")
            {
                mapLayers[i] = noise.CalcNoise(width, height, rainNC, seed, rainScale);
            }

        }
    }

    private int[,] GenerateTempMap(int[,] tempMap, float[] heightMap)
    {
        FresNoise noise = new FresNoise();
        int max = height / 2;
       
        for (int x = 0; x < width; x++)
        {
            float hCount = 0f;
            for (int y = 0; y < height; y++)
            {
                if (y <= max)
                {
                    float toFloat = hCount / (float)max;
                    int scaledNum = noise.ScaleFloatToInt(toFloat, heightMap);
                    tempMap[x, y] = scaledNum;
                    hCount++;
                }
                else
                {
                    float toFloat = hCount/ (float)max;
                    int scaledNum = noise.ScaleFloatToInt(toFloat, heightMap);
                    tempMap[x, y] = scaledNum;
                    hCount--;
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
                region.name += " Island";
            }
        }

        //Generate Ocea and Sea Regions
        List<Region> waterRegions = GetRegions(baseMap, 0);

        foreach (Region region in waterRegions)
        {
            if (region.tiles.Count <= maxLakeSize)
            {
                region.name = "Lake " + region.name;
            }
            else
                region.name += " Ocean";
        }

        waterRegions.AddRange(groundRegions);
        tiles = new Tile[width, height];

        foreach(Region region in waterRegions)
        {
            foreach (Tile tile in region.tiles)
            {
                tile.region = region.name;
                tiles[tile.x, tile.y] = tile;
            }

            //print(region.name + region.tiles.Count);
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

            for (int x = tile.x - 1; x <= tile.x + 1; x++)
            {
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    if (IsInMapRange(x,y)) //&& (x == tile.x || y == tile.y))
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
                        mapFlags[tile.x, tile.y] = 1;
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
        mapLayers = new int[numLayers][,];

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
