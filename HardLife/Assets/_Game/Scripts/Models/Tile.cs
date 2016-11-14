using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Tile:BaseObjectModel {

    public int id;

    public Tile(Vector3 _worldPosition, int x, int y, int _id, string _type = null) : base(_type, _worldPosition, x, y)
    {
        id = _id;
    }
}
