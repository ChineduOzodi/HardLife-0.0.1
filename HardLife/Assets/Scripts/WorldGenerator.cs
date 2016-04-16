using UnityEngine;
using System.Collections;
using System;

public class WorldGenerator : MonoBehaviour {

    public int width;
    public int height;
    [Range(0,100)]
    public int randomFillPercent;
    public string seed;
    public bool useRandomSeed;

    int[,] map;
    public GameObject[] water;
    public GameObject[] dirt;

    private Transform worldHolder;

    void Start()
    {
        GenerateMap();
        CreateWorld();
    }

    private void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();
        for (int i = 0; i < 5; i++)
            SmoothMap();
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random randNum = new System.Random(seed.GetHashCode());
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = 0;
                else
                    map[x, y] = (randNum.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int nbrWaterTiles = GetSurroundingWaterCount(x,y);

                if (nbrWaterTiles > 5)
                    map[x, y] = 0;
                else if (nbrWaterTiles < 5)
                    map[x, y] = 1;
            }
        }
    }

    int GetSurroundingWaterCount (int gridX, int gridY)
    {
        int waterCount = 0;
        for (int nbrX = gridX - 1; nbrX <=gridX + 1; nbrX++)
        {
            for (int nbrY = gridY - 1; nbrY <= gridY + 1; nbrY++)
            {
                if (nbrX >= 0 && nbrX < width && nbrY >= 0 && nbrY < height)
                {
                    if (nbrX != gridX || nbrY != gridY)
                    {
                        if (map[nbrX, nbrY] == 0)
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

    void CreateWorld()
    {
        worldHolder = new GameObject("World").transform;

        for (int x = 0; x < width; x++)
        {
                for (int y = 0; y < height; y++)
                {

                    GameObject toInstantiate = water[UnityEngine.Random.Range(0, water.Length)];

                    if (map[x,y] == 1)
                    {
                        toInstantiate = dirt[UnityEngine.Random.Range(0, dirt.Length)];
                    }

                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y), Quaternion.identity) as GameObject;

                    instance.transform.SetParent(worldHolder);
                }
        }
        
    }
}
