using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class InventoryManager : MonoBehaviour {


    public GameObject Items = null;
    public GameObject InvenItem_Prefab = null;

    private static InventoryManager instance = null;

    public static InventoryManager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(InventoryManager)) as InventoryManager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("InventoryManager ");
            instance = obj.AddComponent(typeof(InventoryManager)) as InventoryManager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

    void Start()
    {
        Get_UserItemID();
    }

    void Get_UserItemID()
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "GetUserItemID");

        sendData.Add("user_index", GameManager.Get_Inctance().Index);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyUserItemID));
    }
    public void ReplyUserItemID(string json)
    {
        // JsonReader.Deserialize() : 원하는 자료형의 json을 만들 수 있다
        Dictionary<string, object> dataDic = (Dictionary<string, object>)JsonReader.Deserialize(json, typeof(Dictionary<string, object>));

        if (dataDic == null) { return; }

        foreach (KeyValuePair<string, object> info in dataDic)
        {
            RecvUserItemInfo data = JsonReader.Deserialize<RecvUserItemInfo>(JsonWriter.Serialize(info.Value));

            GameManager.Get_Inctance().Get_Item.Add(data.Item_id, data.Item_count);
            View_AddItem(data.Item_id, data.Item_count);
        }

    }
    public void View_ALLItem()
    {
        for (int i = 0; i < Items.transform.childCount; i++)
        {
            Items.transform.GetChild(i).gameObject.SetActive(true);
        }

        Items.GetComponent<UIGrid>().repositionNow = true;
        Items.GetComponentInParent<UIScrollView>().ResetPosition();
        SpringPanel.Begin(Items.transform.parent.gameObject, new Vector3(2, -67, 0), 8);
    }
    public void View_EquipmentItem()
    {
        for (int i = 0; i < Items.transform.childCount; i++)
        {
            InvenItem_Action data = Items.transform.GetChild(i).GetComponent<InvenItem_Action>();

            if (data.Type != ITEMTYPE.EQUIPMENT)
            {
                data.gameObject.SetActive(false);
            }
            else
            {
                data.gameObject.SetActive(true);
            }
        }

        Items.GetComponent<UIGrid>().repositionNow = true;
        Items.GetComponentInParent<UIScrollView>().ResetPosition();
        SpringPanel.Begin(Items.transform.parent.gameObject, new Vector3(2, -67, 0), 8);
    }
    public void View_ItemItem()
    {
        for (int i = 0; i < Items.transform.childCount; i++)
        {
            InvenItem_Action data = Items.transform.GetChild(i).GetComponent<InvenItem_Action>();

            if (data.Type != ITEMTYPE.ITEM)
            {
                data.gameObject.SetActive(false);
            }
            else
            {
                data.gameObject.SetActive(true);
            }

        }

        Items.GetComponent<UIGrid>().repositionNow = true;
        Items.GetComponentInParent<UIScrollView>().ResetPosition();
        SpringPanel.Begin(Items.transform.parent.gameObject, new Vector3(2, -67, 0), 8);
    }

    public void View_AddItem(int item_id, int count)
    {
        ITEM info = ItemManager.Instance.Get_ItemInfo(item_id);
        GameObject item = Instantiate(InvenItem_Prefab, Items.transform) as GameObject;
        item.transform.localScale = Vector3.one;
        item.name = info.Name;

        item.GetComponent<InvenItem_Action>().Set_ItemInfo(info.id, info.Icon_Name, info.Idx, info.Grade, info.Name, info.Type, count);

        Items.GetComponent<UIGrid>().repositionNow = true;
    }

    public class RecvUserItemInfo
    {
        public int Item_id;
        public int Item_count;
    }

}
