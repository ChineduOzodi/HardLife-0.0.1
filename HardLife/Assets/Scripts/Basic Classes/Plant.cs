using UnityEngine;

public class Plant: Alive
{
    internal float growthRate = 1;
    internal Date growthLevel = new Date(0);
    internal Date matureLevel;
    internal Date maxAge;
    internal float minTemp;

    public Plant(string _type, Vector3 _worldPosition, int x, int y)
        :base(_type, _worldPosition, x, y)
    {

    }
}