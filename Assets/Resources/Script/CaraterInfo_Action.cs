using UnityEngine;
using System.Collections;

public class CaraterInfo_Action : MonoBehaviour {

    public UILabel Label_Name;
    public UISprite CharaterIcon;
    public UILabel Label_Attack;
    public UILabel Label_Defense;
    public GameObject Types;
    public GameObject Stars;
    public string Equipment;
    public GameObject Check;

    public void Set_CharaterInfo(string name, int attack, int defense, string type, int star )
    {
        Label_Name.text = name;
        CharaterIcon.spriteName = name;
        Label_Attack.text = attack.ToString();
        Label_Defense.text = defense.ToString();
        Types.transform.FindChild(type).gameObject.SetActive(true);
        
        for(int i = 0; i < star; i++)
        {
            Stars.transform.GetChild(i).gameObject.SetActive(true);
        }

    }

    public void Click_Charater()
    {
        if (GameManager.Get_Inctance().SelectCharater(Label_Name.text, !Check.activeSelf) == false)
        {
            return;
        }

        Check.SetActive(!Check.activeSelf);
    }
}
