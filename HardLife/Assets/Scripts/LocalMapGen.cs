using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LocalMapGen : MonoBehaviour {

    World world;
    public LocalMap local;
    public string seed;
    public int width = 100;
    public int height = 100;

    int aveTemp;

	// Use this for initialization
	void Awake () {

        world = GetComponent<WorldGen>().world;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public LocalMap CreateLocalMap(Coord coord)
    {
        DestroyLocalMap();
        
        int x = coord.x;
        int y = coord.y;

        seed = world.seed + x + y;

        aveTemp = world.aveTempMap[x, y];
        int[,] baseMap = world.mapLayers[Array.IndexOf(world.layerNames, "Base Map")];
        int[,] biomeMap = world.mapLayers[Array.IndexOf(world.layerNames, "Biome Map")];
        int[,] mMap = world.mapLayers[Array.IndexOf(world.layerNames, "Mountain Map")];
        int biomeType = biomeMap[x, y];
        int mType = mMap[x, y];

        int[,] adjacentBaseTiles = AdjacentTiles(baseMap, x, y);
        int[,] adjacentMTiles = AdjacentTiles(mMap, x, y);

        local = new LocalMap(seed, biomeType, mType, adjacentBaseTiles, adjacentMTiles);
        return local;

    }

    private int[,] AdjacentTiles(int[,] baseMap, int x, int y)
    {
        int[,] adj = new int[3, 3];

        for (int nbrX = x - 1; nbrX <= x + 1; nbrX++)
        {
            for (int nbrY = y - 1; nbrY <= y + 1; nbrY++)
            {
                if (IsInMapRange(nbrX, nbrY))
                {
                    adj[nbrX, nbrY] = baseMap[nbrX, nbrY];
                }
                else
                {
                    adj[nbrX, nbrY] = -1; 
                }

            }
        }

        return adj;
    }
    private float[,] AdjacentTiles(float[,] baseMap, int x, int y)
    {
        float[,] adj = new float[3, 3];

        for (int nbrX = x - 1; nbrX <= x + 1; nbrX++)
        {
            for (int nbrY = y - 1; nbrY <= y + 1; nbrY++)
            {
                if (IsInMapRange(nbrX, nbrY))
                {
                    adj[nbrX, nbrY] = baseMap[nbrX, nbrY];
                }
                else
                {
                    adj[nbrX, nbrY] = baseMap[x,y];
                }

            }
        }

        return adj;
    }
    private T[,] AdjacentTiles<T>(T[,] baseMap, int x, int y)
    {
        T[,] adj = new T[3, 3];

        for (int nbrX = x - 1; nbrX <= x + 1; nbrX++)
        {
            for (int nbrY = y - 1; nbrY <= y + 1; nbrY++)
            {
                if (IsInMapRange(nbrX, nbrY))
                {
                    adj[nbrX, nbrY] = baseMap[nbrX, nbrY];
                }

            }
        }

        return adj;
    }

    public bool IsInMapRange(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return true;
        else
            return false;
    }

    private void DestroyLocalMap()
    {
        throw new NotImplementedException();
    }
}
