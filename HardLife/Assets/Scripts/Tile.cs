using UnityEngine;
using System.Collections;

public class Tile {

    public int tileX;
    public int tileY;
    public string regionName;

    private string sd;

    public Tile(int x, int y, string seed)
    {
        tileX = x;
        tileY = y;
        sd = seed;
    }
}
