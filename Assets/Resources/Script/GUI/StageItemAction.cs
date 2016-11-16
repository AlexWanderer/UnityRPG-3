using UnityEngine;
using System.Collections;

public enum STATEITEM_TYPE
{
    GOLD = 0,
    ITEM = 1,
    CHARATER = 2
};

public class StageItemAction : MonoBehaviour {

    public UIAtlas ItemAtlas;
    public UIAtlas CharaterAtlas;

    public UISprite Icon;
    public UILabel Num;
    public UILabel Name;


    public void Set_UI(STATEITEM_TYPE type ,string icon_name, int num, string name)
    {
        switch (type)
        {
            case STATEITEM_TYPE.GOLD:
                {
                    Icon.atlas = ItemAtlas;
                    Icon.spriteName = "money_purse";
                    break;
                }
            case STATEITEM_TYPE.ITEM:
                {
                    Icon.atlas = ItemAtlas;
                    Icon.spriteName = icon_name;
                    break;
                }
            case STATEITEM_TYPE.CHARATER:
                {
                    Icon.atlas = CharaterAtlas;
                    Icon.spriteName = icon_name;
                    break;
                }
        }
        Num.text = num.ToString();
        Name.text = name;
    }
}
