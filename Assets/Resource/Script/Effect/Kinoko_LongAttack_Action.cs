using UnityEngine;
using System.Collections;

// 보스 Kinoko의 LongAttack에 관한 스크립트.
public class Kinoko_LongAttack_Action : MonoBehaviour {

    GameObject target_pos = null;                                               
    GameObject collision_Effect = null;
    GameObject Defalut_Effect = null;
    GameObject mark = null;

    float speed = 0.08f;

    // Use this for initialization
    void Awake () {

        target_pos = PlayerManager.Get_Inctance().LongAttackPoint;
        collision_Effect = transform.FindChild("Explosion").gameObject;
        Defalut_Effect = transform.FindChild("Defalut").gameObject;

        mark = UIManager.Get_Inctance().Set_Mark(gameObject);

        StartCoroutine(C_Move());
        Invoke("Obj_Off", 5f);
	}

    IEnumerator C_Move()
    {
        Vector3 target = target_pos.transform.position;
        target.x += Random.Range(-2f, 2f);

        while (true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                // 카메라에서 화면상의 마우스 좌표에 해당하는 공간으로 레이를 쏜다.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Physics.Raycast(쏜 레이 정보, 충돌 정보, 거리)
                //  => 충돌이 되면 true를 리턴하면서 충돌 정보를 확인 할 수 있다.
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    // 충돌한 obj를 가져와 obj가 Player일 경우 Skill을 발동시킨다.
                    GameObject obj = hit.collider.gameObject;

                    if (obj == gameObject)
                    {
                        Defalut_Effect.SetActive(false);
                        collision_Effect.SetActive(true);
                        mark.GetComponent<MarkAction>().Stop_obj();

                        Invoke("Obj_Off", 0.3f);
                        yield break;
                    }
                }
            }

            Vector3 v = target - transform.position;
            transform.rotation = Quaternion.LookRotation(v);

            transform.Translate(Vector3.forward * speed);

            yield return null;
        }
    }

    void Obj_Off()
    {
        Destroy(gameObject);

        if (mark != null)
        {
            mark.GetComponent<MarkAction>().Stop_obj();
        }
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.CompareTag("Monster") || obj.gameObject.name.Equals("Kinoko_LongAttack") ||
               obj.gameObject.name.Equals("Explosion")) { return; }




        if (obj.gameObject.CompareTag("Player"))
        {
            collision_Effect.GetComponent<SphereCollider>().enabled = true;
        }

        StopAllCoroutines();
        GetComponent<SphereCollider>().enabled = false;

        if (mark != null)
        {
            mark.GetComponent<MarkAction>().Stop_obj();
        }

        Defalut_Effect.SetActive(false);
        collision_Effect.SetActive(true);

        Invoke("Obj_Off", 0.3f);
    }
}
