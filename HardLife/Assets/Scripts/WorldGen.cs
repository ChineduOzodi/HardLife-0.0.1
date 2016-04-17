using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldGen : World {


    public GameObject whiteBlock;
    public GameObject[] water;
    public GameObject[] dirt;

    private Transform worldHolder = null;
    private Camera mainCam;

    void Awake()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        DestroyWorld();
        GenerateMap();
        //PreviewWorld("Temperature Map",2);
        //PreviewWorld("Base Map", 1);
        PreviewWorld();

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
        int[,] map = mapLayers[Array.IndexOf(layerNames, layerName)];
        worldHolder = new GameObject("World").transform;

        mainCam.orthographicSize = height / 2f;
        mainCam.transform.position = new Vector3(width / 2, height / 2, -10f);

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
    public void PreviewWorld()
    {

        mainCam.orthographicSize = height / 2f;
        mainCam.transform.position = new Vector3(width / 2, height / 2, -10f);

        for (int i = 0; i < layerNames.Length; i++)
        {
            int[,] map = mapLayers[i];
            int max = 2;
            if (layerNames[i] == "Base Map")
                maxIslandSize = 1;
            
            worldHolder = new GameObject(layerNames[i]).transform;

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
