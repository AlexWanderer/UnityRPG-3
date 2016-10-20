using UnityEngine;
using System.Collections;

public class GhostAttakEffect_Action : MonoBehaviour {

    public bool active = false;
    Vector3 StandPos = Vector3.zero;
    public float AttackDamage = 0f;
    float speed = 3f;

    void OnEnable()
    {
        active = true;
        StandPos = transform.position;
        GetComponent<SphereCollider>().enabled = true;
        transform.FindChild("Default").gameObject.SetActive(true);
        transform.FindChild("Explosion").gameObject.SetActive(false);
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
        GetComponent<SphereCollider>().enabled = false;
        transform.FindChild("Default").gameObject.SetActive(false);
        transform.FindChild("Explosion").gameObject.SetActive(true);

        Invoke("AtiveOf_Explosion", 0.6f);
    }
    void AtiveOf_Explosion()
    {
        active = false;
        transform.FindChild("Explosion").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider obj)
    {
        if(obj.CompareTag("Player"))
        {
            obj.GetComponent<PlayerAction>().Set_Demage(AttackDamage, null);
            Disenable();
        }
    }
}
