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
    public string name;
    internal int saveNum = 1;

	internal Vector2 worldSize;
    internal Vector2 localSize;
    internal float nodeRadius;
    internal float nodeDiameter;
    int worldSizeX, worldSizeY, localSizeX, localSizeY;
    [Range(45, 60)]
    public int randomFillPercent = 53;
    internal string seed = null;

//    int iceID = 1;
//    int grassID = 2;
//    int jungleID = 3;
//    int desertID = 4;

    public LocalMap[,] localMaps;
	public LocalMap localMap;
    
    public NameGen nameGen = new NameGen();
	#endregion
    ///-----initializer(s)
    public World(Vector2 _worldSize, Vector2 _localSize, float _nodeRadius)
    {
        //layerNames = new [] { "Base Map", "Temperature Map", "Rain Map", "Mountain Map", "Biome Map"};
        //SetLayers(layerNames);
        worldSize = _worldSize;
        localSize = _localSize;
        nodeRadius = _nodeRadius;
        nodeDiameter = nodeRadius * 2;
        worldSizeX = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        worldSizeY = Mathf.RoundToInt(worldSize.y / nodeDiameter);
        localMaps = new LocalMap[worldSizeX, worldSizeY];

        GenerateMap();

    }
    //--------------Map Generation Functions----------------

    /// <summary>
    /// Generates Map based on layers, there currently needs to be at lease 4 layers
    ///This includes a Base Map, Temperature Map, Rain Map, and Mountain Map
    /// </summary>
    public void GenerateMap(string _seed = null, string _name = null)
    {
        if (_seed == null || _seed == "")
        {
            seed = Time.time.ToString();
        }

        if (_name == null)
        {

            name = nameGen.GenerateWorldName(seed);
        }
        else
        {
            name = _name;
        }

        //SetLayers(layerNames);

        GenerateLayers();
        //GenerateAverageTemp();        

    }

    private void GenerateLayers()
    {
        FresNoise noise = new FresNoise();

        //Declarations
        
        float mountainScale = 10f;
        
        float rainScale = 7f;
        
        float tempScale = 5f;

        //BaseMap Generation
        int[,] baseMap = RandomFillMap(new int[worldSizeX, worldSizeY]);
        baseMap = SmoothMap(baseMap, 2);
        baseMap = SmoothMap(baseMap, 1);

        //Temperature Map Generation
        float[,] tempMap = GenerateTempMap(noise.CalcNoise(worldSizeX, worldSizeY, seed+"temp", tempScale));

        //Mountain Map Generation
        float[,] mMap = noise.CalcNoise(worldSizeX, worldSizeY,  seed+"elevation", mountainScale);

        //Rain Map Generation
        float[,] rainMap = noise.CalcNoise(worldSizeX, worldSizeY, seed+"rain", rainScale);

        //Biome Map
        string[,] biomeMap = GenerateBiomes(baseMap, tempMap,mMap,rainMap);

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                //Set Local Map Declarations
                localMaps[x, y].biome = biomeMap[x, y];
                localMaps[x, y].aveTemp = tempMap[x, y];
                localMaps[x, y].elevation = mMap[x, y];
                localMaps[x, y].rain = rainMap[x, y];
            }
        }
    }

  //  private void GenerateAverageTemp()
  //  {
		//int[] tempMapModTemp = { -10, -4, 2};
		//int[] tempMapModRain = { 3, 0, -3 };
		//int[] tempMapModMountains = { 0, -2, -4};
		//int[] biomeTemps = { 15, -5, 15, 20, 25 };

  //      float[,] aveTempMap = new float[worldSizeX, worldSizeY];
  //      int[,] tempMap = mapLayers[Array.IndexOf(layerNames, "Temperature Map")];
  //      int[,] biomeMap = mapLayers[Array.IndexOf(layerNames, "Biome Map")];
  //      int[,] rainMap = mapLayers[Array.IndexOf(layerNames, "Rain Map")];
  //      int[,] mMap = mapLayers[Array.IndexOf(layerNames, "Mountain Map")];
  //      for (int x = 0; x < worldSizeX; x++)
  //      {
  //          for (int y = 0; y < worldSizeY; y++)
  //          {
  //              aveTempMap[x, y] = tempMapModTemp[tempMap[x,y]] + tempMapModRain[rainMap[x, y]] + biomeTemps[biomeMap[x,y]] + tempMapModMountains[mMap[x,y]];
  //          }
            
  //      }
  //  }

    private string[,] GenerateBiomes(int[,] baseMap, float[,] tempMap, float[,] elevMap, float[,] rainMap)
    {
        string[,] map = new string[worldSizeX, worldSizeY];

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {

                if( baseMap[x, y] == 1)
                {
                    map[x, y] = BiomeName(tempMap[x,y],rainMap[x,y], elevMap[x,y]);
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
    private string BiomeName(float temp, float rain, float elevation)
    {
        float[] mountainNC = { .5f, .75f }; //Mountain noise conversion scale
        float[] rainNC = { .33f, .66f };
        float[] tempNC = { .33f, .5f };
    }

    

    private float[,] GenerateTempMap(float[,] tempMap)
    {
        FresNoise noise = new FresNoise();
        int max = worldSizeY / 2;
        float[,] map = new float[worldSizeX, worldSizeY];
        for (int x = 0; x < worldSizeX; x++)
        {
            float hCount = 0f;
            for (int y = 0; y < worldSizeY; y++)
            {
                if (y <= max)
                {
                    float toFloat = hCount / (float)max;
                    float adjustedFloat = (toFloat + .5f * tempMap[x, y])/1.5f;
                    //int scaledNum = noise.ScaleFloatToInt(adjustedFloat, heightMap);
                    
                    map[x, y] = adjustedFloat;
                    hCount++;
                }
                else
                {
                    float toFloat = hCount/ (float)max;
                    float adjustedFloat = (toFloat + .5f * tempMap[x, y]) / 1.5f;
                    //int scaledNum = noise.ScaleFloatToInt(adjustedFloat, heightMap);

                    map[x, y] = adjustedFloat;
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
    //private void SetLayers(string[] layers)
    //{
    //    numLayers = layers.Length;
    //    mapLayers = new int[numLayers][,];

    //    for (int i = 0; i < numLayers; i++)
    //    {
    //        mapLayers[i] = new int[worldSizeX, worldSizeY];
    //    }
        
    //}

    public bool IsInMapRange(int x, int y)
    {
        if (x >= 0 && x < worldSizeX && y >= 0 && y < worldSizeY)
            return true;
        else
            return false;
    }

    internal int[,] RandomFillMap(int[,] baseMap)
    {

        System.Random randNum = new System.Random(seed.GetHashCode());

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (x == 0 || x == worldSizeX - 1 || y == 0 || y == worldSizeY - 1)
                    baseMap[x, y] = 0;
                else
                    baseMap[x, y] = (randNum.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }

        return baseMap;
    }

    public int[,] SmoothMap(int[,] baseMap, int lDist)
    {
        for (int y = 0; y < worldSizeY; y++)
        {
            for (int x = 0; x < worldSizeX; x++)
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
