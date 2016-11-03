using UnityEngine;
using System.Collections;

public class ItemInfo_Action : MonoBehaviour {

    int ID;
    public UISprite ItemIcon;
    public UILabel Label_Name;
    public UILabel Label_DescriptionText;
    public ITEMTYPE ItemType;
    public ITEMUSE ItemUse;
    public UISprite[] EquipType = new UISprite[3];
    public UILabel Label_Price;



    public void Set_ItemInfo(int id, string icon, string name, ITEMTYPE type, string description, string[] limit_type, int price)
    {
        ItemIcon.spriteName = icon;
        Label_Name.text = name;
        Label_Price.text = price.ToString();
        ID = id;

        ItemType = type;

        if (ItemType == ITEMTYPE.EQUIPMENT)
        {
            for (int i = 0; i < 3; i++)
            {
                if (limit_type == null) { break; }
                if (limit_type[i] == null) { EquipType[i].gameObject.SetActive(false); continue; }

                EquipType[i].gameObject.SetActive(true);
                EquipType[i].spriteName = limit_type[i];
            }

            Label_DescriptionText.text = description;
        }
        else if (ItemType == ITEMTYPE.ITEM)
        {

        }
    }
}
