using UnityEngine;
using System.Collections;

public class Charater_DetailedInfo_Action : MonoBehaviour
{
    public UILabel Label_Name;
    public UISprite Type;
    public UILabel Label_attack;
    public UILabel Label_defense;
    public UISprite Touchskill_Icon;
    public UILabel Touchskill_Explanation;
    public UISprite Specialskill_Icon;
    public UILabel Specialskill_Explanation;
    public UISprite Item_Icon;
    public GameObject UI_Charater;

    public void Set_Charater_DetailedInfo(string charater_name)
    {
        RecvCharaterInfo data = CharaterManager.Instance.Get_CharaterInfo(charater_name);

        Label_Name.text = data.Name;
        Type.spriteName = data.Type;
        Label_attack.text = data.Attack.ToString();
        Label_defense.text = data.Defense.ToString();

        Touchskill_Icon.spriteName = charater_name + "_TouchSkill";
        Specialskill_Icon.spriteName = charater_name + "_SpecialSkill";

        Touchskill_Explanation.text = data.Touch_text;
        Specialskill_Explanation.text = data.Special_text;

        if (GameManager.Get_Inctance().Charater_Equipment.ContainsKey(charater_name))
        {
            Item_Icon.spriteName = GameManager.Get_Inctance().Charater_Equipment[charater_name];
        }

        if (UI_Charater.transform.FindChild("UI_" + charater_name) != null)
        {
            GameObject Charater = UI_Charater.transform.FindChild("UI_" + charater_name).gameObject;
            Charater.SetActive(true);
        }
        else
        {
            GameObject CharaterPrefab = Resources.Load("Prefabs/UI/Charater/UI_" + charater_name) as GameObject;
            GameObject Charater = Instantiate(CharaterPrefab, UI_Charater.transform) as GameObject;
            Charater.transform.localPosition = Vector3.zero;
            Charater.transform.localScale = Vector3.one;
            Charater.transform.localRotation = Quaternion.identity;
            Charater.name = "UI_" + charater_name;
        }

        for (int i = 0; i < UI_Charater.transform.childCount; i++)
        {
            if(UI_Charater.transform.GetChild(i).name.Equals("UI_"+charater_name)) { continue; }

            UI_Charater.transform.GetChild(i).gameObject.SetActive(false);
        }


        gameObject.SetActive(true);
    }
    
    public void Set_Charater_Equipment_Window()
    {
    }

    public void Set_Equipment(int item_id)
    {
        GameManager.Get_Inctance().Set_Charater_Equipment(Label_Name.text, item_id);
    }
}
