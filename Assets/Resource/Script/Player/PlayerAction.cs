using UnityEngine;
using System.Collections;

public class PlayerAction : MonoBehaviour
{
    public enum STATE
    {
        IDLE,                           // 대기
        ATTACK,                     // 공격
        DAMAGE,                   // 공격받음
        SKILL,                          // 스킬중
        DEAD,                        // 사망
        MAX,
    }

    public STATE state = STATE.IDLE;                              // Player의 상태

    public  float InitHP = 100f;                                // Hp의 초기값 
    public float InitAttack = 2f;                              // Attack의 초기값
    public float InitSkillPoint = 20f;                           // 스킬 포인트의 초기값
    public float Hp = 10f;                                                    // Hp
    public float Attack = 2f;                                                  // 공격력
    public float SkillPoint = 0f;                                             // 스킬 포인트

    public MonsterAction Target = null;                              // Player가 공격할 Monster의 스크립트.

    protected Vector3 StandPos = Vector3.zero;                // 초기 Position값

    public Animator ani = null;

    void Awake()
    {
        ani = GetComponent<Animator>();
        Hp = InitHP;
        Attack = InitAttack;
        SkillPoint = 0f;
    }

    void Update()
    {
        SkillPoint += Time.deltaTime * 3f;

        if(SkillPoint >= InitSkillPoint) { SkillPoint = InitSkillPoint; }
    }

    // Player의 상태를 변환하는 함수.
    public virtual void Set_Move()
    {
    }
    public virtual void Set_Attack()
    {
    }
    public virtual void Set_Idle()
    {
    }
    public virtual void Set_Dead()
    {

    }

    // 데미지를 입을때 실행되는 함수
    public virtual bool Set_Demage(float AttackDamage, string type)
    {
        return false;
    }

    // Player를 클릭 or Touch 했을때 실행되는 함수.
    public virtual void Touch_Skill()
    {
    }
    // target쪽으로 이동하는 함수.
    public virtual void Target_Move(Vector3 target)
    {
    }
    // Player에 해당하는 버튼을 Touch 했을때 실행되는 함수.
    public virtual void Special_Skill()
    {

    }

    public virtual void Set_Poison()
    {
    }


    // HP를 반환하는 함수.
    public float Get_HP()
    {
        return Hp;
    }

    public void Off_Active() { gameObject.SetActive(false); }
    public bool Check_Dead()
    {
        if (state == STATE.DEAD) { return true; }

        return false;

    }

}
