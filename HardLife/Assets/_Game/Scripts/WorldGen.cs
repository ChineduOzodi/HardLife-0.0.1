using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using CodeControl;

public struct WorldGen {

    public static WorldModel CreateWorld(Vector2 _worldSize, Vector2 _localSize, float _nodeRadius, string seed)
    {
        //layerNames = new [] { "Base Map", "Temperature Map", "Rain Map", "Mountain Map", "Biome Map"};
        //SetLayers(layerNames);

        WorldModel model = new WorldModel();


        model.worldSize = _worldSize;
        model.localSize = _localSize;

        model.worldSizeX = (int)model.worldSize.x;
        model.worldSizeY = (int)model.worldSize.y;

        model.localSizeX = (int)_localSize.x;
        model.localSizeY = (int)_localSize.y;

        model.worldBottomLeft = -Vector3.right * model.worldSize.x / 2 - Vector3.up * model.worldSize.y / 2;
        model.localMaps = new ModelRef<LocalMapModel>[(int) model.worldSize.x * (int)model.worldSize.y];

        float maxLakeSize = (_worldSize.x * _worldSize.y) / 200;
        float maxIslandSize = (_worldSize.x * _worldSize.y) / 1000;

        GenerateMap(model, maxLakeSize, maxIslandSize, seed);

        return model;

        
    }

    //--------------Map Generation Functions----------------

    /// <summary>
    /// Generates Map based on layers, there currently needs to be at lease 4 layers
    ///This includes a Base Map, Temperature Map, Rain Map, and Mountain Map
    /// </summary>
    private static void GenerateMap(WorldModel model, float maxLakeSize, float maxIslandSize, string seed = null, string name = null)
    {
        if (seed == null || seed == "")
        {
            seed = Time.time.ToString();
        }

        if (name == null)
        {

            name = new NameGen().GenerateWorldName(seed);
        }

        model.seed = seed;
        model.name = name;

        //SetLayers(layerNames);

        GenerateLayers(model);
        GenerateRegions(model, maxLakeSize, maxIslandSize);

    }

    private static void GenerateLayers(WorldModel model)
    {
        FresNoise noise = new FresNoise();

        //Declarations

        float mountainScale = 10f;

        float rainScale = 7f;

        float tempScale = 5f;

        //BaseMap Generation
        int[,] baseMap = RandomFillMap(model, new int[model.worldSizeX, model.worldSizeY]);
        baseMap = SmoothMap(model,baseMap, 2);
        baseMap = SmoothMap(model,baseMap, 1);

        //Temperature Map Generation
        float[,] tempMap = GenerateTempMap(model, noise.CalcNoise(model.worldSizeX, model.worldSizeY, model.seed + "temp", tempScale));

        //Elevation Map Generation
        float[,] elevMap = noise.CalcNoise(model.worldSizeX, model.worldSizeY, model.seed + "elevation", mountainScale);

        //Rain Map Generation
        float[,] rainMap = noise.CalcNoise(model.worldSizeX, model.worldSizeY, model.seed + "rain", rainScale);

        //Biome Map
        string[,] biomeMap = GenerateBiomes(model, baseMap, tempMap, elevMap, rainMap);

        tempMap = GenerateAverageTemp(model, biomeMap, tempMap, elevMap, rainMap);

        for (int x = 0; x < model.worldSizeX; x++)
        {
            for (int y = 0; y < model.worldSizeY; y++)
            {
                //Set Local Map Declarations

                int index = ArrayHelper.ElementIndex(x, y, model.worldSizeY);

                model.localMaps[index] = new ModelRef<LocalMapModel>(new LocalMapModel());

                model.localMaps[index].Model.world = new ModelRef<WorldModel>(model);
                model.localMaps[index].Model.localSizeX = model.localSizeX;
                model.localMaps[index].Model.localSizeY = model.localSizeY;
                model.localMaps[index].Model.worldMapPositionX = x;
                model.localMaps[index].Model.worldMapPositionY = y;
                model.localMaps[index].Model.seed = model.seed + x + y;

                model.localMaps[index].Model.biome = biomeMap[x, y];
                model.localMaps[index].Model.aveTemp = tempMap[x, y];
                model.localMaps[index].Model.curTemp = tempMap[x, y];
                model.localMaps[index].Model.elevation = elevMap[x, y];
                model.localMaps[index].Model.rain = rainMap[x, y];
                model.localMaps[index].Model.baseNum = baseMap[x, y];

                //Set Mounatin Level
                if (model.localMaps[index].Model.elevation < .5)
                {
                    model.localMaps[index].Model.mountainLevel = "Flat";
                }
                else if (model.localMaps[index].Model.elevation < .75)
                {
                    model.localMaps[index].Model.mountainLevel = "Hills";
                }
                else
                {
                    model.localMaps[index].Model.mountainLevel = "Mountains";
                }
            }
        }
    }

