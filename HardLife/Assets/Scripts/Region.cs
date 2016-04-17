using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Region {

    public List<Tile> tiles;
    public string name;
    int tileType = 1;

    public Region(List<Tile> theTiles, string regionName, int typeOfTile = 1)
    {
        tiles = theTiles;
        name = regionName;
        tileType = typeOfTile;
    }


}
