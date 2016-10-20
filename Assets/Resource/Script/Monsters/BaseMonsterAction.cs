using UnityEngine;
using System.Collections;

// MikuNegi Monster의 스크립트.
public class BaseMonsterAction : MonsterAction {

    void OnEnable()
    {
        type = TYPE.BASE;

        Condition = transform.FindChild("Condition").gameObject;

        // 몬스터가 활성화되면 UIManager에게 자신의 Hpbar를 만들라고 요구한다.
        UIManager.Get_Inctance().Set_Hpbar(gameObject);                            
    }

    public override void StartSet_Attack()
    {
        // 이미 Attack Coroutine이 실행되고 있거나 사망했으면 함수를 종료한다
        // 만약 CSet_Attack이 실행중인데 Start를 하게되면 Error가 난다.
        // Attack Corountine이 돌아가면 state가 Attack상태이고 state가 Attack이 아니면 Attack Coroutine이 내부에서 종료된다.
        // state가 Attack상태라면 이미 Attack Coroutine이 돌아가고 있다는 말이기 때문에 Start할 필요가 없으므로 return한다.
        if (state == STATE.ATTACK || state == STATE.DEAD) { return; }

        StartCoroutine(CSet_AniAttack());
    }

    // 공격하는 Ani를 실행시키는 Coroutine.
    IEnumerator CSet_AniAttack()
    {
        state = STATE.ATTACK;

        while (true)
        {
            // state가 Attack이나 도발 ( 특정 상대를 공격 ) 상태가 아니면 Coroutine을 종료한다.
            if (state != STATE.ATTACK && state != STATE.PROVOCATION) { yield break; }

            if (Target == null || Target.Check_Dead())
            {
                PlayerManager.Get_Inctance().Set_ReTarget(this);
            }

            // Target이 있는쪽을 바라본다.
            Vector3 targetPos = Target.gameObject.transform.position;
            targetPos.y = transform.position.y;
            Vector3 v = targetPos - transform.position;
            transform.rotation = Quaternion.LookRotation(v);

            // Target과의 거리가 1.5f이상이면 앞으로 움직이고 아니면 공격한다.
            if (Distance(Target.transform.position, transform.position) > 1.5f)
            {
                ani.SetBool("Attack", false);
                ani.SetBool("Move", true);
                transform.Translate(Vector3.forward * Time.deltaTime * Speed);
            }
            else
            {
                ani.SetBool("Attack", true);
            }
            yield return null;
        }
    }
    // Target을 공격하는 함수. Attack Ani에서 이 함수를 호출한다.
    public void Player_Attack()
    {
        Target.Set_Demage(Attack, null);
    }

    // Charm에 걸렸을때 실행되는 함수.
    // time만큼 지난 후에는 Effect를 끄고 StartSet_Attack()를 실행시킨다. 몬스터 타입에 따라 상태이상을 표시하는 방식이 달라진다.
    public override void Set_StateCharm(float time)
    {
        state = STATE.CHARM;


        GameObject CharmEffect = Instantiate(EffectManager.Get_Inctance().Charm_Effect) as GameObject;
        CharmEffect.transform.parent = Condition.transform;
        CharmEffect.transform.localPosition = Vector3.zero;

        Invoke("State_OFF", time);
        Invoke("StartSet_Attack", time);
    }

    void OnCollisionEnter(Collision obj)
    {
        // Player랑 충돌하면 Target을 충돌한 Player로 변경한다.
        if(obj.gameObject.CompareTag("Player"))
        {
            if(state == STATE.PROVOCATION) { return; }

            Target = obj.gameObject.GetComponent<PlayerAction>();
        }
    }
    
    // A과 B의 거리를 구하는 함수
    float Distance(Vector3 A, Vector3 B)
    {
        return Mathf.Abs(Vector3.Distance(A, B));
    }
}
