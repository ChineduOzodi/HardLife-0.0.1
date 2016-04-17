using UnityEngine;
using System.Collections;

public class TestWG : MonoBehaviour {

    public float timeDelay;
    public World worldGen;
    public int mapSmooth = 1;
    

    private float timeSave = 0;
    private int smoothIter = 0;

	void Awake () {

        timeSave = timeDelay;
        worldGen = new World();
        worldGen.GenerateMap();
        //worldGen.PreviewWorld();
    }
	
	// Update is called once per frame
	void Update () {

        //Deletes and creates new world ever timeDelay seconds
        if (Time.time > timeSave)
        {
            //worldGen.DestroyWorld();
            //if (smoothIter <= mapSmooth)
            //{
            //    worldGen.SmoothMap(tileCount);
            //    worldGen.CreateWorld();
            //    smoothIter++;
            //}
            //else
            //{
            //    worldGen.GenerateMap(0);
            //    worldGen.CreateWorld();
            //    smoothIter = 0;
            //}
            worldGen.GenerateMap();
            //worldGen.PreviewWorld();
            timeSave += timeDelay;
        }
	
	}
}
