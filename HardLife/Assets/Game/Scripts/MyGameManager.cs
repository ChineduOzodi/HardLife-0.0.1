using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MyGameManager : MonoBehaviour
{
	#region "Declarations"

	//GameManager Set-up
	public static MyGameManager instance = null;
    internal SpriteManager spriteManager;
    string savePath;
	public int numAutoSave = 5;
	public bool setup = false;

    internal float gameSpeed = 1;
    internal float inverseGameSpeed = 1;

    private float zoomSpeed = 15f;
	public float camMoveSpeed = .75f;
	public float maxCamSize = 5;

	internal WorldGen worldGen;
	internal World world;
    internal LocalMapGen localMapGen;

    string[] theCors = new string[2] { "HourlyUpdate", "DailyUpdate" };    

    //World Settings
    public Vector2 worldSize; //Sets the grid size for detecting objects in the world. 
                                   //TODO: make sure it isn't confuse with other worldSize
	internal float nodeRadius = .5f;
	internal LayerMask unwalkableMask;

	//Local Settings
	public Vector2 localSize;
    


    #endregion
    #region "MonoDev Functions"
    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        savePath = Application.persistentDataPath + "currentWorld.world";
        spriteManager = new SpriteManager();
        setup = false;
    }

    void OnLevelWasLoaded(int levelInt)
    {
        if (levelInt == 0) //Main Menu
        {
            //RunMainMenuSetup();

        }
        else if (levelInt == 1) //World Creation
        {
            worldGen = GameObject.FindGameObjectWithTag("WorldGen").GetComponent<WorldGen>();
        }
        else if (levelInt == 2) //Local Map Play
        {

            localMapGen = GameObject.FindGameObjectWithTag("LocalGen").GetComponent<LocalMapGen>();
            StartCoroutine("StartCoroutines",theCors);
        }
    }

    private void RunLocalMapSetup()
    {
        throw new NotImplementedException();
    }

    #endregion

    public void CreateWorld()
    {
		setup = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene ("world_creation");
    }

    public void Save()
    {
        try
        {
            //savePath = Application.persistentDataPath + "/World/" + worldGen.world.worldName + "_Auto Save.sav";//"worldGen.world.saveNum"
        }
        catch (DirectoryNotFoundException)
        {
            // Directory.CreateDirectory(Application.persistentDataPath + "/World/");
            //savePath = Application.persistentDataPath + "/World/" + worldGen.world.worldName + "_Auto Save.sav";//"worldGen.world.saveNum"
        }
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(savePath);

        bf.Serialize(file, world);
        file.Close();
        world.saveNum++;
        if (world.saveNum > numAutoSave)
            world.saveNum = 1;
    }
    public void Load()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            world = (World)bf.Deserialize(file);
            file.Close();

            worldGen.loadWorld();

        }
    }

	public void LoadLocal (){
		Load ();
        UnityEngine.SceneManagement.SceneManager.LoadScene ("local_map");
	}

    // Update is called once per frame
    void Update()
    {

        float moveModifier = camMoveSpeed * Camera.main.orthographicSize;
        Camera.main.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;

        if (Camera.main.orthographicSize < 4)
            Camera.main.orthographicSize = 4;
        else if (Camera.main.orthographicSize > maxCamSize)
            Camera.main.orthographicSize = maxCamSize;
        if (!setup)
        {
            float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime;
            float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime;

            Camera.main.transform.Translate(new Vector3(transX, transY));

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "world_creation")
            {
                
                if (Input.GetMouseButtonDown(0)) //Mouse Left Click
                {
                    worldGen.LeftMouseDown();
                }
            }
        }

    }


    internal void GameSpeedChange(float v)
    {
        gameSpeed = gameSpeed * v;
        inverseGameSpeed = 1 / gameSpeed;
        StopAllCoroutines();
        StartCoroutine("StartCoroutines",theCors);
    }

    IEnumerator StartCoroutines(string[] coroutines)
    {
        foreach(string cor in coroutines)
        {
            StartCoroutine(cor);
            yield return null;
        }
    }
    IEnumerator DailyUpdate()
    {
        for (;;)
        {
            if (!setup)
            {
                localMapGen.UpdateAge();
            }
            yield return new WaitForSeconds(Date.Day * inverseGameSpeed); //TODO: Check to make sure ti is updateing correctly
        }
    }
    IEnumerator HourlyUpdate()
    {
        for (;;)
        {
            if (!setup)
            {

                //StartCoroutine("StartCoroutines",theCors);
                localMapGen.UpdateTemperature(world.localMap);
                localMapGen.UpdatePlantGrowth();
                if (localMapGen.statisticsText.IsActive())
                {
                    localMapGen.statisticsText.text = world.localMap.CompileStats();
                }

            }
            yield return new WaitForSeconds(Date.Hour * inverseGameSpeed); //TODO: Check to make sure ti is updateing correctly
        }
    }
    IEnumerator MinuteUpdate()
    {
        for (;;)
        {
            if (!setup)
            {

                //StartCoroutine("StartCoroutines",theCors);
            }
            yield return new WaitForSeconds(inverseGameSpeed); //TODO: Check to make sure ti is updateing correctly
        }
    }
    public void ToggleWorldMap()
    {
        //if (!worldGen.createWorldMenu.isActiveAndEnabled)
        //{
        //    worldGen.createWorldMenu.gameObject.SetActive(true);
        //    worldGen.localMapGen.layers["BaseMap"].gameObject.SetActive(false);
        //    foreach (Transform layers in worldGen.layers.Values)
        //    {
        //        layers.gameObject.SetActive(true);
        //    }

        //    Camera.main.orthographicSize = gridWorldSize.y / 2f;
        //    Camera.main.transform.position = new Vector3(world.width / 2, world.height / 2, -10f);
        //}
    }
    public Coord WorldCoordFromWorldPosition(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + worldSize.x / 2) / worldSize.x;
        float percentY = (worldPosition.y + worldSize.y / 2) / worldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((world.worldSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((world.worldSizeY - 1) * percentY);
        Coord coord = new Coord(x, y);
        return coord;
    }
    public Coord LocalCoordFromWorldPosition(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + localSize.x / 2) / localSize.x;
        float percentY = (worldPosition.y + localSize.y / 2) / localSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((world.localSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((world.localSizeY - 1) * percentY);
        Coord coord = new Coord(x, y);
        return coord;
    }
}
