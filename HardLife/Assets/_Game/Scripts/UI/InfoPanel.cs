using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CodeControl;
using System;

public class InfoPanel : Controller<BaseObjectModel> {

    public Text objectNameText;
    public Text objectInfoText;

    protected override void OnInitialize()
    {
        objectNameText.text = model.name.ToUpper();
        objectInfoText.text = CreateObjectModel.GetInfo(model);
    }

    void Update()
    {
        objectInfoText.text = CreateObjectModel.GetInfo(model);
    }
}
