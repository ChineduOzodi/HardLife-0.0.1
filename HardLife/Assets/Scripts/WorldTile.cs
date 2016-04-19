using UnityEngine;
using System.Collections;
using System;

public class WorldTile : MonoBehaviour {

    public int x;
    public int y;
    public string worldName;
    public string region;
    public string seed;
    public int[] coord;

    private GameManager gameManager;

    public WorldTile()
    {

    }

    public void SetupTile(GameManager gameManager, int x, int y, string seed)
    {
        this.gameManager = gameManager;
        this.x = x;
        this.y = y;
        this.seed = seed + x + y;
        coord = new int[] { x, y };
        worldName = gameManager.worldGen.world.worldName;
    }

    void OnMouseEnter()
    {
        gameManager.SendMessage("SendInfo", coord);
        //print("Found Mouse");
        //print(coord);
    }
}
