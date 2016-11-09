using UnityEngine;
using System.Collections;

public class SelectEquipmentInfo_Action : MonoBehaviour
{

    public int ID;
    public UISprite ItemIcon;
    public UILabel Label_Name;
    public UILabel Label_DescriptionText;
    public ITEMTYPE ItemType;

    public void Set_EquipmentInfo(int id, string icon, string name, ITEMTYPE type, string description)
    {
        ItemIcon.spriteName = icon;
        Label_Name.text = name;
        ID = id;

        ItemType = type;

        Label_DescriptionText.text = description;

    }
}