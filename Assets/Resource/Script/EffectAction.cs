using UnityEngine;
using System.Collections;

// Effect에 붙는 스크립트.
// 현재는 시간이 지나면 알아서 꺼지는 내용밖에 없다.
public class EffectAction : MonoBehaviour {

	void OnEnable()
    {
        Invoke("Disenable", 3f);
    }

    public void Disenable()
    {
        Destroy(gameObject); // 임시
    }
}
