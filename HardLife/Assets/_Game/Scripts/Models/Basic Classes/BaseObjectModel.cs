using UnityEngine;
using System.Collections;
using CodeControl;

public class BaseObjectModel {

    public string name;
    public ModelRef<LocalMapModel> localMap;
    public Vector3 worldPostition;
    public int localMapPositionX, localMapPositionY;

    public ObjectType type;
    public bool updateTexture;
    public int renderOrder = 1;
    
    public float walkSpeedMod = 1;
    public float floatSpeedMod = 0;
    
    
}
