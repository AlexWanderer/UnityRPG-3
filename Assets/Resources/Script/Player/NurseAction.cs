using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 힐러인 Nurse의 스크립트.
// 원거리 번개공격을 하고 Surgeon을 터치하면 Player중 피가 가장 낮은 한명을 힐해준다.
public class NurseAction : PlayerAction {

    public GameObject Attack_Effect_Prefab = null;                                                 // Attack Effect Prefab
    GameObject Attack_Effect_obj = null;                                                                 // Attack Effect OBJ
    List<GameObject> TouchAttackEffectList = new List<GameObject>();             // TouchAttack Effect가 담길 List
    public GameObject TouchAttack_Effect_Prefab = null;                                      //  Touch Attack Effect obj
    public GameObject Attack_Pos = null;                                                                // Attack Position obj

    // Attack을 담당하는 Coroutine을 실행하는 함수.
    public override void Set_AniAttack()
    {
        state = STATE.ATTACK;
        ani.SetBool("Attack", true);
        ani.SetBool("Move", false);
    }

    // 모든 Coroutine을 멈추고 state를 IDLE 상태로 변환하는 함수.
    public override void Set_AniIdle()
    {
        StopAllCoroutines();
        state = STATE.IDLE;

        ani.SetBool("Attack", false);
        ani.SetBool("Move", false);
        transform.localPosition = StandPos;
        transform.rotation = Quaternion.identity;
    }



    //일반 공격을 하는 코루틴. Attack 애니에서 호출한다.
    IEnumerator C_MagicAttack()
    {
        // Target이 null이거나 죽어있으면 새로운 Target을 받는다.
        if (Target == null || Target.Check_Dead())
        {
            MonsterManager.Get_Inctance().Set_ReTarget(this);
        }

       // AttackEffect obj가 활성화되있지 않으면 (첫공격) AttackEffect를 생성한다.
        if (Attack_Effect_obj == null)
        {
            Attack_Effect_obj = Instantiate(Attack_Effect_Prefab).gameObject;
            Attack_Effect_obj.SetActive(false);
            Attack_Effect_obj.name = "NurseAttackAction";

            Attack_Effect_obj.transform.parent = null;
        }

        Attack_Effect_obj.SetActive(false);

        // Player와 AttackEffect를 Target이 있는곳으로 바라보게 만든다.
        Vector3 target = Target.transform.position;
        target.y = transform.position.y;
        Vector3 v = target - transform.position;
        transform.rotation = Quaternion.LookRotation(v);

        // Effect를 Taget이 있는 장소에 둔다.
        Attack_Effect_obj.transform.position = Target.transform.position;
        // AttackEffect가 Active된 순간 Effect안에 있는 스크립트가 작동한다.
        Attack_Effect_obj.SetActive(true);

        // Target에게 Demage를 준다.
        Target.GetComponent<MonsterAction>().Set_Demage(BaseAttack, null);

        yield break;
    }


    // TouchSkill Effect를 만드는 함수. Ani TouchSkill에서 호출. 
    public void Set_TouchSkill()
    {
        // 비활동중인 TouchSkill Effect를 담는 변수.
        GameObject Effect = null;

        // Target이 null이거나 죽어있으면 새로운 Target을 받는다.
        if (Target == null || Target.Check_Dead())
        {
            MonsterManager.Get_Inctance().Set_ReTarget(this);
        }

        // AttackEffect가 하나도 만들어져있지 않으면 Effect OBJ를 하나 만든다.
        if (TouchAttackEffectList.Count == 0)
        {
            Create_TouchAttackEffect();
        }
        // 활동중이지 않은 Effect를 체크해 변수에 넣는다.
        for (int i = 0; i < TouchAttackEffectList.Count; i++)
        {
            if (TouchAttackEffectList[i].GetComponent<BaseEffectAction>().active == false)
            {
                Effect = TouchAttackEffectList[i];
                break;
            }

            // Effect가 모두 활동중이면 새로 하나 만들고 다시 체크한다.
            if (i == TouchAttackEffectList.Count - 1)
            {
                Create_TouchAttackEffect();
                i = 0;
            }
        }

        // Effect의 Particle이 돌아가는지 체크하는 함수를 실행시킨다. ( 여기서 active변수나 기타등등을 관리한다 ) 
        Effect.GetComponent<BaseEffectAction>().Start_Effect();

        // Player중 가장 체력이 낮은 Player를 회복하고 heal_target에 저장해놓는다.
        // heal_target에게 heal Effect를 주기 위함.
        GameObject heal_target = PlayerManager.Get_Inctance().Set_PlayerHeal(Init_Infomation.Touch_value);

        // Effect의 부모를 heal을 받는 Target으로 옮기고 Position을 초기화한다.
        Effect.transform.parent = heal_target.transform;
        Effect.transform.position = heal_target.transform.position;
    }
    //TouchAttackEffect를 만드는 함수
    void Create_TouchAttackEffect()
    {
        GameObject Effect = Instantiate(TouchAttack_Effect_Prefab) as GameObject;
        Effect.AddComponent<BaseEffectAction>();

        TouchAttackEffectList.Add(Effect);
    }
    
    
    
    // 왼쪽의 스페셜스킬버튼을 눌렀을때 실행되는 함수.
    public override void Special_Skill()
    {
        // 만약 Player들이 ATTACK상태가 아니면 스킬이 작동되지 않는다.
        if (state != STATE.ATTACK) { return; }

        // SkillPoint가 다 차지않았으면 함수를 종료한다.
        if (SkillPoint != Init_Infomation.Skillpoint) { return; }

        // Target이 null이거나 죽었을시 스킬이 작동되지 않는다.
        if (Target == null || Target.Check_Dead()) { return; }

        state = STATE.SKILL;
        StartCoroutine(C_Special_Skill());
    }
    // Specal Skill이 작동하는 함수.
    IEnumerator C_Special_Skill()
    {
        // Target이 죽었을시 새로운 Target을 받는다.
        if (Target.Check_Dead())
        {
            MonsterManager.Get_Inctance().Set_ReTarget(this);
        }

        //현재 Player의 회전값을 저장한후 회전값을 초기화한다.
        Quaternion InitRotation = gameObject.transform.rotation;
        transform.rotation = Quaternion.identity;

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

        // Player의 회전값을 원래대로 되돌린다.
        transform.rotation = InitRotation;

        // Player들을 Attack상태로 바꾼다. ( Active 변환 때문.)
        PlayerManager.Get_Inctance().Set_Attack();
        MonsterManager.Get_Inctance().Set_ReAttack(Target.gameObject);

        // Target에게 3초동안 Charm 상태로 바꾼다.
        Target.Set_StateCharm(Init_Infomation.Special_time);

        SkillPoint = 0f;
        yield break;
    }



}
