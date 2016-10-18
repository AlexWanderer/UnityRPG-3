using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// 마법사인 Wizard의 스크립트.
public class WizardAction : PlayerAction
{

    public GameObject Attack_Prefab = null;                                                                     // Attack Effect prefab
    GameObject AttackOBJ = null;                                                                                     // Attack Effect obj
    public List<GameObject> AttackEffectList = new List<GameObject>();                     // Attack Effect가 담길 List.
    public GameObject TouchAttack_Effect = null;                                                            // TouchAttack Effect obj
    public GameObject SpecialSkill_Effect = null;                                                              // SpecialAttack Effect obj
    public GameObject Attack_Pos = null;                                                                        // Effect 좌표

    void OnEnable()
    {
        AttackEffectList.Clear();
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
        // 활동중이지 않은 Effect를 체크해 AttackObj에 넣는다.
        for (int i = 0; i < AttackEffectList.Count; i++)
        {
            if (AttackEffectList[i].GetComponent<EffectAction>().active == false)
            {
                AttackOBJ = AttackEffectList[i];
                break;
            }

            // Effect가 모두 활동중이면 새로 하나 만들고 마지막Effect를 다시 체크한다.
            if (i == AttackEffectList.Count - 1)
            {
                Create_AttackEffect();
                i--;
            }
        }


        // Player와 AttackEffect를 Target이 있는곳으로 바라보게 만든다.
        Vector3 target = Target.transform.position;
        target.y = transform.position.y;
        Vector3 v = target - transform.position;
        transform.rotation = Quaternion.LookRotation(v);
        AttackOBJ.transform.rotation = Quaternion.LookRotation(v);

        // Effect를 공격좌표가 있는곳에 둔다.
        AttackOBJ.transform.position = Attack_Pos.transform.position;
        // AttackEffect가 Active된 순간 Effect안에 있는 스크립트가 작동한다.
        AttackOBJ.SetActive(true);

        // Effect가 Target에게 도달할때까지 ani를 0.2배속으로 만들고 timer를 만든다.
        ani.speed = 0.2f;
        float timer = 0f;
        
        // Effect가 Target에게 도달할때까지 실행된다.
        while(true)
        {
            timer += Time.deltaTime; 

            // Effect와 Target의 거리가 0.5f 미만이면 ani를 원래 상태로 돌리고 while문을 벗어난다.
            if(Distance(AttackOBJ.transform.position, Target.transform.position) < 0.5f )
            {
                ani.speed = 1f;
                break;
            }

            // 만약 timer가 5f를 넘었을경우 (안맞고 계속 직진)  ani를 원래 상태로 돌리고 Effect를 비활성화한 후 코루틴을 종료한다.
            if (timer > 5f)
            {
                ani.speed = 1f;
                AttackOBJ.SetActive(false);
                yield break;
            }

            yield return null;

        }

        // while문을 빠져나오면 Effect가 Target에 도달한것이니 Effect를 비활성화시키고 Target에게 데미지를 준다.
        AttackOBJ.GetComponent<EffectAction>().Disenable();
        Target.Set_Demage(Attack, null);
        AttackOBJ.transform.position = Attack_Pos.transform.position;

        yield break;
    }
    // Attack Effect를 만드는 함수.
    void Create_AttackEffect()
    {
        GameObject obj = Instantiate(Attack_Prefab).gameObject;
        obj.AddComponent<EffectAction>();
        obj.SetActive(false);
        obj.name = "WizardAttack";
        obj.transform.parent = null;
        AttackEffectList.Add(obj);
    }

    public void Set_TouchSkill_Effect()
    {
        Vector3 target = Target.transform.position;
        target.y = transform.position.y;
        Vector3 v = target - transform.position;

        GameObject Effect = Instantiate(TouchAttack_Effect, Vector3.zero, Quaternion.LookRotation(v)) as GameObject;
        Vector3 pos = Attack_Pos.transform.position;
        pos.y = 0.15f;
        Effect.transform.position = pos;
        Target.Set_Demage(10f, null);

        ani.SetTrigger("Idle");

    }

    // 왼쪽의 스페셜스킬버튼을 눌렀을때 실행되는 함수.
    public override void Special_Skill()
    {
        // 만약 Player들이 ATTACK상태가 아니면 스킬이 작동되지 않는다.
        if (PlayerManager.Get_Inctance().state.ToString().Equals("ATTACk") == false) { return; }

        // Target이 null이거나 죽었을시 스킬이 작동되지 않는다.
        if (Target == null || Target.Check_Dead()) { return; }

        StartCoroutine(C_Special_Skill());
    }
    IEnumerator C_Special_Skill()
    {
       if(SkillPoint != InitSkillPoint) { yield break; }

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
        Target.GetComponent<MonsterAction>().Set_AniIdle();

        yield return new WaitForSeconds(0.4f);

        Set_SpecialSkill_Effect();
        ani.SetTrigger("Idle");

        yield return new WaitForSeconds(0.5f);

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
        MonsterManager.Get_Inctance().Set_ReAttack();
        SkillPoint = 0;
    }
    // 스페셜 공격의 Effect를 실행시키는 함수.
    public void Set_SpecialSkill_Effect()
    {
        GameObject Effect = Instantiate(SpecialSkill_Effect, Vector3.zero, Quaternion.identity) as GameObject;

        Vector3 pos = Target.transform.position;
        pos.y += 5f;
        Effect.transform.position = pos;

    }


    float Distance(Vector3 Target, Vector3 Player)
    {
        return Mathf.Abs(Vector3.Distance(Target, Player));
    }
}


