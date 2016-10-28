using UnityEngine;
using System.Collections;

public class CharaterMove_Action : MonoBehaviour {

    public GameObject Charater = null;
    Quaternion StandRotation;
    public float Speed = 3f;
    bool moveCheck = true;

    void Awake()
    {
        Charater = transform.GetChild(0).gameObject;
        StandRotation = Charater.transform.rotation;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) == true)
        {
            if(moveCheck == false)
            {
                return;
            }

            Charater.GetComponent<Animator>().SetBool("Move", true);

            // 카메라에서 화면상의 마우스 좌표에 해당하는 공간으로 레이를 쏜다.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Physics.Raycast(쏜 레이 정보, 충돌 정보, 거리)
            //  => 충돌이 되면 true를 리턴하면서 충돌 정보를 확인 할 수 있다.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.CompareTag("Player")) { return; }

                Vector3 target = hit.point;
                target.y = Charater.transform.position.y;
                target.z = Charater.transform.position.z;
                Vector3 v = target - Charater.transform.position;
                float distance = Vector3.Distance(target, Charater.transform.position);

                if (Mathf.Abs(distance) < 0.1f ) //클릭한 곳과 Player의 거리가 0.1이하면 종료. 그렇지 않으면 플레이어가 터치한곳에 도착후 계속 버벅이며 움직인다.
                    return;

                Charater.transform.rotation = Quaternion.LookRotation(v);

                float check = Mathf.Sign(Charater.transform.rotation.y);

                if (check == 1)
                {
                    transform.Translate( Vector3.right * Time.deltaTime * Speed );
                }
                else
                {
                    transform.Translate(Vector3.left * Time.deltaTime * Speed);
                }

                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);
            }
        }
        else
        {
            Charater.GetComponent<Animator>().SetBool("Move", false);
            Charater.transform.rotation = StandRotation;
        }
    }

    public void Do_Move()
    {
        moveCheck = true;
    }
    public void DoNot_Move()
    {
        moveCheck = false;
    }
}
