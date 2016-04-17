using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldGen : World {


    public GameObject whiteBlock;
    public GameObject[] water;
    public GameObject[] dirt;

    private Transform worldHolder = null;

    void Awake()
    {

        GenerateMap();

    }

    //public void CreateWorld()
    //{
    //    worldHolder = new GameObject("World").transform;

    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {

    //            GameObject toInstantiate = water[UnityEngine.Random.Range(0, water.Length)];

    //            if (baseMap[x, y] == 1)
    //            {
    //                toInstantiate = dirt[UnityEngine.Random.Range(0, dirt.Length)];
    //            }

    //            GameObject instance = Instantiate(toInstantiate, new Vector3(x, y), Quaternion.identity) as GameObject;

    //            instance.transform.SetParent(worldHolder);
    //        }
    //    }

    //}

    public void PreviewWorld(string layerName, int max = 1)
    {
        int[,] map = world.mapLayers[Array.IndexOf(world.layerNames, "layerName")];
        worldHolder = new GameObject("World").transform;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float num = (float)map[x, y] / (float)max;
                Color col = new Color(num, num, num);
                SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
                rend.color = col;

                GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

                instance.transform.SetParent(worldHolder);
            }
        }

    }
    /// <summary>
    /// Preview Map with the string name
    /// </summary>
    /// <param name="name"></param>
    public void PreviewWorld(String name)
    {
        int[,] map = new int[width, height];
        map = mapLayers[Array.IndexOf(layerNames, name)];

        int max = 1;
        worldHolder = new GameObject("World").transform;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float num = (float)map[x, y] / (float)max;
                Color col = new Color(num, num, num);
                SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
                rend.color = col;

                GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

                instance.transform.SetParent(worldHolder);
            }
        }

    }

    public void DestroyWorld()
    {
        if (worldHolder != null)
            Destroy(worldHolder.gameObject);
    }

    // Use this for initialization

	void Update () {
	
	}

}
