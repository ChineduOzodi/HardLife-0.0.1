using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class WorldGen : World {


    public GameObject whiteBlock;
    public GameObject[] water;
    public GameObject[] ice;
    public GameObject[] grass;
    public GameObject[] jungle;
    public GameObject[] desert;
    public GameObject[] hill;
    public GameObject[] mountain;


    private GameObject[][] biomeSprites;
    private Dictionary<string, Transform> layers = new Dictionary<string, Transform> { };
    private Camera mainCam;

    void Awake()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        biomeSprites = new GameObject[][] { water, ice, grass, jungle, desert };

        //DestroyWorld();
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
        layers["World"] = new GameObject("World").transform;

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

                instance.transform.SetParent(layers["World"]);
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

        buildBasicLayers();
        //Preview Biome
        buildBiome();
        builMountains();
    }

    private void builMountains()
    {
        layers["Mountains"] = new GameObject("Mountains").transform;
        int[,] map = mapLayers[Array.IndexOf(layerNames, "Mountain Map")];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x,y] == 1)
                {
                    int num = UnityEngine.Random.Range(0, hill.Length);
                    GameObject tile = hill[num].gameObject;
                    GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(layers["Mountains"]);
                }
                else if(map[x,y] == 2)
                {
                    int num = UnityEngine.Random.Range(0, mountain.Length);
                    GameObject tile = mountain[num].gameObject;
                    GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(layers["Mountains"]);
                }
            }
        }
    }

    private void buildBiome()
    {
        layers["Biomes"] = new GameObject("Biomes").transform;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int sprite = tiles[x, y].id;
                int num = UnityEngine.Random.Range(0, biomeSprites[sprite].Length);
                GameObject tile = biomeSprites[sprite][num].gameObject;
                GameObject instance = Instantiate(tile, new Vector3(x, y), Quaternion.identity) as GameObject;

                instance.transform.SetParent(layers["Biomes"]);
            }
        }
        //layers["Biomes"].gameObject.SetActive(false);
    }

    private void buildBasicLayers()
    {
        for (int i = 0; i < layerNames.Length; i++)
        {
            int[,] map = mapLayers[i];
            int max = 2;

            layers[layerNames[i]] = new GameObject(layerNames[i]).transform;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float num = (float)map[x, y] / (float)max;
                    Color col = new Color(num, num, num);
                    SpriteRenderer rend = whiteBlock.GetComponent<SpriteRenderer>();
                    rend.color = col;

                    GameObject instance = Instantiate(whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

                    instance.transform.SetParent(layers[layerNames[i]]);
                }
            }

            layers[layerNames[i]].gameObject.SetActive(false);

        }
    }

    public void DestroyWorld()
    {
        if (layers != null)
        {
            foreach (Transform layer in layers.Values)
            {
                Destroy(layer.gameObject);
            }
            layers = null;
        }

            
    }

    // Use this for initialization

	void Update () {
	
	}

}
