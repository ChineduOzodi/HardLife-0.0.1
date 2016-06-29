using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGen {

    internal Queue<GameObject> objectQueue = new Queue<GameObject>();

    public GameObject CreateObject(string name, Vector3 position)
    {
        if (objectQueue.Count == 0)
        {
            GameObject obj =  new GameObject(name);
            obj.transform.position = position;
            obj.AddComponent<SpriteRenderer>();
            return obj;
        }
        else
        {
            return objectQueue.Dequeue();
        }
    }
}
