using UnityEngine;
using System.Collections;

// 힐러인 Nurse의 스크립트.
// 원거리 번개공격을 하고 Surgeon을 터치하면 Player중 피가 가장 낮은 한명을 힐해준다.
public class NurseAction : PlayerAction {

    float Heal_Value = 10f;                                          // 힐의 회복값

    public GameObject Attack_Prefab = null;             // Attack Effect Prefab
    GameObject AttackEffect_obj = null;                             // Attack Effect obj
    public GameObject TouchAttack_Effect = null;    //  Touch Attack Effect obj
    public GameObject Attack_Pos = null;                 // Attack Position obj

    void Awake()
    {
        ani = GetComponent<Animator>();
        PlayerSkill_Manager.Get_Inctance().Set_Skill(this, transform.parent.name);
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
    public override void Touch_Skill()
    {
        ani.SetTrigger("TouchSkill");
        ani.SetTrigger("Idle");
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

    public void Set_TouchSkill()
    {
        GameObject Effect = Instantiate(TouchAttack_Effect) as GameObject;

        // Player중 가장 체력이 낮은 Player를 회복하고 heal_target에 저장해놓는다.
        // heal_target에게 heal Effect를 주기 위함.
        GameObject heal_target = PlayerManager.Get_Inctance().Set_PlayerHeal(Heal_Value);
        Effect.transform.parent = heal_target.transform;
        Effect.transform.localPosition = Vector3.zero;
    }

    public void Start_MagicAttack()
    {
        StartCoroutine(C_MagicAttack());
    }
    IEnumerator C_MagicAttack()
    {
        Vector3 Pos = Vector3.zero;

        // Target이 null이거나 죽어있으면 새로운 Target을 받는다.
        if (Target == null || Target.state.ToString().Equals("DEAD"))
        {
            MonsterManager.Get_Inctance().Set_ReTarget(this);
        }

       // AttackEffect obj가 활성화되있지 않으면 (첫공격) AttackEffect를 생성한다.
        if (AttackEffect_obj == null)
        {
            AttackEffect_obj = Instantiate(Attack_Prefab).gameObject;
            AttackEffect_obj.SetActive(false);
            AttackEffect_obj.name = "NurseAttackAction";

            AttackEffect_obj.transform.parent = null;
        }

        AttackEffect_obj.SetActive(false);

        // Player와 AttackEffect를 Target이 있는곳으로 바라보게 만든다.
        Vector3 target = Target.transform.position;
        target.y = transform.position.y;
        Vector3 v = target - transform.position;

        transform.rotation = Quaternion.LookRotation(v);
        AttackEffect_obj.transform.rotation = Quaternion.LookRotation(v);
        
        // Effect를 Taget이 있는 장소에 둔다.
        AttackEffect_obj.transform.position = Target.transform.position;
        // AttackEffect가 Active된 순간 Effect안에 있는 스크립트가 작동한다.
        AttackEffect_obj.SetActive(true);

        // Target에게 Demage를 준다.
        Target.GetComponent<MonsterAction>().Set_Demage(Attack, null);

        yield break;
    }

    public override void Special_Skill()
    {
        // Player가 Idle상태이거나 Move상태이면 버튼을 눌러도 함수가 실행되지 않는다.
        //if (PlayerManager.Get_Inctance().state.ToString().Equals("IDLE") || PlayerManager.Get_Inctance().state.ToString().Equals("MOVE"))
        //    return;

        //Target이 null이거나 죽어있으면 실행하지 않는다.
        //if (Target == null || Target.gameObject.activeSelf == false)
        //    return;

        StartCoroutine(C_Special_Skill());
    }
    IEnumerator C_Special_Skill()
    {
        if (SkillPoint != InitSkillPoint) { yield break; }

        // Target이 죽었을시 새로운 Target을 받는다.
        if (Target.gameObject.activeSelf == false)
            MonsterManager.Get_Inctance().Set_ReTarget(this);

        // ActionCamera에게 Nurse의 애니메이션을 실행시키게 한다.
        ActionCamera_Action.Get_Inctance().Set_preparation(transform, "Nurse");

        // Nurse를 제외한 모든 Player, Monster의 Active를 false시킨다.
        PlayerManager.Get_Inctance().Players_Active(gameObject, "OFF");
        MonsterManager.Get_Inctance().Monsters_Active(null, "OFF");

        yield return new WaitForSeconds(0.3f);

        ani.SetTrigger("SpecialSkill");

        yield return new WaitForSeconds(0.8f);

        ani.SetTrigger("Idle");

        // ActionCamera의 Camera를 끈다.
        ActionCamera_Action.Get_Inctance().CameraOff();

        // Nurse를 제외한 모든 Player, Monster의 Active를 true시킨다.
        PlayerManager.Get_Inctance().Players_Active(null, "ON");
        MonsterManager.Get_Inctance().Monsters_Active(null, "ON");

        // Target에게 3초동안 Charm 상태로 바꾼다.
        Target.Set_Charm(3f);

        // Player들을 Attack상태로 바꾼다. ( Active 변환 때문.)
        PlayerManager.Get_Inctance().Set_Attack();
        MonsterManager.Get_Inctance().Set_ReAttack(Target.gameObject);

        SkillPoint = 0f;
        yield break;
    }

    public override void Set_Poison()
    {
        StartCoroutine(C_Poison());
        UIManager.Get_Inctance().Set_PlayerState(transform.parent.name, "Poison", 5f);
    }
    IEnumerator C_Poison()
    {
        for(int i = 0; i < 5; i ++)
        {
            Set_Demage(1f, null);

            yield return new WaitForSeconds(1f);
        }

        yield break;
    }

}
