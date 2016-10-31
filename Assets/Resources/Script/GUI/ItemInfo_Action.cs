using UnityEngine;
using System.Collections;

public class ItemInfo_Action : MonoBehaviour {

    public UISprite ItemIcon;
    public UILabel Label_Name;
    public UILabel Label_EffectText;
    public string ItemType;
    public UISprite[] EquipType = new UISprite[3];
    public UILabel Label_Price;



    public void Set_ItemInfo(string icon, string name, string effect_text, string type, string[] limit_type, int price)
    {
        ItemIcon.spriteName = icon;
        Label_Name.text = name;
        Label_EffectText.text = effect_text;
        ItemType = type;
        
        for(int i = 0; i < 3; i ++)
        {
            if (limit_type == null) { break; }
            if (limit_type[i] == null) { EquipType[i].gameObject.SetActive(false); continue; }

            EquipType[i].gameObject.SetActive(true);
            EquipType[i].spriteName = limit_type[i];
        }

        Label_Price.text = price.ToString();

    }
}
