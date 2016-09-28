using UnityEngine;
using System.Collections;

// Damage Text에 관련된 Object에 대한 스크립트
public class DamageAction : MonoBehaviour {

    // Damage Text의 초기값을 설정하는 함수
    public void Set_Active(GameObject target, float value)
    {
        if (target == null)
            return;

        //Damage Text의 위치를 조정한다. 
        try
        {
            Vector3 p = Camera.main.WorldToViewportPoint(target.transform.position);

            transform.position = UICamera.mainCamera.ViewportToWorldPoint(p);

            p = transform.localPosition;
            p.x = Mathf.RoundToInt(p.x);
            p.y = Mathf.RoundToInt(p.y) + 30f;
            p.z = 0f;
            transform.localPosition = p;
        }
        catch { return; }
        //Damage Text의 text를 damage값으로 설정한다.
        GetComponentInChildren<UILabel>().text = ((int)value).ToString();
        GetComponent<TweenPosition>().to = transform.localPosition;
        GetComponent<TweenPosition>().from = transform.localPosition + new Vector3(0f, 1f, 0f);

        gameObject.SetActive(true);
    }

    public void Disenable()
    {
        Destroy(gameObject); // 임시
    }
}
