using UnityEngine;
using System.Collections;

public class CharaterInfo_Action : MonoBehaviour {

    public UISprite CharaterIcon;
    public UILabel Label_Level;
    public GameObject Stars;
    public UISprite Type;

    public void Set_CharaterInfo(string icon_name, int level, int star_num, string type)
    {
        CharaterIcon.spriteName = icon_name;
        Label_Level.text = "Lv. " + level.ToString();
        Type.spriteName = type;

        for( int i = 0; i < star_num; i++)
        {
            Stars.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

}
