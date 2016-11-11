using UnityEngine;
using System.Collections.Generic;
using JsonFx.Json;

public class ItemManager : MonoBehaviour
{

    private static volatile ItemManager uniqueInstance;
    private static object _lock = new System.Object();

    Dictionary<int, ITEM> ItemInfos = new Dictionary<int, ITEM>();

    public GameObject ItemView = null;
    public GameObject ItemInfo_Prefab = null;

    private ItemManager() { }

    public static ItemManager Instance
    {
        get
        {
            if (uniqueInstance == null)
            {
                lock (_lock)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new ItemManager();
                    }
                }
            }
            return uniqueInstance;
        }
    }
    void Awake()
    {
        uniqueInstance = this;

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "GetItemInfo");

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyItemInfo));
    }

    public void ReplyItemInfo(string json)
    {
        // JsonReader.Deserialize() : 원하는 자료형의 json을 만들 수 있다
        Dictionary<string, object> dataDic = (Dictionary<string, object>)JsonReader.Deserialize(json, typeof(Dictionary<string, object>));

        foreach (KeyValuePair<string, object> info in dataDic)
        {
            ITEM data = JsonReader.Deserialize<ITEM>(JsonWriter.Serialize(info.Value));

            ItemInfos.Add(data.id, data);
            ReadyViewItemInfo(ItemInfos[data.id]);
        }
    }
    private void ReadyViewItemInfo(ITEM data)
    {
        GameObject Info = Instantiate(ItemInfo_Prefab, ItemView.transform) as GameObject;
        Info.name = data.Name;
        Info.transform.localScale = Vector3.one;

        string[] Limit_Types = new string[3] { data.Limit_Type1, data.Limit_Type2, data.Limit_Type3 };
        Info.GetComponent<ItemInfo_Action>().Set_ItemInfo(data.id, data.Icon_Name, data.Name, data.Type, data.Description,
                                                                                                Limit_Types, data.Price);

        ItemView.transform.parent.parent.GetComponent<StoreManager>().Set_BuyButton(Info.transform.FindChild("Button_Buy").GetComponent<UIButton>());
    }
    public ITEM Get_ItemInfo(int id)
    {
        return ItemInfos[id];
    }

}

public enum ITEMTYPE
{
    EQUIPMENT = 1,
    ITEM = 2,
};
public enum ITEMUSE
{
    WEAPON = 1,
    ACCESSORY = 2,
    MATERIAL = 11,
    QUEST = 12,
    ITEM = 13,
}
public enum ITEMEFFECT_TYPE
{
    NULL = -1,
    ATTACK = 0,
    DEFENSE = 1,
    SPEED = 2,
};

public class ITEM
{
    public int id;
    public ITEMTYPE Type;
    public ITEMUSE Use;
    public int Level;
    public int Grade;
    public int Idx;
    public string Icon_Name;
    public string Name;
    public string Description;
    public string Limit_Type1;
    public string Limit_Type2;
    public string Limit_Type3;
    public ITEMEFFECT_TYPE Effect_Type;
    public string Effects;
    public int Price;
}

