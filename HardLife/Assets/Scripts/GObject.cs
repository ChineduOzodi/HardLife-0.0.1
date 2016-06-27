using UnityEngine;
using System.Collections;

public class GObject {

    internal Vector3 worldPostition;
    internal int localMapPositionX, localMapPositionY;

    internal string type;
    internal string classType = "GObject";
    internal bool updateTexture = false;

    internal int renderOrder = 1;
    

    internal float walkSpeedMod = 1;
    internal float floatSpeedMod = 0;
    public GObject(string _type, Vector3 _worldPosition, int x, int y)
    {
        type = _type;

        worldPostition = _worldPosition;
        localMapPositionX = x;
        localMapPositionY = y;

    }
    public virtual string GetInfo()
    {
        return "Class: " + classType;
    }
}
