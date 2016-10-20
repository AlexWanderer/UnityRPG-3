using UnityEngine;
using System.Collections;

public class Support_Action : MonoBehaviour
{ 
    string Support_Name = "";
    public string[] Ment;
    public string SkillMent = "";
   public bool Skill_Use = false;
    public float Skill_Damage = 0f;
    public GameObject Support_ui = null;
    public UILabel Support_MentLabel = null;

    void Awake()
    {
        Support_Name = "Magica";

        EventDelegate onClick = new EventDelegate(this, "Support_Skill");
        EventDelegate.Add(GetComponentInChildren<UIButton>().onClick, onClick);

        UISprite Icon = Support_ui.transform.FindChild("Sprite_Icon").GetComponent<UISprite>();
        Icon.spriteName = "Supporter_"+Support_Name;

       Support_MentLabel =  Support_ui.transform.FindChild("Talk").GetComponentInChildren<UILabel>();
    }
    public virtual void Support_Skill()
    {

    }

}
