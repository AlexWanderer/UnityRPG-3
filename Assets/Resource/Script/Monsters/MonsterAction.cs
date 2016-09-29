using UnityEngine;
using System.Collections;

// 몬스터들의 행동이 담겨있는 스크립트
public class MonsterAction : MonoBehaviour {

    public enum STATE
    {
        IDLE,                               // 대기
        ATTACK,                         // 공격
        CHARM,                          // 매료
        DEAD,                             // 죽음
        MAX,
    };
    public enum TYPE
    {
        BASE,
        BOSS,
    };



    protected const float initHP = 10f;                        // MAX HP ( 정적 )
    const float initAttack = 1f;                                    // MAX ATTACK ( 정적 )
    public float Hp = 10f;                                           // 현재 HP
    public float Attack = 1f;                                       // 현재 ATTACK
        
    public STATE state = STATE.IDLE;                        // 상태
    public TYPE type = TYPE.BASE;

   protected GameObject Condition = null;            // 상태 Effect가 들어갈 Obj
   public GameObject Target = null;                    // 공격할 대상
    public Animator ani = null;

    void Awake()
    {
        Condition = transform.FindChild("Condition").gameObject;
        ani = GetComponent<Animator>();
    }
   
    // 모든 Coroutine을 멈추고 state를 IDLE 상태로 변환하는 함수.
    public virtual void Set_Idle()
    {
    }
    public virtual void Set_Dead()
    {

    }
    // state를 ATTACK으로 바꾸고 PlayerManager에게 Target을 요구하는 등의 Attack을 준비하는 함수. 
    public virtual void StartSet_Attack()
    {
    }

    // Damage를 입었을때 실행되는 함수. type이 Null이면 일반 데미지 , Skill이면 Special Skill이다. 
    // Monster의 HP가 0이 되면 true 그외는 false를 반환한다.
    public virtual bool Set_Demage(float AttackDamage, string type)
    {
        return false;
    }

    // InitHP를 반환하는 함수.
    public float Get_InitHP()
    {
        return initHP;
    }

    // Charm에 걸렸을때 실행되는 함수. time만큼 지난 후에는 Effect를 끄고 StartSet_Attack()를 실행시킨다.
    public virtual void Set_Charm(float time)
    {
    }

    // Condition의 Effect를 삭제하는 함수.
    public virtual void State_OFF()
    {
    }
    public void Off_Active() { gameObject.SetActive(false); }
}
