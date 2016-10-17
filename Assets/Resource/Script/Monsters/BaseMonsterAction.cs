using UnityEngine;
using System.Collections;

// MikuNegi Monster의 스크립트.
public class BaseMonsterAction : MonsterAction {

    void OnEnable()
    {
        type = TYPE.BASE;
        UIManager.Get_Inctance().Set_Hpbar(gameObject);                             // 몬스터가 활성화되면 UIManager에게 자신의 Hpbar를 만들라고 요구한다.
    }

    public override void Set_Idle()
    {
        StopAllCoroutines();
        state = STATE.IDLE;
        ani.SetTrigger("Idle");
    }
    public override void Set_Dead()
    {
        state = STATE.DEAD;
        StopAllCoroutines();
        MonsterManager.Get_Inctance().Check_Dead(gameObject);
        ani.SetBool("Dead", true);
    }
    public override void StartSet_Attack()
    {
        state = STATE.ATTACK;

        // 만약 CSet_Attack이 실행중인데 Start를 하게되면 Error가 난다. 
        try
        {
            StartCoroutine(CSet_Attack());
        }
        catch
        {
            StopCoroutine(CSet_Attack());
            StartCoroutine(CSet_Attack());
        }
    }
    public override bool Set_Demage(float AttackDamage, string type)
    {
        Hp -= AttackDamage;
        UIManager.Get_Inctance().Set_Damage(gameObject, AttackDamage, type);

        if (Hp <= 0)
        {
            // 만약 Hp가 0이하면 관리자에게 죽었다고 보고한다.
            Set_Dead();
            return false;
        }

        return true;
    }
    public override void Set_Charm(float time)
    {
        state = STATE.CHARM;

        GameObject CharmEffect = Instantiate(EffectManager.Get_Inctance().Charm_Effect) as GameObject;
        CharmEffect.transform.parent = Condition.transform;
        CharmEffect.transform.localPosition = Vector3.zero;

        Invoke("State_OFF", time);
        Invoke("StartSet_Attack", time);
    }
    public override void State_OFF()
    {
        state = STATE.IDLE;
        Condition.transform.DestroyChildren();
    }

    // 공격하는 Coroutine.
    IEnumerator CSet_Attack()
    {
        while (true)
        {
            if (state != STATE.ATTACK && state != STATE.PROVOCATION)
            {
                yield return null;
                continue;
            }

            if (Target == null || Target.activeSelf == false)
            {
                PlayerManager.Get_Inctance().Set_ReTarget(this);
            }

            // Target이 있는쪽을 바라본다.
            Vector3 targetPos = Target.transform.position;
            targetPos.y = transform.position.y;
            Vector3 v = targetPos - transform.position;
            transform.rotation = Quaternion.LookRotation(v);

            // Target과의 거리가 1.5f이상이면 앞으로 움직이고 아니면 공격한다.
            if (Distance(Target.transform.position, transform.position) > 1.5f)
            {
                ani.SetBool("Attack", false);
                ani.SetBool("Move", true);
                transform.Translate(Vector3.forward * Time.deltaTime * 1f);
            }
            else
            {
                ani.SetBool("Attack", true);
            }


            yield return null;
        }
    }

    public void Player_Attack()
    {
        Target.GetComponent<PlayerAction>().Set_Demage(Attack, null);
    }



    void OnCollisionEnter(Collision obj)
    {
        // Player랑 충돌하면 Target을 충돌한 Player로 변경한다.
        if(obj.gameObject.CompareTag("Player"))
        {
            if(state == STATE.PROVOCATION)
            {
                return;
            }

            Target = obj.gameObject;
        }
    }
    // A과 B의 거리를 구하는 함수
    float Distance(Vector3 A, Vector3 B)
    {
        return Mathf.Abs(Vector3.Distance(A, B));
    }
}
