﻿using UnityEngine;
using System.Collections;
using System;
using CodeControl;

//[Serializable]
public class LocalMapModel: Model { //TODO Add comments to class
    //General Variables
    internal ModelRef<WorldModel> world;

    internal Vector3 worldPosition;
    internal Vector2 localSize;
    internal int localSizeX, localSizeY, worldMapPositionX,worldMapPositionY;

    public string seed;
    internal string region;
    internal int baseNum;
    internal string biome;
	internal string mountainLevel;
    internal float elevation;
    internal float rain;
    internal float aveTemp;

    internal float lastUpdated = 0f;

    //Local Map Variables
    public float heightMapScale = 10f;
    public float baseMapScale = 1f;
    public float mountainScale = .2f;
    
    public float[,] elevationMap;
    public ModelRef<TileModel>[,] baseMap;
    public ModelRef<RoadModel>[,] roadMap;
    public ModelRef<BaseObjectModel>[,] objectMap;
    public ModelRef<RoofModel>[,] roofMap;
    public ModelRef<ItemsModel>[,] skyMap;
    
    internal float curTemp;
    internal Vector3 worldBottomLeft;

    //Stats
    Series treeCount;
    Series bushCount;
    

}
