using UnityEngine;
using System.Collections;

public class InvenItem_Action : MonoBehaviour {

    public int ID;
    public ITEMTYPE Type;
    public UISprite Icon;
    public GameObject Stars;
    public UILabel Name;
    public UILabel Count;

    public void Set_ItemInfo(int id, string icon_name, int idx, int grade, string name, ITEMTYPE type, int count)
    {
        ID = id;
        Type = type;
        Icon.spriteName = icon_name;
        Name.text = name;
        Count.text = count.ToString();

        if (Type == ITEMTYPE.EQUIPMENT)
        {
            for (int i = 0; i < grade; i++)
            {
                Stars.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if(Type == ITEMTYPE.ITEM)
        {
            for (int i = 0; i < idx; i++)
            {
                Stars.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

}
