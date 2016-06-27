using UnityEngine;
using System.Collections;
using System;

//[Serializable]
public class LocalMap { //TODO Add comments to class
    //General Variables
    internal World world;
    internal Vector3 worldPosition;
    internal Vector2 localSize;
    internal int localSizeX, localSizeY, worldMapPositionX,worldMapPositionY;

    public string seed;
    internal string region;
    internal int baseNum;
    internal string biome;
	internal string mountainLevel;
    internal float elevation;
    internal float rain;
    internal float aveTemp;

    bool hasShores = false;

    internal float lastUpdated = 0f;

    //Local Map Variables
    public float heightMapScale = 10f;
    public float baseMapScale = 1f;
    public float mountainScale = .2f;
    
    public float[,] elevationMap;
    public Tile[,] baseMap;
    public Road[,] roadMap;
    public GObject[,] objectMap;
    public Roof[,] roofMap;
    public Items[,] skyMap;

    private float[,] heightMapHelper1;
    private float[,] heightMapVerticalHelper;
    private FresNoise noise;

    private float[] baseMapNC = { .5f,.55f, .85f,.9f };
    private float[] itemMapNC = { .33f, .66f };
    private float nodeRadius;
    private float nodeDiameter;
    internal float curTemp;
    internal Vector3 worldBottomLeft;

