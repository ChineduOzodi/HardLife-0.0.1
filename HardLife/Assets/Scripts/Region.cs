using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Region {

    public List<Coord> tiles;
    public string name;
    public int tileType = 1;

    public Region(List<Coord> theTiles, string regionName, int typeOfTile = 1)
    {
        tiles = theTiles;
        name = regionName;
        tileType = typeOfTile;
    }


}
