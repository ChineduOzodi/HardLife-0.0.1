using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{
	#region "Declarations"

	//GameManager Set-up
	public static GameManager instance = null;
	string savePath;
	public int numAutoSave = 5;
	public bool setup = false;

	private float zoomSpeed = 15f;
	public float camMoveSpeed = .75f;
	public float maxCamSize = 5;

	public WorldGen worldGen;
	public World world;

	//World Settings
	public Vector2 gridWorldSize;
	internal float nodeRadius;
	internal LayerMask unwalkableMask;

	//Local Settings
	public Vector2 gridLocalSize;

    //Main Menu Setup
    public Button createWorld;

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
        setup = false;
    }

    void OnLevelWasLoaded(int levelInt)
    {
        if (levelInt == 0) //Main Menu
        {
            RunMainMenuSetup();

        }
//        else if (levelInt == 1) //World Creation
//        {
//            
//            RunWorldCreationSetup();
//        }
        else if (levelInt == 1) //Local Map Play
        {
            
            RunLocalMapSetup();
        }
    }

    private void RunLocalMapSetup()
    {
        throw new NotImplementedException();
    }

    private void RunMainMenuSetup()
    {
        //worldGen = GetComponent<WorldGen>();
        //createWorld.onClick.AddListener(() => { CreateWorld(); });
    }

    #endregion

    public void CreateWorld()
    {
		setup = true;
		SceneManager.LoadScene ("world_creation");
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
		SceneManager.LoadScene ("local_map");
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
        }

    }

    public void ToggleWorldMap()
    {
        if (!worldGen.createWorldMenu.isActiveAndEnabled)
        {
            worldGen.createWorldMenu.gameObject.SetActive(true);
            worldGen.localMapGen.layers["BaseMap"].gameObject.SetActive(false);
            foreach (Transform layers in worldGen.layers.Values)
            {
                layers.gameObject.SetActive(true);
            }

            Camera.main.orthographicSize = gridWorldSize.y / 2f;
            Camera.main.transform.position = new Vector3(world.width / 2, world.height / 2, -10f);
        }
    }
}
