using UnityEngine;
using System.Collections;

// Warrior의 TouchAttackEffect의 충돌판정을 위한 스크립트.
// Warrior의 Axe의 SkillPoint에 Collider랑 스크립트가 있다.
public class Warrior_TouchAttackEffect_Action : MonoBehaviour {

    ParticleSystem particle;
    public bool active = false;
    Vector3 StandPos = Vector3.zero;
    public float Damage = 0;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Check_Alive()
    {
        active = true;
        gameObject.SetActive(true);
        StandPos = transform.position;
        StartCoroutine(C_Check_Alive());
    }

    IEnumerator C_Check_Alive()
    {
        while (true)
        {
            if (particle.IsAlive() == false)
            {
                active = false;
                gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }

    }

void OnTriggerEnter(Collider obj)
{
    // 충돌한 몬스터들에게 Damage를 준다.
    if (obj.gameObject.CompareTag("Monster"))
    {
        if (Damage == 0) { return; }

        obj.GetComponent<MonsterAction>().Set_Demage(Damage, null);
    }
}
}
