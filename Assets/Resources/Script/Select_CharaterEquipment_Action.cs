using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Select_CharaterEquipment_Action : MonoBehaviour {

    string Charater_Name;
    public UISprite Origin_Item_Icon;
    ITEM Origin_Item;
    public UILabel Origin_Description;
    public UISprite Select_Item_Icon;
    ITEM Select_Item;
    public UILabel Select_Description;
    public GameObject Button_Wear;

    public GameObject EquipmentOBJs;
    public GameObject Equipment_Prefab;

    void OnEnable()
    {
        Select_Item_Icon.gameObject.SetActive(false);
        Select_Description.gameObject.SetActive(false);

        for (int i = 0; i < EquipmentOBJs.transform.childCount; i++)
        {
            EquipmentOBJs.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void View_Equipment(string Type, string charater_name)
    {
        Charater_Name = charater_name;

        if(GameManager.Get_Inctance().Get_CharaterEquipment_id(Charater_Name) == -1)
        {
            Origin_Item_Icon.gameObject.SetActive(false);
            Origin_Description.gameObject.SetActive(false);
        }

        List<int> Equipments = GameManager.Get_Inctance().Get_Item;

        for (int i = 0; i < Equipments.Count; i++)
        {
            ITEM Equipment_info = ItemManager.Instance.Get_ItemInfo(Equipments[i]);

            if (Equipment_info.Type != ITEMTYPE.EQUIPMENT) { continue; }


            // 아래 if문 좀더 간단하게 못하나?
            if (Equipment_info.Limit_Type1 == "" && Equipment_info.Limit_Type2 == "" && Equipment_info.Limit_Type3 == "")
            {
                Set_EquipmentObj(Equipment_info);
            }
            else if (Equipment_info.Limit_Type1.Equals(Type) || Equipment_info.Limit_Type2.Equals(Type) || Equipment_info.Limit_Type3.Equals(Type))
            {
                Set_EquipmentObj(Equipment_info);
            }

        }
    }
    void Set_EquipmentObj(ITEM info)
    {
        for(int i = 0; i < EquipmentOBJs.transform.childCount; i++)
        {
            GameObject child = EquipmentOBJs.transform.GetChild(i).gameObject;

            if(child.name.Equals(info.Name))
            {
                child.SetActive(true);
                EquipmentOBJs.GetComponent<UIGrid>().repositionNow = true;

                return;
            }
        }

        GameObject Equipment = Instantiate(Equipment_Prefab, EquipmentOBJs.transform) as GameObject;
        Equipment.transform.localScale = Vector3.one;
        Equipment.name = info.Name;

        Equipment.GetComponent<SelectEquipmentInfo_Action>().Set_EquipmentInfo(info.id, info.Icon_Name, info.Name, info.Type, info.Description);
        Set_ChangeEquipmentButton(Equipment.GetComponent<UIButton>());

        EquipmentOBJs.GetComponent<UIGrid>().repositionNow = true;
    }
    void Set_ChangeEquipmentButton(UIButton button)
    {
        EventDelegate.Parameter param = new EventDelegate.Parameter();

        param.obj = button.gameObject.transform.GetComponent<SelectEquipmentInfo_Action>();
        param.field = "ID";

        EventDelegate onClick = new EventDelegate(gameObject.GetComponent<Select_CharaterEquipment_Action>(), "Select_Equipment");

        onClick.parameters[0] = param;

        EventDelegate.Add(button.onClick, onClick);
    }

    public void Select_Equipment(int Equipment_id)
    {
        Select_Item = ItemManager.Instance.Get_ItemInfo(Equipment_id);
        Select_Item_Icon.spriteName = Select_Item.Icon_Name;
        Select_Description.text = Select_Item.Description;

        Select_Item_Icon.gameObject.SetActive(true);
        Select_Description.gameObject.SetActive(true);
    }

    public void Set_Equipment()
    {
        Origin_Item_Icon.gameObject.SetActive(true);
        Origin_Description.gameObject.SetActive(true);
        Origin_Item = Select_Item;
        Origin_Item_Icon.spriteName = Origin_Item.Icon_Name;
        Origin_Description.text = Origin_Item.Description;

        GameManager.Get_Inctance().Set_Charater_Equipment(Charater_Name, Origin_Item.id);
    }
}
