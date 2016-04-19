using UnityEngine;
using System.Collections;

public class LocalMap {

    public int x;
    public int y;
    public string seed;

    //baselayer ID's
    int waterID = 0;
    int sandID = 1;
    int grassID = 2;
    int jungleID = 3;
    int desertID = 4;

    public float[,] elevatiomMap;
    public Tile[,] baseMap;
    public Road[,] roadMap;
    public Items[,] itemMap;
    public Roof[,] roofMap;
    public Items[,] skyMap;

    public float[] baseMapNC = { .33f, .66f };
    public float[] itemMapNC = { .33f, .66f };

    public LocalMap(string seed, int biomeType, int elevation, int[,] adjacentBaseTiles, int[,] adjacentElevTiles)
    {
        
    }

}
