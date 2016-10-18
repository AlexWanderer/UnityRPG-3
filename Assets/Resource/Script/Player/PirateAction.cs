﻿using UnityEngine;
using System.Collections;

public class PirateAction : PlayerAction {

    public GameObject TouchAttack_Effect = null;                    // TouchAttack Effect obj
    public GameObject[] SpecialSkill_NPC = null;                      
    public GameObject Attack_Pos = null;                                 // Effect 좌표


    void OnEnable()
    {
        Transform NPCs = transform.FindChild("NPC");
        SpecialSkill_NPC = new GameObject[ NPCs.childCount ];

        for (int i = 0; i < NPCs.childCount; i++)
        {
            SpecialSkill_NPC[i] = NPCs.GetChild(i).gameObject;
            SpecialSkill_NPC[i].SetActive(false);
        }
    }

    public override void Set_AniAttack()
    {
        state = STATE.ATTACK;
        ani.SetBool("Attack", true);
        ani.SetBool("Move", false);
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
        if (SkillPoint != InitSkillPoint) { yield break; }

        // Target이 죽었을시 새로운 Target을 받는다.
        if (Target.gameObject.activeSelf == false)
            MonsterManager.Get_Inctance().Set_ReTarget(this);

        // ActionCamera에게 Wizard의 애니메이션을 실행시키게 한다.
        ActionCamera_Action.Get_Inctance().Set_preparation(transform, "Pirate");

        // Pirate을 제외한 모든 Player, Monster의 Active를 false시킨다.
        PlayerManager.Get_Inctance().Players_Active(gameObject, "OFF");
        MonsterManager.Get_Inctance().Monsters_Active(null, "OFF");

        yield return new WaitForSeconds(0.25f);

        ani.SetTrigger("SpecialSkill");

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

        // Warrior을 제외한 모든 Player, Monster의 Active를 true시킨다.
        PlayerManager.Get_Inctance().Players_Active(null, "ON");
        MonsterManager.Get_Inctance().Monsters_Active(null, "ON");

        // Target에게 데미지를 준다.
        Target.Set_Demage(Attack * 10.0f, "Skill");

        // Player들을 Attack상태로 바꾼다. ( Active 변환 때문.)
        PlayerManager.Get_Inctance().Set_Attack();
        MonsterManager.Get_Inctance().Set_ReAttack();

        SkillPoint = 0f;


        yield break;
    }

    public void Target_Move(Vector3 target)
    {
        target.y = transform.position.y;
        Vector3 v = target - transform.position;

        transform.rotation = Quaternion.LookRotation(v);
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
    }
  

    float Distance(Vector3 Target, Vector3 Player)
    {
        return Mathf.Abs(Vector3.Distance(Target, Player));
    }

}
