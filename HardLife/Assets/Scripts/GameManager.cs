using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    string savePath;
    public WorldGen worldGen;
    public Canvas mainMenu;
    public Button createWorld;
    public float camMoveSpeed = .75f;
    public int numAutoSave = 5;
    public bool setup = false;
    public Canvas localMapCanvas;

    private float zoomSpeed = 15f;

    private Camera mainCam;
    public float maxCamSize = 5;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        savePath = Application.persistentDataPath + "currentWorld.world";

        worldGen = GetComponent<WorldGen>();
        createWorld.onClick.AddListener(() => { CreateWorld(); });
        mainMenu.gameObject.SetActive(true);
        localMapCanvas.gameObject.SetActive(false);
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        setup = false;
    }

    private void CreateWorld()
    {
        mainMenu.gameObject.SetActive(false);
        worldGen.OpenMenu();
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

        World world = worldGen.world;
        bf.Serialize(file, world);
        file.Close();
        worldGen.world.saveNum++;
        if (worldGen.world.saveNum > numAutoSave)
            worldGen.world.saveNum = 1;
    }
    public void Load()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            worldGen.world = (World)bf.Deserialize(file);
            file.Close();

            worldGen.loadWorld();

        }
    }

    // Update is called once per frame
    void Update()
    {
        float moveModifier = camMoveSpeed * mainCam.orthographicSize;
        mainCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;

        if (mainCam.orthographicSize < 4)
            mainCam.orthographicSize = 4;
        else if (mainCam.orthographicSize > maxCamSize)
            mainCam.orthographicSize = maxCamSize;
        if (!setup)
        {
            float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime;
            float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime;

            mainCam.transform.Translate(new Vector3(transX, transY));
        }

    }

    public void ToggleWorldMap()
    {
        if (!worldGen.createWorldMenu.isActiveAndEnabled)
        {
            worldGen.createWorldMenu.gameObject.SetActive(true);
            localMapCanvas.gameObject.SetActive(false);
            worldGen.localMapGen.layers["BaseMap"].gameObject.SetActive(false);
            foreach (Transform layers in worldGen.layers.Values)
            {
                layers.gameObject.SetActive(true);
            }

            mainCam.orthographicSize = worldGen.world.height / 2f;
            mainCam.transform.position = new Vector3(worldGen.world.width / 2, worldGen.world.height / 2, -10f);
        }
    }
}
