using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class WorldTile {

	internal Vector3 worldPosition;
    internal string region;
    internal string seed;

	internal float aveTemp;
	internal float curTemp;
	internal float aveRain;
	internal string biome;
	internal int elevation; //either 0, 1 , or 2

    internal bool tileSelected = false;

	public WorldTile(Vector3 _worldPosition, string _seed, string _region = "Unknown")
    {
		worldPosition = _worldPosition;
		seed = _seed + worldPosition;
		region = _region;
    }
    
//    void ToggleTileSelected()
//    {
//        if (tileSelected)
//        {
//            tileSelected = false;
//        }
//        else
//            tileSelected = true;
//    }
}
