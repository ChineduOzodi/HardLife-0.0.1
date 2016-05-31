using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class World{
    /// <summary>
    /// The World Class that can generate and create a game world based on inputed parameters
    /// </summary>
	#region "Declarations"
    public string worldName;
    internal int saveNum = 1;

	internal Vector2 worldSize;
	internal Vector2 localSize;
    [Range(45, 60)]
    public int randomFillPercent = 53;
    internal string seed = null;

//    int iceID = 1;
//    int grassID = 2;
//    int jungleID = 3;
//    int desertID = 4;

    public LocalMap[,] localMaps;
	public LocalMap localMap;
	public int[][,] mapLayers;
	public string[] layerNames;

    
    private float[] mountainNC =  { .5f, .75f}; //Mountain noise conversion scale
    private float mountainScale = 10f;
    private float[] rainNC = { .33f, .66f};
    private float rainScale = 7f;
    private float[] tempNC = { .33f, .5f};
    private float tempScale = 5f;
    private int numLayers;
    
    public NameGen nameGen = new NameGen();
	#endregion
    ///-----initializer(s)
    public World()
    {    
        layerNames = new [] { "Base Map", "Temperature Map", "Rain Map", "Mountain Map", "Biome Map"};
        SetLayers(layerNames);
        localMaps = new LocalMap[width, height];

    }
    //--------------Map Generation Functions----------------

    /// <summary>
    /// Generates Map based on layers, there currently needs to be at lease 4 layers
    ///This includes a Base Map, Temperature Map, Rain Map, and Mountain Map
    /// </summary>
    public void GenerateMap(string name = null)
    {
        if (seed == null || seed == "")
        {
            seed = Time.time.ToString();
        }

        if (name == null)
        {

            name = nameGen.GenerateWorldName(seed);
        }

        worldName = name;

        SetLayers(layerNames);

        GenerateLayers();
        GenerateAverageTemp();        

    }

    private void GenerateAverageTemp()
    {
		int[] tempMapModTemp = { -10, -4, 2};
		int[] tempMapModRain = { 3, 0, -3 };
		int[] tempMapModMountains = { 0, -2, -4};
		int[] biomeTemps = { 15, -5, 15, 20, 25 };

        float[,] aveTempMap = new float[width, height];
        int[,] tempMap = mapLayers[Array.IndexOf(layerNames, "Temperature Map")];
        int[,] biomeMap = mapLayers[Array.IndexOf(layerNames, "Biome Map")];
        int[,] rainMap = mapLayers[Array.IndexOf(layerNames, "Rain Map")];
        int[,] mMap = mapLayers[Array.IndexOf(layerNames, "Mountain Map")];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                aveTempMap[x, y] = tempMapModTemp[tempMap[x,y]] + tempMapModRain[rainMap[x, y]] + biomeTemps[biomeMap[x,y]] + tempMapModMountains[mMap[x,y]];
            }
            
        }
    }

    private int[,] GenerateBiomes()
    {
        int[,] map = new int[width, height];
        int[,] baseMap = mapLayers[Array.IndexOf(layerNames, "Base Map")];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if( baseMap[x, y] == 1)
                {
                    
                    int tileTemp = mapLayers[Array.IndexOf(layerNames, "Temperature Map")][x, y];
                    int tileRain = mapLayers[Array.IndexOf(layerNames, "Rain Map")][x, y];

                    map[x, y] = BiomeID(tileTemp,tileRain);
                }
                else
                    map[x, y] = 0;

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
    private int BiomeID(int tileTemp, int tileRain)
    {
        int biomeID = 10;
        if (tileTemp == 0)
        {
            if (tileRain == 0)
            {
                return iceID;
            }
            else if (tileRain == 1)
            {
                return iceID;
            }
            else if (tileRain == 2)
            {
                return iceID;
            }
        }
        else if (tileTemp == 1)
        {
            if (tileRain == 0)
            {
                return grassID;
            }
            else if (tileRain == 1)
            {
                return grassID;
            }
            else if (tileRain == 2)
            {
                return jungleID;
            }
        }
        else if (tileTemp == 2)
        {
            if (tileRain == 0)
            {
                return desertID;
            }
            else if (tileRain == 1)
            {
                return grassID;
            }
            else if (tileRain == 2)
            {
                return jungleID;
            }
        }
        return biomeID;
    }

    private void GenerateLayers()
    {
        for (int i = 0; i < numLayers; i++)
        {
            string layerName = layerNames[i];
            FresNoise noise = new FresNoise();

            if (layerName == "Base Map")
            {
                mapLayers[i] = RandomFillMap(mapLayers[i]);
                SmoothMap(mapLayers[i], 2);
                SmoothMap(mapLayers[i], 1);
            }
            else if (layerName == "Temperature Map")
            {
                mapLayers[i] = GenerateTempMap(noise.CalcNoise(width, height, seed, tempScale), tempNC);
            }
            else if (layerName == "Mountain Map")
            {
                int[,] map = noise.CalcNoise(width, height, mountainNC, seed, mountainScale);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (mapLayers[Array.IndexOf(layerNames, "Base Map")][x,y] == 1)
                        {
                                mapLayers[i][x, y] = map[x, y];
                        }
                    }
                }

            }
            else if (layerName == "Rain Map")
            {
                mapLayers[i] = noise.CalcNoise(width, height, rainNC, seed, rainScale);
            }
            else if (layerName == "Biome Map")
            {
                mapLayers[i] = GenerateBiomes();
            }

        }
    }

    private int[,] GenerateTempMap(float[,] tempMap, float[] heightMap)
    {
        FresNoise noise = new FresNoise();
        int max = height / 2;
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            float hCount = 0f;
            for (int y = 0; y < height; y++)
            {
                if (y <= max)
                {
                    float toFloat = hCount / (float)max;
                    float adjustedFloat = (toFloat + .5f * tempMap[x, y])/1.5f;
                    int scaledNum = noise.ScaleFloatToInt(adjustedFloat, heightMap);
                    
                    map[x, y] = scaledNum;
                    hCount++;
                }
                else
                {
                    float toFloat = hCount/ (float)max;
                    float adjustedFloat = (toFloat + .5f * tempMap[x, y]) / 1.5f;
                    int scaledNum = noise.ScaleFloatToInt(adjustedFloat, heightMap);

                    map[x, y] = scaledNum;
                    hCount--;
                }
            }
        }

        return map;
    }
    ///------- Region Functions
    ///
    
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

    public bool IsInMapRange(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return true;
        else
            return false;
    }

    internal int[,] RandomFillMap(int[,] baseMap)
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

    public int[,] SmoothMap(int[,] baseMap, int lDist)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int nbrWaterTiles = GetSurroundingWaterCount(baseMap, x, y, lDist);
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
    int GetSurroundingWaterCount(int[,] baseMap, int gridX, int gridY, int lDist)
    {
        int waterCount = 0;
        for (int nbrX = gridX - lDist; nbrX <= gridX + lDist; nbrX++)
        {
            for (int nbrY = gridY - lDist; nbrY <= gridY + lDist; nbrY++)
            {
                if (IsInMapRange(nbrX, nbrY))
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
