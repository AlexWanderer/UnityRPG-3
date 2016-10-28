using UnityEngine;
using System.Collections;

// Pirate의 NPC에 관한 스크립트
public class Henchman_Action : MonoBehaviour
{
    public GameObject Damage_Effect = null;                                 // 충돌 Effect OBJ
    public float Attack = 0;
    public float Time_duration = 0;
    Vector3 StandPos = Vector3.zero;

    void Awake()
    {
        StandPos = transform.position;
    }
   
    // 공격 Coroutine을 시작하는 함수.
    public void Start_Attack()
    {
        StandPos = transform.localPosition;

        // Time_duration 후에도 아무와 부딪치지않으면 폭발을 실행시킨다.
        Invoke("Set_Explosion", Time_duration);

        StartCoroutine(C_Start_Attack());
    }
    // 공격 Coroutine.
    IEnumerator C_Start_Attack()
    {
        float y = -1f;
        while (true)
        {
            // Skill을 사용할때 NPC가 땅에서 올라오는 부분. 
            if (y <= 0)
            {
                y += Time.deltaTime;
                transform.Translate(Vector3.up * Time.deltaTime * 1f);
            }
            // 땅에서 올라오고 난 후 이동하는 부분.
            else
            {
                transform.Translate(Vector3.forward * Time.deltaTime * 2f);
            }
            yield return null;
        }
    }
    // 폭발 Effect를 실행시키는 함수.
   public void Set_Explosion()
    {
        // NPC의 모델을 끄고 폭발 Effect를 실행시킨다.
        transform.FindChild("Model").gameObject.SetActive(false);
        transform.FindChild("Explosion").gameObject.SetActive(true);

        // 1초 후에 OBJ를 초기화한다.
        Invoke("Reset", 1f);
    }
    // 폭발 이펙트가 끝난후 OBJ를 초기화하는 함수.
    void Reset()
    {
        transform.localPosition = StandPos;
        transform.FindChild("Model").gameObject.SetActive(true);
        transform.FindChild("Explosion").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider obj)
    {
        // 몬스터와 충돌하면 폭발한다.
        if(obj.CompareTag("Monster"))
        {
            obj.GetComponent<MonsterAction>().Set_Demage(Attack, "Skill");
            Set_Explosion();
        }
    }
}
