using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LocalMapGen : MonoBehaviour { // TODO fix local map generation

    World world;
    public LocalMap local;
    public GameManager gameManager;

    GameObject baseMapEmpty;
    GameObject itemMapEmpty;

    SpriteRenderer[,] baseMap;
    SpriteRenderer[,] itemMap;

    // Use this for initialization
    void Awake () {

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        world = gameManager.world;

        CreateLocalMap();

    }
	
	// Update is called once per frame
	void Update () {

	}

    public void CreateLocalMap()
    {

        int x = world.localMap.worldPositionX;
        int y = world.localMap.worldPositionY;

        string seed = world.localMap.seed;

        //Camera Setup
        Camera.main.orthographicSize = gameManager.worldSize.x / 2f;
        gameManager.maxCamSize = gameManager.worldSize.y / 2f;
        Camera.main.transform.position = new Vector3(0, 0, -10f);

        //Generate LocalMap if not generated
        if (world.localMap.lastUpdated < 1)
        {
            world.localMap.GenerateLocalMap(world);
            world.localMap.lastUpdated = 1;
        }

        BuildBaseMap();

    }

    public void PreviewMap(LocalMap local)
    {
        throw new NotImplementedException();
        //DestroyLocalMap();

        //FresNoise noise = new FresNoise();

//        worldGen.mainCam.orthographicSize = local.height / 2f;
//        worldGen.gameManager.maxCamSize = local.height / 2f;
//        worldGen.mainCam.transform.position = new Vector3(local.width / 2, local.height / 2, -10f);
//
//        worldGen.createWorldMenu.gameObject.SetActive(false);
//        worldGen.gameManager.localMapCanvas.gameObject.SetActive(true);

        //layers["BaseMap"] = new GameObject("LocalBaseMap").transform;

        ////int[,] map = [width, height];
        //Tile[,] map = local.baseMap;
        //for (int x = 0; x < width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        //map[x, y] = noise.ScaleFloatToInt(local.elevationMap[x,y],local.baseMapNC);
        //        float num = (float)map[x, y].id/5f; // (float)map[x, y] / (float)max;
        //        Color col = new Color(num, num, num);
        //        //SpriteRenderer rend = worldGen.whiteBlock.GetComponent<SpriteRenderer>();
        //        //rend.color = col;

        //        //GameObject instance = Instantiate(worldGen.whiteBlock, new Vector3(x, y), Quaternion.identity) as GameObject;

        //        //instance.transform.SetParent(layers["BaseMap"]);
        //    }
        //}
     }
    public void BuildBaseMap()
    {
        //throw new NotImplementedException();
        //DestroyLocalMap();

        baseMapEmpty = new GameObject("BaseMap");
        baseMap = new SpriteRenderer[world.localSizeX, world.localSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * world.localSize.x / 2 - Vector3.up * world.localSize.y / 2;

        for (int x = 0; x < world.localSizeX; x++)
        {
            for (int y = 0; y < world.localSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * gameManager.world.nodeDiameter + gameManager.world.nodeRadius) + Vector3.up * (y * gameManager.world.nodeDiameter + gameManager.world.nodeRadius);
                GameObject tile = new GameObject("BiomeTile");
                tile.transform.position = worldPoint;
                SpriteRenderer instance = tile.AddComponent<SpriteRenderer>();
                instance.sprite = gameManager.spriteManager.GetSprite(gameManager.world.localMap.baseMap[x,y].type);
                instance.transform.SetParent(baseMapEmpty.transform);
                baseMap[x, y] = instance;
            }
        }
    }



    private void DestroyLocalMap()
    {
        throw new NotImplementedException();
        //if (layers != null)
        //{
        //    foreach (Transform layer in layers.Values)
        //    {
        //        Destroy(layer.gameObject);
        //    }
        //}
    }
}
