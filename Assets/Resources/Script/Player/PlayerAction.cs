using UnityEngine;
using System.Collections;



public class PlayerAction : MonoBehaviour
{
    public enum STATE
    {
        IDLE,                           // 대기
        ATTACK,                     // 공격
        SKILL,                          // 스킬중
        DEAD,                        // 사망
        MAX,
    }

    public STATE state = STATE.IDLE;                              // Player의 상태

   public RecvCharaterInfo Init_Infomation = null;
    public float Hp;                                                // Hp
    public float BaseAttack;                                            // 일반 공격력
    public float SkillPoint;                                       // 스킬 포인트
    public float Speed;
    public int Level;

    public MonsterAction Target = null;                              // Player가 공격할 Monster의 스크립트.

    protected Vector3 StandPos = Vector3.zero;                // 초기 Position값

    public Animator ani = null;

    void Start()
    {
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        SkillPoint += Time.deltaTime * 3f;

        if(SkillPoint >= Init_Infomation.Skillpoint)
        {
            SkillPoint = Init_Infomation.Skillpoint;
        }
    }

    public void Set_Info(RecvCharaterInfo Info)
    {

        Init_Infomation = Info;
        Level = Init_Infomation.Level;
        Speed = Init_Infomation.Speed;


        // HP, Attack, SkillPoint를 초기화한다.
        Hp = Init_Infomation.Hp;
        BaseAttack = Init_Infomation.Attack;
        SkillPoint = Init_Infomation.Skillpoint;

        // Skill버튼에 스페셜스킬 함수를 연동한다.
        PlayerSkill_Manager.Get_Inctance().Set_Skill(this, transform.parent.name);

    }

    // Player의 Ani를 Move로 변환하는 함수
    public void Set_AniMove()
    {
        ani.SetBool("Move", true);
        ani.SetBool("Attack", false);
    }

    public virtual void Set_AniAttack() { }
    // 모든 Coroutine을 멈추고 state를 IDLE 상태로 변환하는 함수.
    public virtual void Set_AniIdle()
    {
    }
    // 모든 Coroutine을 멈추고 state를 DEAD 상태로 변환하는 함수.
    public void Set_Dead()
    {
        state = STATE.DEAD;
        StopAllCoroutines();
        PlayerManager.Get_Inctance().Check_Dead();
        ani.SetBool("Dead", true);
    }
    // Damage를 입었을때 실행되는 함수. type이 Null이면 일반 데미지 , Skill이면 Special Skill이다. 
    // Monster의 HP가 0이 되면 true 그외는 false를 반환한다.
    public bool Set_Demage(float AttackDamage, string type)
    {
        Hp -= AttackDamage;
        // UIManager에게 맞은 Damage와 변경된 HP를 표시하게한다.
        UIManager.Get_Inctance().Set_Damage(gameObject, AttackDamage, type);
        UIManager.Get_Inctance().Set_PlayerHp(Hp / Init_Infomation.Hp, transform.parent.name);

        if (Hp <= 0)
        {
            Set_Dead();
            return true;
        }
        return false;
    }

    // Player를 클릭 or Touch 했을때 Ani를 TouchSkill로 바꾸는 함수.
    public void Set_AniTouchSkill()
    {
        ani.SetTrigger("TouchSkill");
        ani.SetTrigger("Idle");
    }
    public virtual void Special_Skill()
    {
    }

    // HP를 반환하는 함수.
    public float Get_HP()
    {
        return Hp;
    }
    // 해당 OBJ의 Active를 False하는 함수.
    public void Off_Active()
    {
        gameObject.SetActive(false);
    }
    // 해당 obj가 죽었는지 아닌지를 체크하는 함수.
    public bool Check_Dead()
    {
        if (state == STATE.DEAD)
        {
            return true;
        }
        return false;
    }

    //                                                                          //
    //               아래 부터는 상태 이상에 관한 함수 
    //                                                                           //


    // 독에 걸렸을때 실행되는 함수. Coroutine C_Poison을 실행시킨다.
    public void Set_StatePoison(float Damage, float time)
    {
        StartCoroutine(C_Poison(Damage, time));
        UIManager.Get_Inctance().Set_PlayerState(transform.parent.name, "Poison", time);
    }
    // Poison에 걸렸을때 돌아가는 함수. time만큼 Damage를 준다. 1초간격.
    IEnumerator C_Poison(float Damage, float time)
    {
        for (int i = 0; i < time; i++)
        {
            Set_Demage(Damage, null);

            yield return new WaitForSeconds(1f);
        }

        yield break;
    }
}
