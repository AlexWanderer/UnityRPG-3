using UnityEngine;
using System.Collections;

public class BossKinokoAction: MonsterAction {

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
        while (true)
        {
            if (state != STATE.ATTACK)
            {
                continue;
            }

            if (Target == null || Target.activeSelf == false)
            {
                PlayerManager.Get_Inctance().Set_ReTarget(this);
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                GameObject LongAttack = Instantiate(LongAttack_Effect, LongAttack_Position.transform.position, Quaternion.identity) as GameObject;
                LongAttack.name = "Kinoko_LongAttack";

            }
           
            ani.SetBool("Attack", true);


            yield return null;
        }
    }

    public void Player_Attack()
    {
        Target.GetComponent<PlayerAction>().Set_Demage(Attack, null);
    }

    public void Long_Attack()
    {

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
