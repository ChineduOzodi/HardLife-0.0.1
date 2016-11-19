using System;
using UnityEngine;

public class PlantModel: AliveModel
{
    internal float growthRate = 1;
    internal bool replicate = false;
    internal Vector2 replicateLocation;
    internal float birthPercent;
    internal float birthFactor = .0011f; //This allows the replicate number to have a chance of landing that many times in one year

    internal int maxFruit;
    internal int fruit = 0;
    internal int maxLeaves;
    internal int leaves;

    internal float minTemp;
    internal Date growthLevel = new Date(0);
    internal Date matureLevel;
    internal State state = State.Healthy;
}