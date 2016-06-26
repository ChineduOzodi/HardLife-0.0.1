using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Tile:GObject {

    public int id;

    public Tile(int x, int y, int _id)
    {
        localMapPositionX = x;
        localMapPositionY = y;
        id = _id;
    }
}