    private static float[,] GenerateAverageTemp(WorldModel model, string[,] biomeMap, float[,] tempMap, float[,] elevMap, float[,] rainMap)
    {
        //Possible Biome Names: Unknown, Ice, Grass, Desert, Jungle, Water
        float tempScale = 5;
        float rainScale = 3;
        float elevScale = 4;
        Dictionary<string, float> biomeTemps = new Dictionary<string, float>();
        biomeTemps.Add("Grass", 15);
        biomeTemps.Add("Ice", -5);
        biomeTemps.Add("Jungle", 20);
        biomeTemps.Add("Desert", 25);
        biomeTemps.Add("Water", 25);

        float[,] map = new float[model.worldSizeX, model.worldSizeY];

        for (int x = 0; x < model.worldSizeX; x++)
        {
            for (int y = 0; y < model.worldSizeY; y++)
            {
                map[x, y] = biomeTemps[biomeMap[x, y]] + (tempMap[x, y] * tempScale * 2 - tempScale)
                - (rainMap[x, y] * rainScale)
                - (elevMap[x, y] * elevScale);
            }

        }

        return map;
    }

    private static string[,] GenerateBiomes(WorldModel model, int[,] baseMap, float[,] tempMap, float[,] elevMap, float[,] rainMap)
    {
        string[,] map = new string[model.worldSizeX, model.worldSizeY];

        for (int x = 0; x < model.worldSizeX; x++)
        {
            for (int y = 0; y < model.worldSizeY; y++)
            {

                if (baseMap[x, y] == 1)
                {
                    map[x, y] = BiomeName(tempMap[x, y], rainMap[x, y], elevMap[x, y]);
                }
                else
                    map[x, y] = "Water";

            }
        }
        return map;
    }
    /// <summary>
    /// Returns biome ID based on matrix
    /// </summary>
    /// <param name="tileTemp"> int of tile temp from 0 to 2</param>
    /// <param name="tileRain">int of tile rain amount from 0 to 2</param>
    /// <returns></returns>
    private static string BiomeName(float temp, float rain, float elevation)
    {
        //Possible Biome Names: Unknown, Ice, Grass, Desert, Jungle
        string biomeName = "Unknown";
        float[] mountainNC = { .5f, .75f }; //Mountain noise conversion scale
        float[] rainNC = { .33f, .66f };
        float[] tempNC = { .4f, .75f };
        if (temp < tempNC[0])
        {
            if (rain < rainNC[0])
            {
                if (elevation < mountainNC[0])
                {
                    biomeName = "Ice";
                }
                else if (elevation < mountainNC[1])
                {
                    biomeName = "Ice";
                }
                else
                {
                    biomeName = "Ice";
                }
            }
            else if (rain < rainNC[1])
            {
                if (elevation < mountainNC[0])
                {
                    biomeName = "Grass";
                }
                else if (elevation < mountainNC[1])
                {
                    biomeName = "Ice";
                }
                else
                {
                    biomeName = "Ice";
                }
            }
            else
            {
                if (elevation < mountainNC[0])
                {
                    biomeName = "Grass";
                }
                else if (elevation < mountainNC[1])
                {
                    biomeName = "Grass";
                }
                else
                {
                    biomeName = "Ice";
                }
            }
        }
        else if (temp < tempNC[1])
        {
            if (rain < rainNC[0])
            {
                if (elevation < mountainNC[0])
                {
                    biomeName = "Desert";
                }
                else if (elevation < mountainNC[1])
                {
                    biomeName = "Grass";
                }
                else
                {
                    biomeName = "Grass";
                }
            }
            else if (rain < rainNC[1])
            {
                if (elevation < mountainNC[0])
                {
                    biomeName = "Grass";
                }
                else if (elevation < mountainNC[1])
                {
                    biomeName = "Grass";
                }
                else
                {
                    biomeName = "Ice";
                }
            }
            else
            {
                if (elevation < mountainNC[0])
                {
                    biomeName = "Jungle";
                }
                else if (elevation < mountainNC[1])
                {
                    biomeName = "Grass";
                }
                else
                {
                    biomeName = "Ice";
                }
            }
        }
        else
        {
            if (rain < rainNC[0])
            {
                if (elevation < mountainNC[0])
                {
                    biomeName = "Desert";
                }
                else if (elevation < mountainNC[1])
                {
                    biomeName = "Desert";
                }
                else
                {
                    biomeName = "Grass";
                }
            }
            else if (rain < rainNC[1])
            {
                if (elevation < mountainNC[0])
                {
                    biomeName = "Desert";
                }
                else if (elevation < mountainNC[1])
                {
                    biomeName = "Grass";
                }
                else
                {
                    biomeName = "Grass";
                }
            }
            else
            {
                if (elevation < mountainNC[0])
                {
                    biomeName = "Jungle";
                }
                else if (elevation < mountainNC[1])
                {
                    biomeName = "Jungle";
                }
                else
                {
                    biomeName = "Jungle";
                }
            }
        }

        return biomeName;
    }



