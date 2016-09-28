using UnityEngine;
using System.Collections;

// UI를 관리하는 스크립트
public class UIManager : MonoBehaviour
{

    public GameObject Hpbar_Prefab = null;                             // Hpbar Object Prefeb
    public GameObject Damage_Prefab = null;                         // Damage Text Prefab              
    public GameObject Heal_Prefab = null;                               // Heal Text Prefab
    public GameObject SkillDamage_Prefab = null;                  // Skill Damage Text Prefab


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
}
