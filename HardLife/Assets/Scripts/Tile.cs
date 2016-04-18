using UnityEngine;
using System.Collections;

public class Tile {

    public int x;
    public int y;
    public string region;
    public int id;

    private string seed;

    public Tile(int x, int y, string seed, string region)
    {
        this.x = x;
        this.y = y;
        this.seed = seed;
        this.region = region;
    }

    public Tile(int x, int y, string seed)
    {
        this.x = x;
        this.y = y;
        this.seed = seed;
    }

    public Tile()
    {
        x = 0;
        y = 0;
        seed = null;
    }


}