    private static float[,] GenerateTempMap(WorldModel model, float[,] tempMap)
    {
        FresNoise noise = new FresNoise();
        int max = model.worldSizeY / 2;
        float[,] map = new float[model.worldSizeX, model.worldSizeY];
        for (int x = 0; x < model.worldSizeX; x++)
        {
            float hCount = 0f;
            for (int y = 0; y < model.worldSizeY; y++)
            {
                if (y <= max)
                {
                    float toFloat = hCount / (float)max;
                    float adjustedFloat = (toFloat + .5f * tempMap[x, y]) / 1.5f;
                    //int scaledNum = noise.ScaleFloatToInt(adjustedFloat, heightMap);

                    map[x, y] = adjustedFloat;
                    hCount++;
                }
                else
                {
                    float toFloat = hCount / (float)max;
                    float adjustedFloat = (toFloat + .5f * tempMap[x, y]) / 1.5f;
                    //int scaledNum = noise.ScaleFloatToInt(adjustedFloat, heightMap);

                    map[x, y] = adjustedFloat;
                    hCount--;
                }
            }
        }

        return map;
    }
    #region "Region Functions"
    private static void GenerateRegions(WorldModel model, float maxLakeSize, float maxIslandSize)
    {
        //throw new NotImplementedException();
        //Generate Land and Island Regions
        List<Region> regions = GetRegions(model, 1);
        regions.AddRange(GetRegions(model, 0));

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
                model.localMaps[ArrayHelper.ElementIndex(tile.x, tile.y,model.worldSizeY)].Model.region = region.name;
            }

        }

    }
    static List<Coord> GetRegionTiles(WorldModel model, int startX, int StartY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[model.worldSizeX, model.worldSizeY];
        int tileType = model.localMaps[ArrayHelper.ElementIndex(startX, StartY,model.worldSizeY)].Model.baseNum;

        string tileSeed = model.seed + startX + StartY;

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
                    if (IsInMapRange(model,x, y)) //&& (x == tile.x || y == tile.y))
                    {
                        int index = ArrayHelper.ElementIndex(x, y, model.worldSizeY);

                        if (mapFlags[x, y] == 0 && model.localMaps[index].Model.baseNum == tileType)
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

    static List<Region> GetRegions(WorldModel model, int tileType)
    {
        List<Region> regions = new List<Region>();
        int[,] mapFlags = new int[model.worldSizeX, model.worldSizeY];
        for (int x = 0; x < model.worldSizeX; x++)
        {
            for (int y = 0; y < model.worldSizeY; y++)
            {
                int index = ArrayHelper.ElementIndex(x, y, model.worldSizeY);

                if (mapFlags[x, y] == 0 && model.localMaps[index].Model.baseNum == tileType)
                {
                    string regSeed = model.seed + x + y;
                    string newRegionName = new NameGen().GenerateRegionName(regSeed);
                    Region newRegion = new Region(GetRegionTiles(model, x, y), newRegionName, tileType);
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
    #endregion
    ///----------Helper Functions
    ///

    public static bool IsInMapRange(WorldModel model, int x, int y)
    {
        if (x >= 0 && x < model.worldSizeX && y >= 0 && y < model.worldSizeY)
            return true;
        else
            return false;
    }

    internal static int[,] RandomFillMap(WorldModel model, int[,] baseMap, int randomFillPercent = 53)
    {

        System.Random randNum = new System.Random(model.seed.GetHashCode());

        for (int x = 0; x < model.worldSizeX; x++)
        {
            for (int y = 0; y < model.worldSizeY; y++)
            {
                if (x == 0 || x == model.worldSizeX - 1 || y == 0 || y == model.worldSizeY - 1)
                    baseMap[x, y] = 0;
                else
                    baseMap[x, y] = (randNum.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }

        return baseMap;
    }

    public static int[,] SmoothMap(WorldModel model, int[,] baseMap, int lDist)
    {
        for (int y = 0; y < model.worldSizeY; y++)
        {
            for (int x = 0; x < model.worldSizeX; x++)
            {
                int nbrWaterTiles = GetSurroundingWaterCount(model, baseMap, x, y, lDist);
                double powX = lDist * 2f + 1;
                int tileCount = (int)Math.Pow(powX, 2) / 2;
                if (nbrWaterTiles > tileCount + 1)
                    baseMap[x, y] = 0;
                else if (nbrWaterTiles < tileCount - 1)
                    baseMap[x, y] = 1;
            }
        }

        return baseMap;
    }
    /// <summary>
    /// returns the number of surrounding water (int 0) tiles there are around a lDist radius of int from an int array
    /// </summary>
    /// <param name="baseMap">map with ints</param>
    /// <param name="gridX">position x to check</param>
    /// <param name="gridY">position y to check</param>
    /// <param name="lDist">distance from x,y to check</param>
    /// <returns></returns>
    static int GetSurroundingWaterCount(WorldModel model, int[,] baseMap, int gridX, int gridY, int lDist)
    {
        int waterCount = 0;
        for (int nbrX = gridX - lDist; nbrX <= gridX + lDist; nbrX++)
        {
            for (int nbrY = gridY - lDist; nbrY <= gridY + lDist; nbrY++)
            {
                if (IsInMapRange(model, nbrX, nbrY))
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
