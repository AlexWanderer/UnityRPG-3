using UnityEngine;
using System.Collections;

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

    public void View_AddItem(int item_id)
    {
        ITEM info = ItemManager.Instance.Get_ItemInfo(item_id);
        GameObject item = Instantiate(InvenItem_Prefab, Items.transform) as GameObject;
        item.transform.localScale = Vector3.one;
        item.name = info.Name;

        item.GetComponent<InvenItem_Action>().Set_ItemInfo(info.id, info.Icon_Name, info.Idx, info.Grade, info.Name, info.Type);

        Items.GetComponent<UIGrid>().repositionNow = true;
    }
}
