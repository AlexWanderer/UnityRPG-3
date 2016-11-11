using UnityEngine;
using System.Collections;

public class Charater_DetailedInfo_Action : MonoBehaviour
{
    public int Index;
    public UILabel Label_Name;
    public UISprite Type;
    public UILabel Label_attack;
    public UILabel Label_defense;
    public UISprite Touchskill_Icon;
    public UILabel Touchskill_Explanation;
    public UISprite Specialskill_Icon;
    public UILabel Specialskill_Explanation;
    public GameObject UI_Charater;
    public UISprite Equipment_Icon;
    public UILabel[] Label_Equipment_Effects;

    void OnEnable()
    {
        for (int i = 0; i < Label_Equipment_Effects.Length; i++)
        {
            Label_Equipment_Effects[i].gameObject.SetActive(false);
        }

        if (GameManager.Get_Inctance().Charater_Equipment.ContainsKey(Index))
        {
            Equipment_Icon.gameObject.SetActive(true);

            ITEM EquipmentInfo = ItemManager.Instance.Get_ItemInfo(GameManager.Get_Inctance().Charater_Equipment[Index]);

            Equipment_Icon.spriteName = EquipmentInfo.Icon_Name;

            int Effect_Type = (int)EquipmentInfo.Effect_Type;

            if (EquipmentInfo.Effect_Type <= ITEMEFFECT_TYPE.DEFENSE && EquipmentInfo.Effect_Type != ITEMEFFECT_TYPE.NULL)
            {
                string[] Effects = EquipmentInfo.Effects.Split(',');

                Label_Equipment_Effects[Effect_Type].gameObject.SetActive(true);
                Label_Equipment_Effects[Effect_Type].text = "(+"+Effects[Effect_Type]+")";
            }
        }
        else
        {
            for (int i = 0; i < Label_Equipment_Effects.Length; i++)
            {
                Label_Equipment_Effects[i].gameObject.SetActive(false);
                Equipment_Icon.gameObject.SetActive(false);
            }
        }
    }

    public void Set_Charater_DetailedInfo(int index)
    {
        RecvCharaterInfo data = CharaterManager.Instance.Get_CharaterInfo(index);

        Label_Name.text = data.Name;
        Type.spriteName = data.Type;
        Label_attack.text = data.Attack.ToString();
        Label_defense.text = data.Defense.ToString();

        Touchskill_Icon.spriteName = data.Name + "_TouchSkill";
        Specialskill_Icon.spriteName = data.Name + "_SpecialSkill";

        Touchskill_Explanation.text = data.Touch_text;
        Specialskill_Explanation.text = data.Special_text;

        if (UI_Charater.transform.FindChild("UI_" + data.Name) != null)
        {
            GameObject Charater = UI_Charater.transform.FindChild("UI_" + data.Name).gameObject;
            Charater.SetActive(true);
        }
        else
        {
            GameObject CharaterPrefab = Resources.Load("Prefabs/UI/Charater/UI_" + data.Name) as GameObject;
            GameObject Charater = Instantiate(CharaterPrefab, UI_Charater.transform) as GameObject;
            Charater.transform.localPosition = Vector3.zero;
            Charater.transform.localScale = Vector3.one;
            Charater.transform.localRotation = Quaternion.identity;
            Charater.name = "UI_" + data.Name;
        }

        for (int i = 0; i < UI_Charater.transform.childCount; i++)
        {
            if(UI_Charater.transform.GetChild(i).name.Equals("UI_"+ data.Name)) { continue; }

            UI_Charater.transform.GetChild(i).gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    public void Set_Equipment(int item_id)
    {
        Equipment_Icon.gameObject.SetActive(true);
        GameManager.Get_Inctance().Set_Charater_Equipment(Index, item_id);
    }
}
