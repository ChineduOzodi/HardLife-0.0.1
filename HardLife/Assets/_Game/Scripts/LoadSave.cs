using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class LoadSave : MonoBehaviour {

    internal MyGameManager gameManager;
    public GameObject loadPanel;
    public Text loadingText;
    public Slider slider;
    float pro = 0.0f;

    // Use this for initialization
    void Start () {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MyGameManager>();

    }
	
	public void OnLoadButtonClicked()
    {
        //Model.DeleteAll();
        gameManager.setup = true;
        Model.Load("Saves", OnLoadStart, OnLoadProgress, OnLoadDone, OnLoadError);
    }

    private void OnLoadStart()
    {
        loadPanel.SetActive(true);
    }

    private void OnLoadProgress(float progress)
    {
        if (progress > pro)
        {
            pro += .1f;
            slider.value = progress;
            loadingText.text = "Loading..."+(pro * 100) + "%";
        }
        
    }

    private void OnLoadDone()
    {
        print("Load Done");
        Model[] models = Model.GetAll().ToArray();
        gameManager.world = Model.First<WorldModel>();

        
        UnityEngine.SceneManagement.SceneManager.LoadScene("local_map");
    }

    private void OnLoadError(string error)
    {
        print(error);
    }
}
