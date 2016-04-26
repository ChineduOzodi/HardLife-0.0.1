using UnityEngine;
using System.Collections;
using System;

public class LocalTile : MonoBehaviour {

    GameManager gameManager;
    int x;
    int y;
    string type;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    internal void SetupTile(GameManager gameManager, int x, int y, string type)
    {
        this.gameManager = gameManager;
        this.x = x;
        this.y = y;
        this.type = type;
    }
}
