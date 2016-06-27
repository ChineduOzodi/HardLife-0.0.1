using UnityEngine;

public class Items : GObject
{
    
    
    internal int stackLimit = 1;
    internal int amount = 1;

    public Items()
    {

    }

    public Items(string _type, int x, int y)
    {
        type = _type;

        localMapPositionX = x;
        localMapPositionY = y;
    }
}