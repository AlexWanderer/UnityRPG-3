using UnityEngine;
using System.Collections;

// Wizard AttackEffect에 붙는 스크립트.
public class Wizard_AttackEffectAction : MonoBehaviour {

   public bool active = false;
    Vector3 StandPos = Vector3.zero;
    float speed = 3f;

	void OnEnable()
    {
        active = true;
        StandPos = transform.position;
        StartCoroutine(C_Update());
    }
    IEnumerator C_Update()
    {
        while (true)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            yield return null;
        }
    }

    public void Disenable()
    {
        StopCoroutine(C_Update());
        gameObject.transform.position = StandPos;
        active = false;
        gameObject.SetActive(false);
    }
}
