using UnityEngine;
using System.Collections;

public class Kinoko_LongAttack_Action : MonoBehaviour {

    Rigidbody rigidbody = null;

    Vector3 target_pos = Vector3.zero;

    // Use this for initialization
    void Awake () {

        rigidbody = GetComponent<Rigidbody>();

        target_pos = PlayerManager.Get_Inctance().LongAttackPoint.transform.position;


        StartCoroutine(C_Move());
	}

    IEnumerator C_Move()
    {
        while(true)
        {
            Vector3 
        }
    }

    void OnCollisionEnter(Collision obj)
    {
        if (obj.gameObject.CompareTag("Monster"))
            return;

        Destroy(gameObject);
    }
}
