using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {


    //Camera Controlls
    private float zoomSpeed = 15f;
    public float camMoveSpeed = .75f;
    public float maxCamSize = 5;

    internal MyGameManager mGM;

    // Use this for initialization
    void Start () {

        mGM = GameObject.FindGameObjectWithTag("GameController").GetComponent<MyGameManager>();
	
	}
	
	// Update is called once per frame
	void Update () {

        float moveModifier = camMoveSpeed * Camera.main.orthographicSize;
        Camera.main.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;

        if (Camera.main.orthographicSize < 4)
            Camera.main.orthographicSize = 4;
        else if (Camera.main.orthographicSize > maxCamSize)
            Camera.main.orthographicSize = maxCamSize;

        if (!mGM.setup)
        {
            float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime;
            float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime;

            Camera.main.transform.Translate(new Vector3(transX, transY));
        }

    }
}