    public LocalMap(int _worldPositionX, int _worldPositionY, string _seed)
    {
        worldMapPositionX = _worldPositionX;
        worldMapPositionY = _worldPositionY;

        seed = _seed + worldMapPositionX + worldMapPositionY;

        
    }
    public void GenerateLocalMap( World _world)
    {
        world = _world;

        localSize = world.localSize;

        nodeRadius = world.nodeRadius;
        nodeDiameter = nodeRadius * 2;

        localSizeX = world.localSizeX;
        localSizeY = world.localSizeY;

        worldBottomLeft = Vector3.right * world.localSize.x / 2 - Vector3.up * world.localSize.y / 2;

        // Setup Helper Maps elevation map
        noise = new FresNoise();
        float[,] heightMap = noise.CalcNoise(localSizeX, localSizeY, seed, heightMapScale);
        float[,] hMap = heightMap;
        float[,] hHelperMap = GenerateHorHelperMap();
        float[,] vHelperMap = GenerateVertHelperMap();
        float[,] cHelperMap = GenerateCorHelperMap(hHelperMap, vHelperMap);

        LocalMap[,] adjacentLocalMaps = AdjacentLocalMaps();

        //Check if has shores
        foreach( LocalMap localMap in adjacentLocalMaps)
        {
            if (localMap.biome == "Water")
            {
                hasShores = true;
                break;
            }
        }

        #region "Heigh Map Generation"
        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                if (x <= localSizeX / 2 && y <= localSizeY/2) //Bottom Left Generation
                {
                    //Adjacent Elevation Application
                    heightMap[x, y] += (1 - vHelperMap[x, y]) * (adjacentLocalMaps[0, 1].elevation - elevation);
                    heightMap[x, y] += (1 - hHelperMap[x, y]) * (adjacentLocalMaps[1, 0].elevation - elevation);
                    heightMap[x, y] += (1 - cHelperMap[x, y]) * (adjacentLocalMaps[0, 0].elevation - elevation);

                    if (adjacentLocalMaps[0,1].biome == "Water") //Water application
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentLocalMaps[1, 0].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentLocalMaps[0, 0].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y])*baseMapScale;
                    }
                }
                else if (x <= localSizeX / 2 && y > localSizeY / 2)
                {
                    //Adjacent Elevation Application
                    heightMap[x, y] += (1 - vHelperMap[x, y]) * (adjacentLocalMaps[0, 1].elevation - elevation);
                    heightMap[x, y] += (1 - hHelperMap[x, y]) * (adjacentLocalMaps[1, 2].elevation - elevation);
                    heightMap[x, y] += (1 - cHelperMap[x, y]) * (adjacentLocalMaps[0, 2].elevation - elevation);

                    if (adjacentLocalMaps[0, 1].biome == "Water") //Water Application
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentLocalMaps[1, 2].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentLocalMaps[0, 2].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y])*baseMapScale;
                    }
                }
                else if (x > localSizeX / 2 && y <= localSizeY / 2)
                {
                    //Adjacent Elevation Application
                    heightMap[x, y] += (1 - vHelperMap[x, y]) * (adjacentLocalMaps[2, 1].elevation - elevation);
                    heightMap[x, y] += (1 - hHelperMap[x, y]) * (adjacentLocalMaps[1, 0].elevation - elevation);
                    heightMap[x, y] += (1 - cHelperMap[x, y]) * (adjacentLocalMaps[2, 0].elevation - elevation);

                    if (adjacentLocalMaps[2, 1].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentLocalMaps[1, 0].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentLocalMaps[2, 0].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y])*baseMapScale;
                    }
                }
                else if (x > localSizeX / 2 && y > localSizeY / 2)
                {
                    //Adjacent Elevation Application
                    heightMap[x, y] += (1 - vHelperMap[x, y]) * (adjacentLocalMaps[2, 1].elevation - elevation);
                    heightMap[x, y] += (1 - hHelperMap[x, y]) * (adjacentLocalMaps[1, 2].elevation - elevation);
                    heightMap[x, y] += (1 - cHelperMap[x, y]) * (adjacentLocalMaps[2, 2].elevation - elevation);

                    if (adjacentLocalMaps[2, 1].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentLocalMaps[1, 2].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentLocalMaps[2, 2].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y])*baseMapScale;
                    }
                }
            }
        }
        #endregion
        heightMap = MapScaler(heightMap);               

        elevationMap = heightMap; //Set local elevation map

        CreateBaseMap();
        CreateObjectMap();

    }

    private void CreateObjectMap()
    {
        objectMap = new GObject[localSizeX, localSizeY];
        System.Random random = new System.Random(seed.GetHashCode());

        double treeLikeliness = .02; //Probaility of loading items
        double bushLikeliness = .05;
        double boulderLikeliness = .01;

        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * world.nodeDiameter + world.nodeRadius) + Vector3.up * (y * world.nodeDiameter + world.nodeRadius);

                if (biome != "Water") //water local map
                {
                    if (baseMap[x, y].type == "Grass")
                    {
                        if (random.NextDouble() < treeLikeliness)
                        {
                            objectMap[x, y] = new Tree("normaltree", new Date(-random.Next(100) * Date.Year), worldPoint,x,y);
                        }
                        else if (random.NextDouble() < bushLikeliness)
                        {
                            objectMap[x, y] = new Tree("normalbush", new Date(-random.Next(24) * Date.Year),worldPoint,x,y);
                        }
                        else if (random.NextDouble() < boulderLikeliness)
                        {
                            objectMap[x, y] = new Items("boulder", worldPoint,x,y);
                        }
                        
                    }
                    else if (baseMap[x, y].type == "Rock")
                    {
                        objectMap[x, y] = new Items("rock",worldPoint, x,y);
                    }

                }

            }


        }
    }

    private void CreateBaseMap()
    {
        baseMap = new Tile[localSizeX, localSizeY];

        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * world.nodeDiameter + world.nodeRadius) + Vector3.up * (y * world.nodeDiameter + world.nodeRadius);

                baseMap[x, y] = new Tile(worldPoint, x, y, noise.ScaleFloatToInt(elevationMap[x, y], baseMapNC));
                
                
                if (biome == "Water") //water local map
                {
                    baseMap[x, y].type = "Water";
                }
                else if (baseMap[x, y].id >= 4)
                {
                    baseMap[x, y].type = "Rock";
                }
                else if (biome == "Ice")
                {

                    baseMap[x, y].type = "Ice";
                }
                else if (biome == "Grass")
                {

                    baseMap[x, y].type = "Grass";
                }
                else if (biome == "Jungle")
                {

                    baseMap[x, y].type = "Grass";
                }
                else if (biome == "Desert")
                {

                    baseMap[x, y].type = "Sand";
                }

                if (hasShores && baseMap[x,y].id == 0)
                {
                    baseMap[x, y].type = "Water";
                }
                else if (hasShores && baseMap[x, y].id == 1 && biome != "Ice" && biome != "Water" && biome != "Desert")
                {
                    baseMap[x, y].type = "Sand";
                }

            }
                

        }
    }

    private LocalMap[,] AdjacentLocalMaps()
    {
        LocalMap[,] adj =  new LocalMap[3,3];

        for (int nbrX = worldMapPositionX - 1; nbrX <= worldMapPositionX + 1; nbrX++)
        {
            for (int nbrY = worldMapPositionY - 1; nbrY <= worldMapPositionY + 1; nbrY++)
            {
                if (IsInWorldMapRange(nbrX, nbrY))
                {
                    adj[nbrX + 1 - worldMapPositionX, nbrY + 1 - worldMapPositionY] = world.localMaps[nbrX, nbrY];
                }
                else
                {
                    adj[nbrX + 1 - worldMapPositionX, nbrY + 1 - worldMapPositionY] = null;
                }

            }
        }

        return adj;
    }
    private bool IsInWorldMapRange(int x, int y)
    {
        if (x >= 0 && x < world.worldSizeX && y >= 0 && y < world.worldSizeY)
            return true;
        else
            return false;
    }
    private float[,] MapScaler(float[,] map) //Scales float Maps Betwee 0 and 1
    {
        float max = map[0, 0];
        float min = map[0, 0];
        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                if (map[x, y] > max)
                    max = map[x, y];
                if (map[x, y] < min)
                    min = map[x, y];
            }
        }

        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                map[x, y] = (map[x, y] - min) / (max - min);
            }
        }

        return map;

    }

    private float[,] GenerateCorHelperMap(float[,] hHelperMap, float[,] vHelperMap)
    {
        //throw new NotImplementedException();
        float[,] map = new float[localSizeX, localSizeY];
        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                float newVar = (hHelperMap[x, y] + vHelperMap[x, y]);
                if (newVar > 1f)
                {
                    newVar = 1f;
                }
                map[x, y] = newVar;
            }
        }

        return map;
    }

    private float[,] GenerateHorHelperMap()
    {
        //throw new NotImplementedException();
        int max = localSizeY / 2;
        float[,] map = new float[localSizeX, localSizeY];
        for (int x = 0; x < localSizeX; x++)
        {
            float hCount = 0f;
            for (int y = 0; y < localSizeY; y++)
            {
                if (y <= max)
                {
                    float toFloat = hCount / (float)max;

                    map[x, y] = toFloat;
                    hCount++;
                }
                else
                {
                    float toFloat = hCount / (float)max;

                    map[x, y] = toFloat;
                    hCount--;
                }
            }
        }

        return map;
    }

    private float[,] GenerateVertHelperMap()
    {

        int max = localSizeY / 2;
        float[,] map = new float[localSizeX, localSizeY];
        for (int y = 0; y < localSizeY; y++)
        {
            float hCount = 0f;
            for (int x = 0; x < localSizeX; x++)
            {
                if (x <= max)
                {
                    float toFloat = hCount / (float)max;

                    map[x, y] = toFloat;
                    hCount++;
                }
                else
                {
                    float toFloat = hCount / (float)max;

                    map[x, y] = toFloat;
                    hCount--;
                }
            }
        }

        return map;
    }

}
