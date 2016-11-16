using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

    public Text objectNameText;
    public Text objectInfoText;

    // Use this for initialization
    public void SetTitle(string title)
    {
        objectNameText.text = title;
    }
    public void SetInfoText(string text)
    {
        objectInfoText.text = text;
    }
}
