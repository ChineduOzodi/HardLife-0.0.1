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
        float[,] hHelperMap = GenerateHorHelperMap();
        float[,] vHelperMap = GenerateVertHelperMap();
        float[,] cHelperMap = GenerateCorHelperMap(hHelperMap, vHelperMap);

        elevationMap = heightMap;
    }

    private float[,] GenerateCorHelperMap(float[,] hHelperMap, float[,] vHelperMap)
    {
        float[,] map = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (hHelperMap[x, y] + vHelperMap[x, y]) / 2f;
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
        for (int y = 0; y < width; y++)
        {
            float hCount = 0f;
            for (int x = 0; x < height; x++)
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

}
