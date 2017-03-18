using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CodeControl;
using System;

public class InfoPanel : MonoBehaviour {

    public Text objectNameText;
    public Text objectInfoText;

    bool infoActive = false;

    BaseObjectModel model;

    void Update()
    {
        if (infoActive)
            objectInfoText.text = CreateObjectModel.GetInfo(model);
    }

    public void SetObject(BaseObjectModel _model)
    {
        model = _model;
        infoActive = true;
        objectNameText.text = model.name.ToUpper();
        objectInfoText.text = CreateObjectModel.GetInfo(model);
    }
}
