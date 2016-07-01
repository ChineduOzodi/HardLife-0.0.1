using UnityEngine;
using System.Collections;
using CodeControl;

public class LineGraphTest : MonoBehaviour {

	// Use this for initialization
	void Awake () {

        LineGraphModel model = new LineGraphModel();
        model.title = "Hello World";
        model.dataNames = new string[] { "Testing", "Hi" };

        //Instantiate Controller
        Controller.Instantiate<LineGrahController>(model, gameObject.transform);

        //model.NotifyChange();

        //model.Delete();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
