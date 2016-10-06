using UnityEngine;
using System.Collections;

public class BossKinokoAction : MonsterAction {

    public GameObject LongAttack_Effect = null;
    public GameObject LongAttack_Position = null;

    void Awake()
    {
        ActionCamera_Action.Get_Inctance().Set_preparation(transform, "Boss");
        ani = GetComponent<Animator>();
    }
    public override void Set_Idle()
    {
        StopAllCoroutines();
        state = STATE.IDLE;
        ani.SetTrigger("Idle");
    }
    public override void Set_Dead()
    {
        if (state == STATE.DEAD)
            return;

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
        if (state == STATE.DEAD)
            return false;

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
    }
    public override void State_OFF()
    {
        state = STATE.IDLE;
        Condition.transform.DestroyChildren();
    }

    // 공격하는 Coroutine.
    IEnumerator CSet_Attack()
    {
        StartCoroutine(C_LongAttack());

        while (true)
        {
            if (state != STATE.ATTACK)
            {
                yield return null;
                continue;
            }

            if (Target == null)
            {
                yield return null;
                continue;
            }

            if (Target.activeSelf == false)
            {
                Target = null;
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                Debuff_poison();
            }

            ani.SetBool("Attack", true);


            yield return null;
        }
    }
    IEnumerator C_LongAttack()
    {
        while (true)
        {
            float random = Random.Range(0, 100);

            if (Target == null)
            {
                random = 0;
            }

            if (random < 20)
            {
                ani.SetTrigger("LongAttack");
                ani.SetTrigger("Idle");
                yield return new WaitForSeconds(1f);
                continue;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }


    public void Player_Attack()
    {
        Target.GetComponent<PlayerAction>().Set_Demage(Attack, null);
    }

    public void Long_Attack()
    {
        GameObject LongAttack = Instantiate(LongAttack_Effect, LongAttack_Position.transform.position, Quaternion.identity) as GameObject;
        LongAttack.name = "Kinoko_LongAttack";
    }

    public void Debuff_poison()
    {
        GameObject Target = PlayerManager.Get_Inctance().Get_RandomPlayer();
        Target.GetComponent<PlayerAction>().Set_Poison();

        GameObject Effectc = Instantiate(EffectManager.Get_Inctance().Poison_Effect, Vector3.zero, Quaternion.identity) as GameObject;
        Effectc.name = "PoisonEffect";
        Effectc.transform.position = Target.transform.position;
    }

    void OnCollisionEnter(Collision obj)
    {
        // Player랑 충돌하면 Target을 충돌한 Player로 변경한다.
        if (obj.gameObject.CompareTag("Player"))
        {
            Target = obj.gameObject;
        }
    }
    // A과 B의 거리를 구하는 함수
    float Distance(Vector3 A, Vector3 B)
    {
        return Mathf.Abs(Vector3.Distance(A, B));
    }
}
