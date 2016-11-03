using UnityEngine;
using System.Collections;

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
}
