using UnityEngine;
using System.Collections;

public class MarkAction : MonoBehaviour {

    public GameObject Target = null;

    public void Start_Update()
    {
        StartCoroutine(C_Update());
    }
    IEnumerator C_Update()
    {
        while (true)
        {
            Vector3 p = Camera.main.WorldToViewportPoint(Target.transform.position);
            transform.position = UICamera.mainCamera.ViewportToWorldPoint(p);

            p = transform.localPosition;
            p.x = Mathf.RoundToInt(p.x);
            p.y = Mathf.RoundToInt(p.y);
            p.z = 0f;
            transform.localPosition = p;

            yield return null;
        }
    }

    public void Stop_obj()
    {
        StopAllCoroutines();

        Destroy(gameObject);
    }
}
