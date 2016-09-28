using UnityEngine;
using System.Collections;

// Warrior의 TouchAttack의 충돌판정을 위한 스크립트.
// Warrior의 Axe의 SkillPoint에 Collider랑 스크립트가 있다.
public class Warrior_TouchAttack_Action : MonoBehaviour {

    void OnTriggerEnter(Collider obj)
    {
        if(obj.gameObject.CompareTag("Monster"))
        {
            obj.GetComponent<MonsterAction>().Set_Demage(10f, null);
        }
    }

}
