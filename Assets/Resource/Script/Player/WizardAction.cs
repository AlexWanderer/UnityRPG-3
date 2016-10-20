using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// 마법사인 Wizard의 스크립트.
// 장거리 마법공격을 날린다.
public class WizardAction : PlayerAction
{

    public GameObject Attack_Prefab = null;                                                                                 // Attack Effect prefab
    public List<GameObject> AttackEffectList = new List<GameObject>();                                 // Attack Effect가 담길 List.
    public GameObject TouchAttack_Effect_Prefab = null;                                                            // TouchAttack Effect Prefab obj
    public List<GameObject> TouchAttackEffectList = new List<GameObject>();                       // TouchAttack Effect가 담길 List
    public GameObject SpecialSkill_Effect_Prefab = null;                                                              // SpecialAttack Effect Prefab obj
    public GameObject Attack_Pos = null;                                                                                     // Effect 좌표



    void OnEnable()
    {
        //Effect List들을 초기화한다.
        AttackEffectList.Clear();
        TouchAttackEffectList.Clear();
    }



    // Ani를 Attack 상태로  변환하는 함수.
    public override void Set_AniAttack()
    {
        state = STATE.ATTACK;
        ani.SetBool("Attack", true);
        ani.SetBool("Move", false);
    }
   
    

    //일반 공격을 하는 코루틴. Attack 애니에서 호출한다.
    IEnumerator C_MagicAttack()
    {
        // 비활동중인 Attack Effect가 담기는 변수
        GameObject Effect = null;

        if (state != STATE.ATTACK) { yield break; }

        // Target이 null이거나 죽어있으면 새로운 Target을 받는다.
        if (Target == null || Target.Check_Dead())
        {
            MonsterManager.Get_Inctance().Set_ReTarget(this);
        }

        // AttackEffect가 하나도 만들어져있지 않으면 Effect OBJ를 하나 만든다.
        if (AttackEffectList.Count == 0)
        {
            Create_AttackEffect();
        }
        // 활동중이지 않은 Effect를 체크해 변수에 넣는다.
        for (int i = 0; i < AttackEffectList.Count; i++)
        {
            if (AttackEffectList[i].GetComponent<Wizard_AttackEffectAction>().active == false)
            {
                Effect = AttackEffectList[i];
                break;
            }

            // Effect가 모두 활동중이면 새로 하나 만들고 다시 체크한다.
            if (i == AttackEffectList.Count - 1)
            {
                Create_AttackEffect();
                i = 0;
                yield return null;
            }
        }


        // Player와 AttackEffect를 Target이 있는곳으로 바라보게 만든다.
        Vector3 target = Target.transform.position;
        target.y = transform.position.y;
        Vector3 v = target - transform.position;
        transform.rotation = Quaternion.LookRotation(v);
        Effect.transform.rotation = Quaternion.LookRotation(v);

        // Effect를 공격좌표가 있는곳에 둔다.
        Effect.transform.position = Attack_Pos.transform.position;
        // AttackEffect가 Active된 순간 Effect안에 있는 스크립트가 작동한다.
        Effect.SetActive(true);

        // Effect가 Target에게 도달할때까지 ani를 0.2배속으로 만들고 timer를 만든다.
        ani.speed = 0.2f;
        float timer = 0f;
        
        // Effect가 Target에게 도달할때까지 실행된다.
        while(true)
        {
            timer += Time.deltaTime; 

            // Effect와 Target의 거리가 0.5f 미만이면 ani를 원래 상태로 돌리고 while문을 벗어난다.
            // 날린 Attack이 몬스터에 맞을때까지 기다린후 공격하는 구조이다.
            if(Vector3.Distance(Effect.transform.position, Target.transform.position) < 0.5f )
            {
                ani.speed = 1f;
                break;
            }

            // 만약 timer가 5f를 넘었을경우 (안맞고 계속 직진)  ani를 원래 상태로 돌리고 Effect를 비활성화한 후 코루틴을 종료한다.
            if (timer > 5f)
            {
                ani.speed = 1f;
                Effect.SetActive(false);
                yield break;
            }

            yield return null;

        }

        // while문을 빠져나오면 Effect가 Target에 도달한것이니 Effect를 비활성화시키고 Target에게 데미지를 준다.
        Effect.GetComponent<Wizard_AttackEffectAction>().Disenable();
        Target.Set_Demage(BaseAttack, null);
        Effect.transform.position = Attack_Pos.transform.position;

        yield break;
    }
    // Attack Effect를 만드는 함수.
    void Create_AttackEffect()
    {
        GameObject obj = Instantiate(Attack_Prefab).gameObject;
        obj.AddComponent<Wizard_AttackEffectAction>();
        obj.SetActive(false);
        obj.name = "WizardAttack";
        obj.transform.parent = null;
        AttackEffectList.Add(obj);
    }



