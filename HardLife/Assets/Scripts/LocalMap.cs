using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class LocalMap { //TODO Add comments to class
    //General Variables
    internal Vector3 worldPosition;
    internal Vector2 localSize;
    int localSizeX, localSizeY, worldPositionX,worldPostionY;

    public string seed;
    internal string region;
    internal int baseNum;
    internal string biome;
	internal string mountainLevel;
    internal float elevation;
    internal float rain;
    internal float aveTemp;

    internal float lastUpdated = 0f;

    //Local Map Variables
    public float heightMapScale = 10f;
    public float baseMapScale = 1f;
    public float mountainScale = .2f;
    
    public float[,] elevationMap;
    public Tile[,] baseMap;
    public Road[,] roadMap;
    public Items[,] itemMap;
    public Roof[,] roofMap;
    public Items[,] skyMap;

    private float[,] heightMapHelper1;
    private float[,] heightMapVerticalHelper;
    private FresNoise noise;

    private float[] baseMapNC = { .5f,.55f, .85f,.9f };
    private float[] itemMapNC = { .33f, .66f };
    private float nodeRadius;
    private float nodeDiameter;

    public LocalMap(int _worldPositionX, int _worldPositionY)
    {
        worldPositionX = _worldPositionX;
        worldPostionY = _worldPositionY;
    }
    public void GenerateLocalMap(string seed, int biomeType, int elevation, int[,] adjacentBaseTiles, int[,] adjacentElevTiles, Vector2 _localSize, float _nodeRadius)
    {
        this.seed = seed;
        localSize = _localSize;

        nodeRadius = _nodeRadius;
        nodeDiameter = nodeRadius * 2;

        localSizeX = Mathf.RoundToInt(localSize.x / nodeDiameter);
        localSizeY = Mathf.RoundToInt(localSize.y / nodeDiameter);

        noise = new FresNoise();
        float[,] heightMap = noise.CalcNoise(localSizeX, localSizeY, seed, heightMapScale);
        float[,] hMap = heightMap;
        float[,] hHelperMap = GenerateHorHelperMap();
        float[,] vHelperMap = GenerateVertHelperMap();
        float[,] cHelperMap = GenerateCorHelperMap(hHelperMap, vHelperMap);
        baseMap = new Tile[localSizeX, localSizeY];

        bool hasShores = false;
        foreach (int baseNum in adjacentBaseTiles)
        {
            if (baseNum == 0)
            {
                hasShores = true;
                break;
            }
        }

        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                if (x <= localSizeX / 2 && y <= localSizeY/2)
                {
                    if (adjacentBaseTiles[0,1] == 0)
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[0, 1] < elevation)
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[0, 1] > elevation)
                    {
                        heightMap[x, y] += (1 - vHelperMap[x, y])* mountainScale;
                    }
                    if (adjacentBaseTiles[1, 0] == 0)
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y])*mountainScale;
                    }
                    if ( adjacentElevTiles[1, 0] < elevation)
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[1, 0] > elevation)
                    {
                        heightMap[x, y] += (1 - hHelperMap[x, y])*mountainScale;
                    }
                    if (adjacentBaseTiles[0, 0] == 0 )
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[0, 0] < elevation)
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[0, 0] > elevation)
                    {
                        heightMap[x, y] += (1 - cHelperMap[x, y])*mountainScale;
                    }
                }
                else if (x <= localSizeX / 2 && y > localSizeY / 2)
                {
                    if (adjacentBaseTiles[0, 1] == 0)
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[0, 1] < elevation)
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[0, 1] > elevation)
                    {
                        heightMap[x, y] += (1 - vHelperMap[x, y])*mountainScale;
                    }
                    if (adjacentBaseTiles[1, 2] == 0)
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[1, 2] < elevation)
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[1, 2] > elevation)
                    {
                        heightMap[x, y] += (1 - hHelperMap[x, y])*mountainScale;
                    }
                    if (adjacentBaseTiles[0, 2] == 0 )
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[0, 2] < elevation)
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[0, 2] > elevation)
                    {
                        heightMap[x, y] += (1 - cHelperMap[x, y])*mountainScale;
                    }
                }
                else if (x > localSizeX / 2 && y <= localSizeY / 2)
                {
                    if (adjacentBaseTiles[2, 1] == 0 )
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[2, 1] < elevation)
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[2, 1] > elevation)
                    {
                        heightMap[x, y] += (1 - vHelperMap[x, y])*mountainScale;
                    }
                    if (adjacentBaseTiles[1, 0] == 0 )
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[1, 0] < elevation)
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[1, 0] > elevation)
                    {
                        heightMap[x, y] += (1 - hHelperMap[x, y])*mountainScale;
                    }
                    if (adjacentBaseTiles[2, 0] == 0 )
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[2, 0] < elevation)
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[2, 0] > elevation)
                    {
                        heightMap[x, y] += (1 - cHelperMap[x, y])*mountainScale;
                    }
                }
                else if (x > localSizeX / 2 && y > localSizeY / 2)
                {
                    if (adjacentBaseTiles[2, 1] == 0 )
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[2, 1] < elevation)
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[2, 1] > elevation)
                    {
                        heightMap[x, y] += (1 - vHelperMap[x, y])*mountainScale;
                    }
                    if (adjacentBaseTiles[1, 2] == 0)
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[1, 2] < elevation)
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[1, 2] > elevation)
                    {
                        heightMap[x, y] += (1 - hHelperMap[x, y])*mountainScale;
                    }
                    if (adjacentBaseTiles[2, 2] == 0)
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y])*baseMapScale;
                    }
                    if (adjacentElevTiles[2, 2] < elevation)
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y]) * mountainScale;
                    }
                    else if (adjacentElevTiles[2, 2] > elevation)
                    {
                        heightMap[x, y] += (1 - cHelperMap[x, y])*mountainScale;
                    }
                }
            }
        }

        heightMap = MapScaler(heightMap);
        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                baseMap[x, y] = new Tile(x, y, noise.ScaleFloatToInt(heightMap[x, y], baseMapNC));
                
                
                if (biomeType == 0) //water local map
                {
                    baseMap[x, y].type = "water";
                }
                else if (baseMap[x, y].id >= 4)
                {
                    baseMap[x, y].type = "rock";
                }
                else if (biomeType == 1)
                {

                    baseMap[x, y].type = "ice";
                }
                else if (biomeType == 2)
                {

                    baseMap[x, y].type = "grass";
                }
                else if (biomeType == 3)
                {

                    baseMap[x, y].type = "jungle";
                }
                else if (biomeType == 4)
                {

                    baseMap[x, y].type = "sand";
                }

                if (hasShores && baseMap[x,y].id == 0)
                {
                    baseMap[x, y].type = "water";
                }
                else if (hasShores && baseMap[x, y].id == 1 && biomeType != 1 && biomeType != 0 && biomeType != 4)
                {
                    baseMap[x, y].type = "sand";
                }

            }
                

        }
                

        elevationMap = heightMap;
    }

    private float[,] MapScaler(float[,] map)
    {
        throw new NotImplementedException();
        //float max = map[0, 0];
        //float min = map[0, 0];
        //for (int x = 0; x < localSizeX; x++)
        //{
        //    for (int y = 0; y < localSizeY; y++)
        //    {
        //        if (map[x, y] > max)
        //            max = map[x, y];
        //        if (map[x, y] < min)
        //            min = map[x, y];
        //    }
        //}

        //for (int x = 0; x < localSizeX; x++)
        //{
        //    for (int y = 0; y < localSizeY; y++)
        //    {
        //        map[x, y] = (map[x, y] - min) / (max-min);
        //    }
        //}

        //return map;

    }

    private float[,] GenerateCorHelperMap(float[,] hHelperMap, float[,] vHelperMap)
    {
        throw new NotImplementedException();
        //float[,] map = new float[localSizeX, localSizeY];
        //for (int x = 0; x < localSizeX; x++)
        //{
        //    for (int y = 0; y < localSizeY; y++)
        //    {
        //        float newVar = (hHelperMap[x, y] + vHelperMap[x, y]);
        //        if (newVar > 1f)
        //        {
        //            newVar = 1f;
        //        }
        //        map[x, y] = newVar;
        //    }
        //}

        //return map;
    }

    private float[,] GenerateHorHelperMap()
    {
        throw new NotImplementedException();
        //int max = localSizeY / 2;
        //float[,] map = new float[localSizeX, localSizeY];
        //for (int x = 0; x < localSizeX; x++)
        //{
        //    float hCount = 0f;
        //    for (int y = 0; y < localSizeY; y++)
        //    {
        //        if (y <= max)
        //        {
        //            float toFloat = hCount / (float)max;

        //            map[x, y] = toFloat;
        //            hCount++;
        //        }
        //        else
        //        {
        //            float toFloat = hCount / (float)max;

        //            map[x, y] = toFloat;
        //            hCount--;
        //        }
        //    }
        //}

        //return map;
    }

    private float[,] GenerateVertHelperMap()
    {
        throw new NotImplementedException();
        //int max = localSizeY / 2;
        //float[,] map = new float[localSizeX, localSizeY];
        //for (int y = 0; y < localSizeY; y++)
        //{
        //    float hCount = 0f;
        //    for (int x = 0; x < localSizeX; x++)
        //    {
        //        if (x <= max)
        //        {
        //            float toFloat = hCount / (float)max;

        //            map[x, y] = toFloat;
        //            hCount++;
        //        }
        //        else
        //        {
        //            float toFloat = hCount / (float)max;

        //            map[x, y] = toFloat;
        //            hCount--;
        //        }
        //    }
        //}

        //return map;
    }

}
