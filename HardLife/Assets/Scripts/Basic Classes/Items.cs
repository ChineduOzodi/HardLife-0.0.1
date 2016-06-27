using UnityEngine;

public class Items : GObject
{
    
    
    internal int stackLimit = 1;
    internal int amount = 1;
    bool stackable = false;

    public Items(string _type, Vector3 _worldPosition, int x, int y) : base(_type, _worldPosition, x, y)
    {
    }
}