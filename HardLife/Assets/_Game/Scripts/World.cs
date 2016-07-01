using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

[Serializable]
public class World: Model{
    /// <summary>
    /// The World Class that can generate and create a game world based on inputed parameters
    /// </summary>
	#region "Declarations"
    public string name;
    internal int saveNum = 1;

    //time info
    public Date date = new Date(0);    

	internal Vector2 worldSize;
    internal Vector2 localSize;
    internal float nodeRadius;
    internal float nodeDiameter;
    internal int worldSizeX, worldSizeY, localSizeX, localSizeY;
    [Range(45, 60)]
    public int randomFillPercent = 53;

    //[Range(0,100)]
    //public int waterFillPercent = 75; //THis is in case you wnat to use the elvation Map to determine land and water instead of baseMap
    internal string seed = null;

    private float maxLakeSize;
    private float maxIslandSize;

    public LocalMap[,] localMaps;
	public LocalMap localMap;
    
    public NameGen nameGen = new NameGen();
    internal Vector3 worldBottomLeft;

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
        localSizeX = Mathf.RoundToInt(localSize.x / nodeDiameter);
        localSizeY = Mathf.RoundToInt(localSize.y / nodeDiameter); 

        worldBottomLeft = - Vector3.right * worldSize.x / 2 - Vector3.up *  worldSize.y / 2;

        maxLakeSize = (worldSize.x * worldSize.y) / 200;
        maxIslandSize = (worldSize.x * worldSize.y) / 1000;

        localMaps = new LocalMap[worldSizeX, worldSizeY];
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
        GenerateRegions();

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

        //Elevation Map Generation
        float[,] elevMap = noise.CalcNoise(worldSizeX, worldSizeY,  seed+"elevation", mountainScale);

        //Rain Map Generation
        float[,] rainMap = noise.CalcNoise(worldSizeX, worldSizeY, seed+"rain", rainScale);

        //Biome Map
        string[,] biomeMap = GenerateBiomes(baseMap, tempMap,elevMap,rainMap);

		tempMap = GenerateAverageTemp(biomeMap, tempMap,elevMap,rainMap);

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                //Set Local Map Declarations
                localMaps[x, y] = new LocalMap(x,y,seed);
                localMaps[x, y].biome = biomeMap[x, y];
                localMaps[x, y].aveTemp = tempMap[x, y];
                localMaps[x, y].curTemp = tempMap[x, y];
                localMaps[x, y].elevation = elevMap[x, y];
                localMaps[x, y].rain = rainMap[x, y];
                localMaps[x, y].baseNum = baseMap[x, y];

                //Set Mounatin Level
                if (localMaps[x, y].elevation < .5)
                {
                    localMaps[x, y].mountainLevel = "Flat";
                }
                else if (localMaps[x, y].elevation < .75)
                {
                    localMaps[x, y].mountainLevel = "Hills";
                }else 
                {
                    localMaps[x, y].mountainLevel = "Mountains";
                }
            }
        }
    }

	private float[,] GenerateAverageTemp(string[,] biomeMap,float[,] tempMap, float[,] elevMap, float[,] rainMap)
    {
        //Possible Biome Names: Unknown, Ice, Grass, Desert, Jungle, Water
		float tempScale = 5;
		float rainScale = 3;
		float elevScale = 4;
		Dictionary<string,float> biomeTemps = new Dictionary<string,float> ();
		biomeTemps.Add ("Grass", 15);
		biomeTemps.Add ("Ice", -5);
		biomeTemps.Add ("Jungle", 20);
		biomeTemps.Add ("Desert", 25);
        biomeTemps.Add("Water", 25);

        float[,] map = new float[worldSizeX, worldSizeY];

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
				map [x, y] = biomeTemps [biomeMap [x, y]] + (tempMap [x, y] * tempScale * 2 - tempScale)
				- (rainMap [x, y] * rainScale)
				- (elevMap [x, y] * elevScale);
            }
            
        }

		return map;
    }

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
        //Possible Biome Names: Unknown, Ice, Grass, Desert, Jungle
		string biomeName = "Unknown";
        float[] mountainNC = { .5f, .75f }; //Mountain noise conversion scale
        float[] rainNC = { .33f, .66f };
        float[] tempNC = { .4f, .75f };
		if (temp < tempNC [0]) {
			if (rain < rainNC [0]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Ice";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Ice";
				} else {
					biomeName = "Ice";
				}
			} else if (rain < rainNC [1]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Grass";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Ice";
				} else {
					biomeName = "Ice";
				}
			} else {
				if (elevation < mountainNC [0]) {
					biomeName = "Grass";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Ice";
				}
			}
		} else if (temp < tempNC [1]) {
			if (rain < rainNC [0]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Desert";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Grass";
				}
			} else if (rain < rainNC [1]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Grass";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Ice";
				}
			} else {
				if (elevation < mountainNC [0]) {
					biomeName = "Jungle";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Ice";
				}
			}
		} else {
			if (rain < rainNC [0]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Desert";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Desert";
				} else {
					biomeName = "Grass";
				}
			} else if (rain < rainNC [1]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Desert";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Grass";
				}
			} else {
				if (elevation < mountainNC [0]) {
					biomeName = "Jungle";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Jungle";
				} else {
					biomeName = "Jungle";
				}
			}
		}

		return biomeName;
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
    #region "Region Functions"
    private void GenerateRegions()
    {
        //throw new NotImplementedException();
        //Generate Land and Island Regions
        List<Region> regions = GetRegions(1);
        regions.AddRange(GetRegions(0));

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
                localMaps[tile.x, tile.y].region = region.name;
            }

        }

    }
    List<Coord> GetRegionTiles(int startX, int StartY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[worldSizeX, worldSizeY];
        int tileType = localMaps[startX, StartY].baseNum;

        string tileSeed = seed + startX + StartY;

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
                    if (IsInMapRange(x, y)) //&& (x == tile.x || y == tile.y))
                    {
                        if (mapFlags[x, y] == 0 && localMaps[x, y].baseNum == tileType)
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

    List<Region> GetRegions(int tileType)
    {
        List<Region> regions = new List<Region>();
        int[,] mapFlags = new int[worldSizeX, worldSizeY];
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (mapFlags[x, y] == 0 && localMaps[x, y].baseNum == tileType)
                {
                    string regSeed = seed + x + y;
                    string newRegionName = nameGen.GenerateRegionName(regSeed);
                    Region newRegion = new Region(GetRegionTiles(x, y), newRegionName, tileType);
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
