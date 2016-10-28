using UnityEngine;
using System.Collections;

public class BossKinokoAction : MonsterAction {

    public GameObject LongAttack_Effect = null;                                     // 장거리공격 Effect
    public GameObject LongAttack_Position = null;                                 // 장거리공격이 나오는 위치

    bool First_Appear = false;

    float PoisonDamage = 1f;
    float PoisonTime = 5f;

    void OnEnable()
    {
        if(First_Appear == true) { return; }

        ActionCamera_Action.Get_Inctance().Set_preparation(transform, "Boss");

        First_Appear = true;
        type = TYPE.BOSS;
        ani = GetComponent<Animator>();
    }

    public override void StartSet_Attack()
    {
        UIManager.Get_Inctance().Set_BossHp("버섯돌이", gameObject);

        // 이미 Attack Coroutine이 실행되고 있거나 사망했으면 함수를 종료한다
        // 만약 CSet_Attack이 실행중인데 Start를 하게되면 Error가 난다.
        // Attack Corountine이 돌아가면 state가 Attack상태이고 state가 Attack이 아니면 Attack Coroutine이 내부에서 종료된다.
        // state가 Attack상태라면 이미 Attack Coroutine이 돌아가고 있다는 말이기 때문에 Start할 필요가 없으므로 return한다.
        if (state == STATE.ATTACK || state == STATE.DEAD) { return; }

        StartCoroutine(CSet_AniAttack());

    } 
    // 공격하는 Ani를 실행시키는 Coroutine.
    // 피가 가장 낮은 Player를 공격한다.
    IEnumerator CSet_AniAttack()
    {
        state = STATE.ATTACK;
        StartCoroutine(C_SpecialAttack());

        while (true)
        {
            // state가 Attack이나 도발 ( 특정 상대를 공격 ) 상태가 아니면 Coroutine을 종료한다.
            if (state != STATE.ATTACK && state != STATE.PROVOCATION) { yield break; }

            if (Target == null || Target.Check_Dead())
            {
                Target = PlayerManager.Get_Inctance().Get_Player_LowHp().GetComponent<PlayerAction>();
            }

            ani.SetBool("Attack", true);


            yield return null;
        }
    }
    // 특수 공격( 독, 장거리공격 등등)을 1초간격으로 하는 Coroutine.
    IEnumerator C_SpecialAttack()
    {
        while (true)
        {
            // state가 Attack이나 도발 ( 특정 상대를 공격 ) 상태가 아니면 Coroutine을 종료한다.
            if (state != STATE.ATTACK && state != STATE.PROVOCATION) { yield break; }

            float random = Random.Range(0, 100);
            // 10%확률로 Target에게 독공격을 한다.
            if (random < 10)
            {
                Debuff_poison();
            }  
            // 20%확률로 장거리 공격을 한다.
          else if (random < 30)
            {
                ani.SetTrigger("LongAttack");
                ani.SetTrigger("Idle");
            }

            yield return new WaitForSeconds(1f);

        }
    }
    // Target을 공격하는 함수. Attack Ani에서 이 함수를 호출한다.
    public void Player_Attack()
    {
        Target.Set_Demage(Attack, null);
    }
    
    // 장거리 공격의 Effect를 만드는 함수. Long Attack Ani에서 이 함수를 호출한다
    public void Set_LongAttack_Effect()
    {
        GameObject LongAttack = Instantiate(LongAttack_Effect, LongAttack_Position.transform.position, Quaternion.identity) as GameObject;
        LongAttack.name = "Kinoko_LongAttack";
    }

    // Player중 랜덤한 한명에게에게 독 상태이상을 거는 함수.
    public void Debuff_poison()
    {
        // Player중 아무나 한명을 받아온다.,
        PlayerAction Target = PlayerManager.Get_Inctance().Get_RandomPlayer().GetComponent<PlayerAction>();

        // 대상에게 독 상태이상을 건다.
        Target.Set_StatePoison(PoisonDamage, PoisonTime);

        // 독 Effect를 대상이 있는 곳에 만든다.
        GameObject Effectc = Instantiate(EffectManager.Get_Inctance().Poison_Effect, Vector3.zero, Quaternion.identity) as GameObject;
        Effectc.name = "PoisonEffect";
        Effectc.transform.position = Target.gameObject.transform.position;
    }

    // Charm에 걸렸을때 실행되는 함수.
    // time만큼 지난 후에는 Effect를 끄고 StartSet_Attack()를 실행시킨다. 몬스터 타입에 따라 상태이상을 표시하는 방식이 달라진다.
    public override void Set_StateCharm(float time)
    {
        state = STATE.CHARM;

       GameObject States = UIManager.Get_Inctance().Boss_HP.transform.FindChild("State").gameObject;
        States.GetComponent<UI_StateManager>().Start_PlayerState("Charm", time);

        Invoke("StartSet_Attack", time);
    }


    // A과 B의 거리를 구하는 함수
    float Distance(Vector3 A, Vector3 B)
    {
        return Mathf.Abs(Vector3.Distance(A, B));
    }
}
