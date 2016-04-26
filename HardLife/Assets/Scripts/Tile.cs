using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Tile {

    public int x;
    public int y;
    public int width;
    public int height;
    public string region;
    public int id;

    public Tile(int x, int y, int id)
    {
        this.x = x;
        this.y = y;
        this.id = id;
    }

    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Tile()
    {
        x = 0;
        y = 0;
    }


}
