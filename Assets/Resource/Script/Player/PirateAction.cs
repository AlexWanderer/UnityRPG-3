using UnityEngine;
using System.Collections;

public class PirateAction : PlayerAction {

    public GameObject TouchAttack_Effect = null;                    // TouchAttack Effect obj
    public GameObject SpecialSkill_Effect = null;                      // SpecialAttack Effect obj
    public GameObject Attack_Pos = null;                                 // Effect 좌표


    void Awake()
    {
        ani = GetComponent<Animator>();
    }

    public override void Set_Move()
    {
        ani.SetBool("Move", true);
        ani.SetBool("Attack", false);
    }
    public override void Set_Attack()
    {
        state = STATE.ATTACK;
        ani.SetBool("Attack", true);
    }
    public override void Set_Idle()
    {
        StopAllCoroutines();
        ani.SetBool("Move", false);
        ani.SetBool("Attack", false);
        transform.localPosition = StandPos;
        transform.rotation = Quaternion.identity;

    }
    public override void Set_Dead()
    {
        state = STATE.DEAD;
        StopAllCoroutines();
        PlayerManager.Get_Inctance().Check_Dead();
        ani.SetBool("Dead", true);
    }

    public override bool Set_Demage(float AttackDamage, string type)
    {
        Hp -= AttackDamage;
        UIManager.Get_Inctance().Set_Damage(gameObject, AttackDamage, type);
        UIManager.Get_Inctance().Set_PlayerHp(Hp / InitHP, transform.parent.name);

        if (Hp <= 0)
        {
            // 만약 Hp가 0이하면 관리자에게 죽었다고 보고한다.
            Set_Dead();
            return false;
        }

        return true;
    }

    public void Start_Attack()
    {
        StartCoroutine(C_Attack());
    }
    IEnumerator C_Attack()
    {
        while (true)
        {
            // 상태가 Attack이 아닌경우 코루틴을 종료한다.
            if (state != STATE.ATTACK)
                yield break;

            // Target이 null이거나 죽어있는 MonsterManager에게 새로운 Target을 받아온다.
            if (Target == null || Target.state.ToString().Equals("DEAD"))
            {
                MonsterManager.Get_Inctance().Set_ReTarget(this);
            }

            // Target이 있는 쪽을 바라본다.
            Vector3 target = Target.transform.position;
            target.y = transform.position.y;
            Vector3 v = target - transform.position;
            transform.rotation = Quaternion.LookRotation(v);

            if (Distance(target, transform.position) > 10f)
            {
                if (state != STATE.SKILL)
                {
                    Set_Move();
                    Target_Move(Target.gameObject.transform.position);
                    yield return null;
                }
            }
            else
            {
                ani.SetBool("Attack", true);
            }

            yield return null;

        }
    }
    public void Monster_Attack()
    {
        Target.Set_Demage(Attack, null);
    }

    public override void Touch_Skill()
    {

        ani.SetTrigger("TouchSkill");
        ani.SetTrigger("Idle");
    }

    public void Set_TouchSkill()
    {
        Invoke("Stop_TouchSkillEffect", 5f);
        UIManager.Get_Inctance().Set_PlayerState(transform.parent.name, "Provocation", 5f);
        MonsterManager.Get_Inctance().Set_ParticularTarget(gameObject, 5f);
        TouchAttack_Effect.SetActive(true);
    }
    void Stop_TouchSkillEffect()
    {
        TouchAttack_Effect.SetActive(false);
    }


    public override void Special_Skill()
    {
        // Player가 Idle상태이거나 Move상태이면 버튼을 눌러도 함수가 실행되지 않는다.
        if (PlayerManager.Get_Inctance().state.ToString().Equals("IDLE") || PlayerManager.Get_Inctance().state.ToString().Equals("MOVE"))
            return;

        //Target이 null이거나 죽어있으면 실행하지 않는다.
        //if (Target == null || Target.state.ToString().Equals("DEAD"))
        //    return;

        StartCoroutine(C_Special_Skill());
    }
    IEnumerator C_Special_Skill()
    {


        // Target이 죽었을시 새로운 Target을 받는다.
        if (Target.gameObject.activeSelf == false)
            MonsterManager.Get_Inctance().Set_ReTarget(this);

        // ActionCamera에게 Wizard의 애니메이션을 실행시키게 한다.
        ActionCamera_Action.Get_Inctance().Set_preparation(transform, "Wizard");

        // Nurse를 제외한 모든 Player, Monster의 Active를 false시킨다.
        PlayerManager.Get_Inctance().Players_Active(gameObject, "OFF");
        MonsterManager.Get_Inctance().Monsters_Active(null, "OFF");

        yield return new WaitForSeconds(0.25f);

        ani.SetTrigger("SpecialSkill");

        yield return new WaitForSeconds(0.8f);

        ani.speed = 0f;

        yield return new WaitForSeconds(0.4f);

        ani.speed = 1f;

        // Wizard의 두번째 카메라를 실행시킨다. ( 번개가 내릴때의 카메라 )
        ActionCamera_Action.Get_Inctance().Set_preparation(transform, "Wizard2");

        // 카메라보다 한박자 늦게 Target을 실행시킨다.
        yield return new WaitForSeconds(0.3f);

        // Target을 활성화하고 대기상태로 만든후 스페셜 공격 Effect를 만든다.
        Target.gameObject.SetActive(true);
        Target.GetComponent<MonsterAction>().Set_Idle();
        Set_SpecialSkill_Effect();

        yield return new WaitForSeconds(0.8f);

        ani.SetTrigger("Idle");

        // ActionCamera의 Camera를 끈다.
        ActionCamera_Action.Get_Inctance().CameraOff();

        // Wizard를 제외한 모든 Player, Monster의 Active를 true시킨다.
        PlayerManager.Get_Inctance().Players_Active(null, "ON");
        MonsterManager.Get_Inctance().Monsters_Active(null, "ON");

        // Target이 Idle상태가 됬으니 다시 Attack을 실행하게 한다.
        Target.GetComponent<MonsterAction>().StartSet_Attack();

        // Player들을 Attack상태로 바꾼다. ( Active 변환 때문.)
        PlayerManager.Get_Inctance().Set_Attack();

        // 스킬이 내리고 한박자 늦게 damage를 준다.
        yield return new WaitForSeconds(0.5f);

        Target.Set_Demage(Attack * 10.0f, "Skill");
    }

    // 스페셜 공격의 Effect를 실행시키는 함수.
    public void Set_SpecialSkill_Effect()
    {
        GameObject Effect = Instantiate(SpecialSkill_Effect, Vector3.zero, Quaternion.identity) as GameObject;

        Vector3 pos = Target.transform.position;
        pos.y += 5f;
        Effect.transform.position = pos;

    }

    public override void Set_Poison()
    {
        StartCoroutine(C_Poison());
        UIManager.Get_Inctance().Set_PlayerState(transform.parent.name, "Poison", 5f);
    }
    IEnumerator C_Poison()
    {
        for (int i = 0; i < 5; i++)
        {
            Set_Demage(1f, null);

            yield return new WaitForSeconds(1f);
        }

        yield break;
    }

    float Distance(Vector3 Target, Vector3 Player)
    {
        return Mathf.Abs(Vector3.Distance(Target, Player));
    }

}
