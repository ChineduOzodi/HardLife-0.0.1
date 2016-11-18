using UnityEngine;
using System.Collections;
using CodeControl;

public class BaseObjectModel : Model {

    internal ModelRef<LocalMapModel> localMap;
    internal Vector3 worldPostition;
    internal int localMapPositionX, localMapPositionY;

    internal string type;
    internal string classType = "GObject";
    internal bool updateTexture = false;

    internal int renderOrder = 1;
    
    internal float walkSpeedMod = 1;
    internal float floatSpeedMod = 0;
    
    
}
