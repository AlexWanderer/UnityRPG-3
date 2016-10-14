using UnityEngine;
using System.Collections;

// UI를 관리하는 스크립트
public class UIManager : MonoBehaviour
{

    public GameObject Hpbar_Prefab = null;                             // Hpbar Object Prefeb
    public GameObject Damage_Prefab = null;                         // Damage Text Prefab              
    public GameObject Heal_Prefab = null;                               // Heal Text Prefab
    public GameObject SkillDamage_Prefab = null;                  // Skill Damage Text Prefab
    public UISprite[] Players_HP = null;                                     // UI Player Hpbars ( 0 : Center  1 : Sub1    2 : Sub2 )
    public GameObject[] Player_State = null;

    public UIScrollBar Space = null;
    public UILabel Timer = null;
    public GameObject BossWarning = null;

    public GameObject Boss_HP = null;

    public GameObject Mark_Prefab = null;

    public GameObject Boss_UI = null;
    public GameObject Boss_StateUI = null;

    

    private static UIManager instance = null;

    // Use this for initialization
    void Awake()
    {
        instance = this;

    }
    public static UIManager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(UIManager)) as UIManager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("UIManager ");
            instance = obj.AddComponent(typeof(UIManager)) as UIManager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

    public void Set_PlayerHp(float value, string typename)
    {
        int type = -1;
        switch(typename)
        {
            case "Center":
                {
                    type = 0;
                    break;
                }
            case "Sub1":
                {
                    type = 1;
                    break;
                }
            case "Sub2":
                {
                    type = 2;
                    break;
                }
        }

        if(type == -1)
        {
            Debug.Log("Player Hp Error");
            return;
        }

        Players_HP[type].fillAmount = value;
    }

    public void Set_BossHp(string name, GameObject target)
    {
        Boss_HP.GetComponent<BossHpAction>().name = name;
        Boss_HP.GetComponent<BossHpAction>().Target = target;
        Boss_HP.SetActive(true);
        Boss_HP.GetComponent<BossHpAction>().Set_Start();
    }

    // Target의 위치에 Hpbar를 만드는 함수.
    public void Set_Hpbar(GameObject target)
    {
        GameObject Hpbar = Instantiate(Hpbar_Prefab);
        Hpbar.transform.parent = GameObject.Find("UI Root").transform;
        Hpbar.transform.localScale = Vector3.one;
        Hpbar.GetComponent<HpbarAction>().Target = target;
        Hpbar.GetComponent<HpbarAction>().Start_Update();

    }
    // Target의 위치에 Damage Text를 만드는 함수
    // type이 null이면 Damage_Prefab을 Skill이면 SkillDamage_Prefab을 만든다.
    public void Set_Damage(GameObject target, float damage, string type)
    {
        GameObject DamageText = null;
        if (type == null)
        {
            DamageText = Instantiate(Damage_Prefab);
        }
        else if (type.Equals("Skill"))
        {
            DamageText = Instantiate(SkillDamage_Prefab);
        }

        DamageText.transform.parent = GameObject.Find("UI Root").transform;
        DamageText.transform.localScale = Vector3.one;
        DamageText.transform.localPosition = Vector3.zero;

        DamageText.GetComponent<DamageAction>().Set_Active(target, damage);
    }
    // Heal이 실행되면 Heal Text를 만드는 함수. 
    public void Set_Heal(GameObject target, float damage)
    {
        GameObject HealText = Instantiate(Heal_Prefab);
        HealText.transform.parent = GameObject.Find("UI Root").transform;
        HealText.transform.localScale = Vector3.one;
        HealText.transform.localPosition = Vector3.zero;

        HealText.GetComponent<HealAction>().Set_Active(target, damage);
    }

    public void Set_PlayerState(string TargetName, string Symptom, float time)
    {
        for (int i = 0; i < Player_State.Length; i++)
        {
            string str = string.Format("{0}State", TargetName);

            if(Player_State[i].name.Contains(str))
            {
                Player_State[i].GetComponent<UI_StateManager>().Start_State(Symptom, time);
                return;
            }
        }
    }

    public GameObject Set_Mark(GameObject target)
    {
        GameObject mark = Instantiate(Mark_Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        mark.transform.parent = GameObject.Find("UI Root").transform;
        mark.transform.localScale = Vector3.one;
        mark.transform.localPosition = Vector3.zero;

        mark.GetComponent<MarkAction>().Target = target;
        mark.GetComponent<MarkAction>().Start_Update();

        return mark;
    }

    public void Set_Space(float value)
    {
        Space.value = value;
    }
    public void Set_Time(int value)
    {
        int HH = value / 60;
        int MM = value % 60;

        string form = string.Format("{0:D2}:{1:D2}", HH, MM);
        Timer.text = form;
    }

    public void Set_Warning()
    {
        BossWarning.GetComponent<WarningAction>().Set_Warning();
    }
}