    // TouchSkill Effect를 만드는 함수. Ani TouchSkill에서 호출. 
    public void Set_TouchSkill_Effect()
    {
        if (state != STATE.ATTACK) { return; }

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
            if (i == AttackEffectList.Count - 1)
            {
                Create_TouchAttackEffect();
                i = 0;
            }
        }

        // Effect의 Particle이 돌아가는지 체크하는 함수를 실행시킨다. ( 여기서 active변수나 기타등등을 관리한다 ) 
        Effect.GetComponent<BaseEffectAction>().Start_Effect();
        Target.Set_Demage(BaseAttack * 1.5f, null);
        ani.SetTrigger("Idle");
    }
    //TouchAttackEffect를 만드는 함수
    void Create_TouchAttackEffect()
    {
        GameObject Effect = Instantiate(TouchAttack_Effect_Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        Effect.AddComponent<BaseEffectAction>();

        Vector3 pos = Attack_Pos.transform.position;
        pos.y = 0.15f;
        Effect.transform.position = pos;
        TouchAttackEffectList.Add(Effect);
    }



    // 왼쪽의 스페셜스킬버튼을 눌렀을때 실행되는 함수.
    public override void Special_Skill()
    {
        // 만약 Player가 ATTACK상태가 아니면 스킬이 작동되지 않는다.
        if (state  != STATE.ATTACK) { return; }

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
        ani.speed = 0.8f;

      yield return new WaitForSeconds(0.6f);
        // 스페셜 공격 Effect를 Target에 만든다.
        Set_SpecialSkill_Effect();
        ani.SetTrigger("Idle");
        // Target을 활성화하고 대기상태로 만든다.
        Target.gameObject.SetActive(true);
        Target.GetComponent<MonsterAction>().Set_AniIdle();

        // 스피드를 원래대로 되돌린다.
        ani.speed = 1f;

        yield return new WaitForSeconds(0.7f);
        
        // ActionCamera의 Camera를 끈다.
        ActionCamera_Action.Get_Inctance().CameraOff();

        // Wizard를 제외한 모든 Player, Monster의 Active를 true시킨다.
        PlayerManager.Get_Inctance().Players_Active(null, "ON");
        MonsterManager.Get_Inctance().Monsters_Active(null, "ON");

        // Player의 회전값을 원래대로 되돌린다.
        transform.rotation = InitRotation;

        // Target이 Idle상태가 됬으니 다시 Attack을 실행하게 한다.
        Target.GetComponent<MonsterAction>().StartSet_Attack();

        // 모두를 Attack상태로 바꾼다. ( Active 변환 때문.)
        PlayerManager.Get_Inctance().Set_Attack();
        MonsterManager.Get_Inctance().Set_ReAttack();

        // Target에게 데미지를 준다.
        Target.Set_Demage(BaseAttack * 10.0f, "Skill");
        // SkillPoint를 초기화한다.
        SkillPoint = 0;
    }
    // 스페셜 공격의 Effect를 실행시키는 함수.
    public void Set_SpecialSkill_Effect()
    {
        GameObject Effect = Instantiate(SpecialSkill_Effect_Prefab, Vector3.zero, Quaternion.identity) as GameObject;

        Vector3 pos = Target.transform.position;
        pos.y += 5.5f;
        Effect.transform.position = pos;

    }
}


