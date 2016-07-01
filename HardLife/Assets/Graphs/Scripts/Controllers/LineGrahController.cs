using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class LineGrahController : Controller<LineGraphModel> {

    public Text title;
    public ToggleGroup dataNameGroup;
    public ToggleGroup dataPreferenceGroup;

    public GameObject toggeButton;

    protected override void OnInitialize()
    {
        title = GameObject.FindGameObjectWithTag("Water").GetComponent<Text>();
        //ToggleGroup[] toggleGroups = gameObject.GetComponentsInChildren<ToggleGroup>();
        ToggleGroup[] toggleGroups = GameObject.FindObjectsOfType<ToggleGroup>();

        dataNameGroup = toggleGroups[0];
        dataPreferenceGroup = toggleGroups[1];

        //MessageListeners
        Message.AddListener<ToggleMessage>(OnToggleMessage);

        //Create Toggle Buttons for the Toggle Group Data Names
        foreach (string dataName in model.dataNames)
        {
            //Init Models
            ToggleButtonModel nameToggle = new ToggleButtonModel();

            nameToggle.label = dataName;
            nameToggle.toggleGroupName = "Data Names";

            //InitController

            ToggleButtonController cont = Controller.Instantiate<ToggleButtonController>(toggeButton, nameToggle, dataNameGroup.transform);

            //cont.transform.localScale = Vector3.one;
        }

        UpdateGraph();
    }

    private void OnToggleMessage(ToggleMessage obj)
    {
        throw new NotImplementedException();
    }

    public void UpdateGraph()
    {
        title.text = model.title;
    }

    protected override void OnModelChanged()
    {
        base.OnModelChanged();
        UpdateGraph();

    }
}
