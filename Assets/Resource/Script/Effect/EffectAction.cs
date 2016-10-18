using UnityEngine;
using System.Collections;

// Effect에 붙는 스크립트.
// 
public class EffectAction : MonoBehaviour {

   public bool active = false;
    Vector3 StandPos = Vector3.zero;

	void OnEnable()
    {
        active = true;
        StandPos = transform.position;
    }

    public void Disenable()
    {
        gameObject.transform.position = StandPos;
        active = false;
        gameObject.SetActive(false);
    }
}
