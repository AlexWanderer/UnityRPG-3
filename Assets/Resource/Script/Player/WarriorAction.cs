using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 전사인 Warrior의 스크립트.
// 1:1근접공격을 하고 Warrior를 터치하면 도끼를 크게 휘두르는 스킬을 쓴다.
public class WarriorAction : PlayerAction
{
    public GameObject TouchAttack_Effect_Prefab = null;                                                             // 터치 공격 Effect Prefab
    List<GameObject> TouchAttackEffectList = new List<GameObject>();                                   // TouchAttack Effect가 담길 List
    public GameObject TouchAttack_Pos = null;                                                                            // 터치 공격시 Effect의 위치
    public GameObject SpecialSkill_Effect_Prefab = null;                                                               // 스페셜 스킬 Effect Prefab

    public bool check_TargetApproach = false;                                                                              // Target과 가까이 있는지 아닌지 체크하는 변수.

    // Attack을 담당하는 Coroutine을 실행하는 함수.
    public override void Set_AniAttack()
    {
        state = STATE.ATTACK;
        check_TargetApproach = false;
        ani.SetBool("Move", false);
        ani.SetBool("Attack", true);

        StartCoroutine(C_Attack());
    }
  
    
    
    //일반 공격을 하는 코루틴.
    IEnumerator C_Attack()
    {
        while (true)
        {
            // 상태가 Attack이 아닌경우 코루틴을 종료한다.
            if (state != STATE.ATTACK)
            {
                check_TargetApproach = false;
                yield break;
            }

            // Target이 null이거나 죽어있는 MonsterManager에게 새로운 Target을 받아온다.
            if (Target == null || Target.Check_Dead())
            {
                check_TargetApproach = false;
                MonsterManager.Get_Inctance().Set_ReTarget(this);
            }

            // Target이 있는 쪽을 바라본다.
            Vector3 target = Target.transform.position;
            target.y = transform.position.y;
            Vector3 v = target - transform.position;
            transform.rotation = Quaternion.LookRotation(v);

            // Target과 붙어있지 않다면 Target쪽으로 계속 움직인다.
            if (check_TargetApproach == false)
            {
                // 스킬중엔 움직이지 않는다.
                if (state == STATE.SKILL || Target.Check_Dead()) { yield return null; }

                Set_AniMove();
                transform.Translate(Vector3.forward * Time.deltaTime * Speed);
                yield return null;

            }
            // Target과 붙어있다면 공격한다.
            else
            {
                ani.SetBool("Attack", true);
            }

            yield return null;

        }
    }
    // Target을 공격하는 함수. Attack Ani에서 호출한다.
    public void Monster_Attack()
    {
       bool check =  Target.Set_Demage(BaseAttack, null);

        // 몬스터가 다 죽었을지 AttackCoroutine을 종료한다.
        if(check)
        {
            StopCoroutine(C_Attack());
        }
    }



    // TouchSkill Effect를 만드는 함수. Ani TouchSkill에서 호출. 
    // Damage는 폭발Effect안에있는 Warrior_TouchAttack_Action에서 준다.
    public void Set_TouchSkill_Effect()
    {      
        // 비활동중인 TouchSkill Effect를 담는 변수.
        GameObject Effect = null;

        // AttackEffect가 하나도 만들어져있지 않으면 Effect OBJ를 하나 만든다.
        if (TouchAttackEffectList.Count == 0)
        {
            Create_TouchAttackEffect();
        }
        // 활동중이지 않은 Effect를 체크해 변수에 넣는다.
        for (int i = 0; i < TouchAttackEffectList.Count; i++)
        {
            if (TouchAttackEffectList[i].GetComponent<Warrior_TouchAttackEffect_Action>().active == false)
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
        Effect.GetComponent<Warrior_TouchAttackEffect_Action>().Check_Alive();
    }
    //TouchAttackEffect를 만드는 함수
    void Create_TouchAttackEffect()
    {
        GameObject Effect = Instantiate(TouchAttack_Effect_Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        Effect.AddComponent<Warrior_TouchAttackEffect_Action>();
        //Effect의 Damage 수치를 넣어준다.
        Effect.GetComponent<Warrior_TouchAttackEffect_Action>().Damage = InitAttack * 1.5f;

        Effect.transform.position = TouchAttack_Pos.transform.position;
        TouchAttackEffectList.Add(Effect);
    }



    // 왼쪽의 스페셜스킬버튼을 눌렀을때 실행되는 함수.
    public override void Special_Skill()
    {
        // 만약 Player가 ATTACK상태가 아니면 스킬이 작동되지 않는다.
        if (state != STATE.ATTACK) { return; }

        // SkillPoint가 다 차지않았으면 함수를 종료한다.
        if (SkillPoint != InitSkillPoint) { return; }

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

        // ActionCamera에게 Warrior의 애니메이션을 실행시키게 한다.
        ActionCamera_Action.Get_Inctance().Set_preparation(transform, "Warrior");

        // Warrior을 제외한 모든 Player, Monster의 Active를 false시킨다.
        PlayerManager.Get_Inctance().Players_Active(gameObject, "OFF");
        MonsterManager.Get_Inctance().Monsters_Active(null, "OFF");

        yield return new WaitForSeconds(0.25f);

        ani.SetTrigger("SpecialSkill");

        yield return new WaitForSeconds(0.35f);

        ani.speed = 0f;

        yield return new WaitForSeconds(0.8f);

        ani.speed = 1f;
        ani.SetTrigger("Idle");

        // ActionCamera의 Camera를 끈다.
        ActionCamera_Action.Get_Inctance().CameraOff();

        // Warrior을 제외한 모든 Player, Monster의 Active를 true시킨다.
        PlayerManager.Get_Inctance().Players_Active(null, "ON");
        MonsterManager.Get_Inctance().Monsters_Active(null, "ON");

        // Player의 회전값을 원래대로 되돌린다.
        transform.rotation = InitRotation;

        // Target에게 데미지를 준다.
        Target.Set_Demage(BaseAttack * 10.0f, "Skill");

        // Player들을 Attack상태로 바꾼다. ( Active 변환 때문.)
        PlayerManager.Get_Inctance().Set_Attack();
        MonsterManager.Get_Inctance().Set_ReAttack();

        //SkillPoint를 초기화한다.
        SkillPoint = 0f;


        yield break;
    }
    // 스페셜 스킬의 Effect를 실행시킨다.
    public void Set_SpecialSkill_Effect()
    {
        GameObject Effect = Instantiate(SpecialSkill_Effect_Prefab, Vector3.zero, Quaternion.identity) as GameObject;

        Vector3 pos = TouchAttack_Pos.transform.position;
        pos.y = 1f;

        Effect.transform.position = pos;
    }



    // A와 B사이의 거리를 반환하는 함수. ( 음수값이 없다. )
    float Distance(Vector3 Target, Vector3 Player)
    {
        return Mathf.Abs(Vector3.Distance(Target, Player));
    }



    void OnCollisionEnter(Collision obj)
    {
        // 몬스터와 충돌하면 Target을 충돌한 몬스터로 바꾼다.
        if(obj.gameObject.CompareTag("Monster"))
        {
            Target = obj.gameObject.GetComponent<MonsterAction>();
            check_TargetApproach = true;
        }
    }
}
