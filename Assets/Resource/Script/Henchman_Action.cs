using UnityEngine;
using System.Collections;

public class Henchman_Action : MonoBehaviour
{

    Animator ani = null;
    public GameObject Damage_Effect = null;
    Vector3 StandPos = Vector3.zero;

    void Awake()
    {
        ani = GetComponent<Animator>();
    }

    public void Start_Attack()
    {
        StandPos = transform.position;
        Invoke("Set_Explosion", 3f);
        StartCoroutine(C_Start_Attack());
    }
    void Set_AniMove()
    {
        ani.SetBool("Move", true);
    }
    IEnumerator C_Start_Attack()
    {
        float y = -1f;
        while (true)
        {
            if (y <= 0)
            {
                y += Time.deltaTime;

                transform.Translate(Vector3.up * Time.deltaTime * 1f);

            }
            else
            {
                Set_AniMove();
                transform.Translate(Vector3.forward * Time.deltaTime * 2f);
            }
            yield return null;
        }
    }
    void Set_Explosion()
    {
        transform.FindChild("Model").gameObject.SetActive(false);
        transform.FindChild("Explosion").gameObject.SetActive(true);
        StartCoroutine(C_Reset());
    }

    IEnumerator C_Reset()
    {
        yield return new WaitForSeconds(1f);

        gameObject.transform.position = StandPos;
        transform.FindChild("Model").gameObject.SetActive(true);
        transform.FindChild("Explosion").gameObject.SetActive(false);
        gameObject.SetActive(false);

    }

    void OnTriggerEnter(Collider obj)
    {
        if(obj.CompareTag("Monster"))
        {
            Set_Explosion();
        }
    }
}
