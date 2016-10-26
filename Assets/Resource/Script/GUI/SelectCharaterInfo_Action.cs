using UnityEngine;
using System.Collections;

public class SelectCharaterInfo_Action : MonoBehaviour {

    public UILabel Label_Level;
    public UISprite CharaterIcon;
    public UISprite TypeIcon;

    public void SelectCharaterInfo(int level, string name, string type)
    {
        Label_Level.text = "Lv"+level.ToString();
        CharaterIcon.spriteName = name;
        TypeIcon.spriteName = type;
    }
}
