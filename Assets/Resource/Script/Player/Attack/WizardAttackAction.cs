using UnityEngine;
using System.Collections;

// Wizard의 Attack Effect에 붙어있는 스크립트.
// 현재는 앞으로 나아가는 기능만 있다.
public class WizardAttackAction : MonoBehaviour {

    void OnEnable()
    {
        StartCoroutine(C_Update());
    }

    IEnumerator C_Update()
    {
        while (true)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 8f);
            yield return null;
        }
    }
}
