using UnityEngine;
using System.Collections;

public class InvenItem_Action : MonoBehaviour {

    public int ID;
    public ITEMTYPE Type;
    public UILabel Count;
    public UISprite Icon;
    public GameObject Stars;
    public UILabel Name;

    public void Set_ItemInfo(int id, int count, string icon_name, int idx, int grade, string name, ITEMTYPE type)
    {
        Debug.Log(idx);

        ID = id;
        Type = type;
        Count.text = count.ToString();
        Icon.spriteName = icon_name;
        Name.text = name;

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
