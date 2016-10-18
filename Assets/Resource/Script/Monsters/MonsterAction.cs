using UnityEngine;
using System.Collections;

// 몬스터들의 행동이 담겨있는 스크립트
public class MonsterAction : MonoBehaviour {

    public enum STATE
    {
        IDLE,                               // 대기
        ATTACK,                         // 공격
        PROVOCATION,            // 도발
        CHARM,                          // 매료
        DEAD,                             // 죽음
        MAX,
    };
    public enum TYPE
    {
        BASE,                              // 일반 몬스터
        BOSS,                             // 보스 몬스터
    };



    public float InitHP = 500f;                                          // MAX HP ( 정적 )
    public float initAttack = 1f;                                        // MAX ATTACK ( 정적 )
    public float Hp = 500f;                                               // 현재 HP
    public float Attack = 1f;                                             // 현재 ATTACK
    public float Speed = 1f;                                             // Speed
        
    public STATE state = STATE.IDLE;                             // 걸린 상태 OR 상태이상
    public TYPE type = TYPE.BASE;                                 // 몬스터의 타입

   protected GameObject Condition = null;                  // 상태이상 Effect가 들어갈 Obj
   public PlayerAction Target = null;                             // 공격할 대상
    public Animator ani = null;

    void Awake()
    {
        ani = GetComponent<Animator>();
        Hp = InitHP;
    }

    public virtual void StartSet_Attack()
    {
    }
   
    // 모든 Coroutine을 멈추고 state를 IDLE 상태로 변환하는 함수.
    public void Set_AniIdle()
    {
        if(Check_Dead()) { return; }

        StopAllCoroutines();
        state = STATE.IDLE;
        ani.SetTrigger("Idle");
    }
    // 모든 Coroutine을 멈추고 state를 DEAD 상태로 변환하는 함수.
    public void Set_AniDead()
    {
        state = STATE.DEAD;
        StopAllCoroutines();
        // MonsterManager에게 자신이 죽었다는 사실을 알린다.
        MonsterManager.Get_Inctance().Check_Dead(gameObject);
        ani.SetBool("Dead", true);

    }


    // Damage를 입었을때 실행되는 함수. type이 Null이면 일반 데미지 , Skill이면 Special Skill이다. 
    // Monster의 HP가 0이 되면 true 그외는 false를 반환한다.
    public bool Set_Demage(float AttackDamage, string type)
    {
        if (state == STATE.DEAD) { return false; }

        Hp -= AttackDamage;
        UIManager.Get_Inctance().Set_Damage(gameObject, AttackDamage, type);

        if (Hp <= 0)
        {
            Hp = 0;
            Set_AniDead();
            return true;
        }

        return false;
    }

    // HP를 반환하는 함수.
    public float Get_HP()
    {
        return Hp;
    }


    // State 표시 UI나 Effect등을 삭제하는 함수.
    public virtual void State_OFF()
    {
        if(type == TYPE.BASE)
        {
            Condition.transform.DestroyChildren();
        }
        else if (type == TYPE.BOSS)
        {

        }
    }

    // GameObject의 SetAtive를 false하는 함수.
    public void Off_Active() { gameObject.SetActive(false); }

    // 도발에 걸렸는지 아닌지를 체크하는 함수.
    public bool Check_StateProvocation()
    {
        if (state == STATE.PROVOCATION)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // 해당 obj가 죽었는지 아닌지를 체크하는 함수.
    public bool Check_Dead()
    {
        if (state == STATE.DEAD)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //                                                                          //
    //               아래 부터는 상태 이상에 관한 함수 
    //                                                                           //


    // Charm에 걸렸을때 실행되는 함수.
    // time만큼 지난 후에는 Effect를 끄고 StartSet_Attack()를 실행시킨다. 몬스터 타입에 따라 상태이상을 표시하는 방식이 달라진다.
    public void Set_StateCharm(float time)
    {
        state = STATE.CHARM;

        if (type == TYPE.BASE)
        {
            GameObject CharmEffect = Instantiate(EffectManager.Get_Inctance().Charm_Effect) as GameObject;
            CharmEffect.transform.parent = Condition.transform;
            CharmEffect.transform.localPosition = Vector3.zero;

            Invoke("State_OFF", time);
            Invoke("StartSet_Attack", time);
        }
        else if (type == TYPE.BOSS)
        {

        }
    }
    // 도발에 걸렸을때 실행되는 함수.
    public void Set_StateProvocation()
    {
        state = STATE.PROVOCATION;
    }
}
