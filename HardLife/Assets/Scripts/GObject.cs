using UnityEngine;
using System.Collections;

public class GObject {

    internal Vector3 worldPostition;
    internal int localMapPositionX, localMapPositionY;

    internal string type;
    internal string classType;

    internal int stackOrder = 1;
    bool stackable = false;

    internal float walkSpeedMod;
    internal float driveSpeedMod;
    internal float floatSpeedMod;
}
