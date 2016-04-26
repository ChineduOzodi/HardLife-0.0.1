using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class WorldTile : MonoBehaviour {

    public int x;
    public int y;
    public string worldName;
    public string region;
    public string seed;
    public int[] coord;
    bool tileSelected = false;
    int layerMask = 1 << 5;

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
        if (!tileSelected && !EventSystem.current.IsPointerOverGameObject())
        {
            gameManager.SendMessage("SendInfo", coord);
            //print("Found Mouse");
            //print(coord);
            //gameObject.transform.localScale = new Vector3(1.25f, 1.25f);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(.8f, .8f, .8f);
        }
        
    }
    void OnMouseExit()
    {
        if (!tileSelected && !EventSystem.current.IsPointerOverGameObject())
        {
            gameObject.transform.localScale = new Vector3(1, 1);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
        
    }
    void OnMouseDown()
    {
        
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            gameObject.transform.localScale = new Vector3(1, 1);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
            gameManager.worldGen.layers["Biomes"].BroadcastMessage("ToggleTileSelected");
            gameManager.worldGen.SendMessage("ToggleTileSelected");
            //gameObject.GetComponentInParent<Transform>().BroadcastMessage("ToggleTileSelected");
        }


    }
    void ToggleTileSelected()
    {
        if (tileSelected)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            tileSelected = false;
        }
        else
            tileSelected = true;
    }
}
