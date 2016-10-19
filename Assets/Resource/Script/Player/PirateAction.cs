using UnityEngine;
using System.Collections;

// 해적 Pirate의 소스.
// 중거리에서 대포를 날린다.
public class PirateAction : PlayerAction {

    public GameObject TouchAttack_Effect_Prefab = null;                    // TouchAttack Effect Prefab obj
    public GameObject[] SpecialSkill_NPC = null;                                 // Special Attack을 하는 눈송이 NPC들.
    public GameObject Attack_Pos = null;                                            // Effect 좌표
    float TouchSkill_Time = 5f;



    void OnEnable()
    {
        // 자식인 NPC들을 SpecialSkill_NPC에 저장해놓는다.
        Transform NPCs = transform.FindChild("NPC");
        SpecialSkill_NPC = new GameObject[ NPCs.childCount ];

        for (int i = 0; i < NPCs.childCount; i++)
        {
            SpecialSkill_NPC[i] = NPCs.GetChild(i).gameObject;
            SpecialSkill_NPC[i].SetActive(false);
        }
    }
 


    // Attack을 담당하는 Coroutine을 실행하는 함수.
    public override void Set_AniAttack()
    {
        state = STATE.ATTACK;
        StartCoroutine(C_Attack());
    }
   
    
    
    //일반 공격을 하는 코루틴. Attack 애니에서 호출한다.
    IEnumerator C_Attack()
    {
        while (true)
        {
            // 상태가 Attack이 아닌경우 코루틴을 종료한다.
            if (state != STATE.ATTACK) { yield break; }

            // Target이 null이거나 죽어있는 MonsterManager에게 새로운 Target을 받아온다.
            if (Target == null || Target.Check_Dead())
            {
                MonsterManager.Get_Inctance().Set_ReTarget(this);
            }

            // Target이 있는 쪽을 바라본다.
            Vector3 target = Target.transform.position;
            target.y = transform.position.y;
            Vector3 v = target - transform.position;
            transform.rotation = Quaternion.LookRotation(v);

            // Target과 거리가 4f이상이면 Target쪽으로 움직인다.
            if (Distance(Target.transform.position, transform.position) > 5f)
            {
                // 스페셜 스킬중에는 움직이지 않도록 한다.
                if (state == STATE.SKILL) { yield return null; }

                // Ani를 Move로 변환한다,
                Set_AniMove();

                transform.Translate(Vector3.forward * Time.deltaTime * Speed);
                yield return null;
            }
            // 거리가 10f 미만이면 공격하는 Ani를 호출한다.
            else
            {
                ani.SetBool("Attack", true);
            }

            yield return null;
        }
    }
    // Target을 공격하는 함수. Attack Ani에서 호출한다.
    void Monster_Attack()
    {
        Target.Set_Demage(BaseAttack, null);
    }



    // TouchSkill을 하는 함수. TouchSkill Ani에서 호출한다.
    public void Set_TouchSkill()
    {
        // TochSkill_Time후에 SKillEffect를 끈다.
        Invoke("Stop_TouchSkillEffect", TouchSkill_Time);

        // Player의 HpBar옆에 도발아이콘을 띄우게 한다.
        UIManager.Get_Inctance().Set_PlayerState(transform.parent.name, "Provocation", TouchSkill_Time);
        // 모든 몬스터들의 Target을 자기로 고정시킨다. TouchSkill_Time만큼 지나면 Target은 다시 재조정된다.
        MonsterManager.Get_Inctance().Set_ParticularTarget(gameObject, TouchSkill_Time);
        TouchAttack_Effect_Prefab.SetActive(true);
    }
    // TouchSkillEffect를 끄는 함수.
    void Stop_TouchSkillEffect()
    {
        TouchAttack_Effect_Prefab.SetActive(false);
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

        // ActionCamera에게 Wizard의 애니메이션을 실행시키게 한다.
        ActionCamera_Action.Get_Inctance().Set_preparation(transform, "Pirate");

        // Pirate을 제외한 모든 Player, Monster의 Active를 false시킨다.
        PlayerManager.Get_Inctance().Players_Active(gameObject, "OFF");
        MonsterManager.Get_Inctance().Monsters_Active(null, "OFF");

        yield return new WaitForSeconds(0.25f);

        ani.SetTrigger("SpecialSkill");

        //  NPC들을 활성화시킨후 공격준비를 한다.
        for(int i = 0; i < SpecialSkill_NPC.Length; i++)
        {
            SpecialSkill_NPC[i].SetActive(true);

            yield return new WaitForSeconds(0.2f);

            SpecialSkill_NPC[i].GetComponent<Henchman_Action>().Start_Attack();
        }

        yield return new WaitForSeconds(1f);

        ani.SetTrigger("Idle");

        // ActionCamera의 Camera를 끈다.
        ActionCamera_Action.Get_Inctance().CameraOff();

        // Player을 제외한 모든 Player, Monster의 Active를 true시킨다.
        PlayerManager.Get_Inctance().Players_Active(null, "ON");
        MonsterManager.Get_Inctance().Monsters_Active(null, "ON");

        // Player의 회전값을 원래대로 되돌린다.
        transform.rotation = InitRotation;

        // Player들을 Attack상태로 바꾼다. ( Active 변환 때문.)
        PlayerManager.Get_Inctance().Set_Attack();
        MonsterManager.Get_Inctance().Set_ReAttack();

        // SkillPoint를 초기화한다.
        SkillPoint = 0f;
        yield break;
    }



    // A와 B사이의 거리를 반환하는 함수. ( 음수값이 없다. )
    float Distance(Vector3 Target, Vector3 Player)
    {
        return Mathf.Abs(Vector3.Distance(Target, Player));
    }

}
