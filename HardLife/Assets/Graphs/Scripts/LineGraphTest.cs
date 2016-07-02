using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;

public class LineGraphTest : MonoBehaviour {

    public GameObject lineGraph;
    public GameObject parentObj;
    public LineGraphModel model;

    // Use this for initialization
    void Awake() {

        //Init Model
        model = new LineGraphModel();

        model.dataNames = new string[] { "Bushes", "Trees" };
        model.dataPrefs = new string[] { "Days" };

        model.data = new Dictionary<string, float>[model.dataNames.Length, model.dataPrefs.Length];
        model.data[0, 0] = new Dictionary<string, float>();
        model.data[1, 0] = new Dictionary<string, float>();

        model.selectedDataName = 0;
        model.selectedDataPreference = 0;



        //Instantiate Controller
        Controller.Instantiate<LineGraphController>(lineGraph, model, parentObj.transform);

        StartCoroutine("SecondUpdate");
	
	}
	
    IEnumerator SecondUpdate()
    {
        int count = 0;
        for (;;)
        {
            model.data[0, 0].Add(count++.ToString(), Random.Range(0, 100));
            model.data[1, 0].Add(count.ToString(), count + Random.Range(0, 10));
            if (model.data[1,0].Count > 10)
            {
                model.data[1, 0].Remove((count - 10).ToString());
            }

            model.NotifyChange();

            yield return new WaitForSeconds(1);
        }
    }
	// Update is called once per frame
	void Update () {

	
	}
}
