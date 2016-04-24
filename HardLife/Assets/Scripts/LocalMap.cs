using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class LocalMap {

    public int x;
    public int y;
    public string seed;
    public int width;
    public int height;
    public float heightMapScale = 10f;
    public float baseMapScale = .5f;
    public float mountainScale = .2f;

    //baselayer ID's
    int waterID = 0;
    int sandID = 1;
    int grassID = 2;
    int jungleID = 3;
    int desertID = 4;

    public float[,] elevationMap;
    public Tile[,] baseMap;
    public Road[,] roadMap;
    public Items[,] itemMap;
    public Roof[,] roofMap;
    public Items[,] skyMap;

    private float[,] heightMapHelper1;
    private float[,] heightMapVerticalHelper;
    private FresNoise noise;

    public float[] baseMapNC = { .33f, .66f };
    public float[] itemMapNC = { .33f, .66f };

    public LocalMap(string seed, int biomeType, int elevation, int[,] adjacentBaseTiles, int[,] adjacentElevTiles, int width, int height)
    {
        this.seed = seed;
        this.height = height;
        this.width = width;

        noise = new FresNoise();
        float[,] heightMap = noise.CalcNoise(width, height, seed, heightMapScale);
        float[,] hMap = heightMap;
        float[,] hHelperMap = GenerateHorHelperMap();
        float[,] vHelperMap = GenerateVertHelperMap();
        float[,] cHelperMap = GenerateCorHelperMap(hHelperMap, vHelperMap);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x <= width / 2 && y <= height/2)
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
                else if (x <= width / 2 && y > height / 2)
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
                else if (x > width / 2 && y <= height / 2)
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
                else if (x > width / 2 && y > height / 2)
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

        elevationMap = heightMap;
    }

    private float[,] MapScaler(float[,] map)
    {
        float max = map[0, 0];
        float min = map[0, 0];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] > max)
                    max = map[x, y];
                if (map[x, y] < min)
                    min = map[x, y];
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (map[x, y] - min) / (max-min);
            }
        }

        return map;

    }

    private float[,] GenerateCorHelperMap(float[,] hHelperMap, float[,] vHelperMap)
    {
        float[,] map = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
        
        int max = height / 2;
        float[,] map = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            float hCount = 0f;
            for (int y = 0; y < height; y++)
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

        int max = height / 2;
        float[,] map = new float[width, height];
        for (int y = 0; y < height; y++)
        {
            float hCount = 0f;
            for (int x = 0; x < width; x++)
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
