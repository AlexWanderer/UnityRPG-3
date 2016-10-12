using UnityEngine;
using System.Collections;

public class ExplosionAction : MonoBehaviour
{

    void OnTriggerEnter(Collider obj)
    {
        if(obj.CompareTag("Monster"))
        {
            obj.gameObject.GetComponent<MonsterAction>().Set_Demage(10f, "Skill");
        }
    }
}
