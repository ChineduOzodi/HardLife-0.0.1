﻿using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

[Serializable]
public class WorldModel: Model{
    /// <summary>
    /// The World Class that can generate and create a game world based on inputed parameters
    /// </summary>
    /// 
    public string name;
    internal string seed = null;

    internal int saveNum = 1;

    //time info
    public Date date = new Date(0);    

	internal Vector2 worldSize;
    internal Vector2 localSize;

    internal int worldSizeX, worldSizeY, localSizeX, localSizeY;

    //[Range(0,100)]
    //public int waterFillPercent = 75; //THis is in case you wnat to use the elvation Map to determine land and water instead of baseMap

    public ModelRef<LocalMapModel>[,] localMaps;
	public ModelRef<LocalMapModel> currentLocalMap;

    internal Vector3 worldBottomLeft;
    internal Vector3 localMapWorldBottomLeft;
    

}
