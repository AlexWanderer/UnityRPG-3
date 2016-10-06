using UnityEngine;
using System.Collections;

public class Kinoko_LongAttack_Explosion_Action : MonoBehaviour {

    float damage = 5f;

    void OnTriggerEnter(Collider obj)
    {
        if(obj.CompareTag("Player"))
        {
            obj.GetComponent<PlayerAction>().Set_Demage(damage, null);
        }
    }
}
