using UnityEngine;

public class Items
{
    internal Vector3 worldPostition;
    internal int localMapPositionX, localMapPositionY;

    internal string type;
    internal string classType;
    internal int stackOrder = 1;

    bool stackable = false;
    internal int stackLimit = 1;
    internal int amount = 1;

    internal float age = 0;

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