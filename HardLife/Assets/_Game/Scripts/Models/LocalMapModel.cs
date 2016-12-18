using UnityEngine;
using System.Collections;
using System;
using CodeControl;

//[Serializable]
public class LocalMapModel: Model { //TODO Add comments to class
    //General Variables
    public ModelRef<WorldModel> world;

    public Vector3 worldPosition;
    public Vector2 localSize;
    public int localSizeX, localSizeY, worldMapPositionX,worldMapPositionY;

    public string seed;
    public string region;
    public int baseNum;
    public string biome;
	public string mountainLevel;
    public float elevation;
    public float rain;
    public float aveTemp;

    public float lastUpdated = 0f;

    //Local Map Variables
    public float heightMapScale = 10f;
    public float baseMapScale = 1f;
    public float mountainScale = .2f;
    
    public float[] elevationMap;
    public TileModel[] baseMap;
    public RoadModel[] roadMap;
    public BaseObjectModel[] objectMap;
    public RoofModel[] roofMap;
    public ItemsModel[] skyMap;
    
    public float curTemp;
    public Vector3 worldBottomLeft;

    //Stats
    Series treeCount;
    Series bushCount;
    

}
