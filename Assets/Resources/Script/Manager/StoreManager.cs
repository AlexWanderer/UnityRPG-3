using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class StoreManager : MonoBehaviour {

    public GameObject Items = null;

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
            ItemInfo_Action data = Items.transform.GetChild(i).GetComponent<ItemInfo_Action>();

            if(data.ItemType != ITEMTYPE.EQUIPMENT)
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
            ItemInfo_Action data = Items.transform.GetChild(i).GetComponent<ItemInfo_Action>();

            if (data.ItemType != ITEMTYPE.ITEM)
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

    public void Set_BuyButton(UIButton button)
    {
        EventDelegate.Parameter param = new EventDelegate.Parameter();

        param.obj = button.gameObject.transform.parent.GetComponent<ItemInfo_Action>();
        param.field = "ID";

        EventDelegate onClick = new EventDelegate(gameObject.GetComponent<StoreManager>(), "Buy_Item");

        onClick.parameters[0] = param;

        EventDelegate.Add(button.onClick, onClick);
    }

    public void Buy_Item(int item_id)
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "SetBuyItem");
        sendData.Add("user_gold", GameManager.Get_Inctance().Gold);
        sendData.Add("item_id", item_id);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyBuyItem));
    }
    public void ReplyBuyItem(string json)
    {
        // JsonReader.Deserialize() : 원하는 자료형의 json을 만들 수 있다
         RecvBuyItem dataDic = JsonReader.Deserialize<RecvBuyItem>(json);

        GameManager.Get_Inctance().Set_BuyItem(dataDic.item_id, 7000, 1);


    }

    private class RecvBuyItem
    {
        public int gold;
        public int item_id;
        public  int item_count;
    }
}
