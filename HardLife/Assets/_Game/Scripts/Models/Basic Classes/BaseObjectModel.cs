using UnityEngine;
using System.Collections;
using CodeControl;

public class BaseObjectModel : Model {

    internal string name;
    internal ModelRef<LocalMapModel> localMap;
    internal Vector3 worldPostition;
    internal int localMapPositionX, localMapPositionY;


    internal ObjectType type;
    internal bool updateTexture;
    internal int renderOrder = 1;
    
    internal float walkSpeedMod = 1;
    internal float floatSpeedMod = 0;
    
    
}
