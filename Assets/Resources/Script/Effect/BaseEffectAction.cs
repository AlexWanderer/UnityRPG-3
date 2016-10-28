using UnityEngine;
using System.Collections;

// Effect에 관한 스크립트.
public class BaseEffectAction : MonoBehaviour {

    ParticleSystem particle;                                                    // 스크립트가 붙어있는 OBJ의 particle
    public bool active = false;                                                // Effect가 작동중인가? 아닌가?
    Vector3 StandPos = Vector3.zero;                                    // Effect 초기 생성 위치.

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }



    // Effect를 활성화시키고 자동으로 꺼지는 Coroutine을 실행시키는 함수.
    public void Start_Effect()
    {
        active = true;
        gameObject.SetActive(true);
        StandPos = transform.position;
        StartCoroutine(C_Check_Alive());
    }
    // Effect가 활성화중인지 아닌지를 체크해 변수값을 바꾸는 코루틴.
	IEnumerator C_Check_Alive()
    {
        while(true)
        {
            // Effect가 꺼지면 변수를 초기화하고 Coroutine을 종료한다.
            if (particle.IsAlive() == false)
            {
                active = false;
                gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
	}
}
