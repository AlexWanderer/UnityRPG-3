using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseLongMonsterAction : MonsterAction {

    public GameObject AttackEffect_Prefab = null;
    public GameObject AttackPos = null;
    List<GameObject> AttackEffectList = new List<GameObject>();

    void OnEnable()
    {
        type = TYPE.BASE;

        Condition = transform.FindChild("Condition").gameObject;

        // 몬스터가 활성화되면 UIManager에게 자신의 Hpbar를 만들라고 요구한다.
        UIManager.Get_Inctance().Set_Hpbar(gameObject);
    }

    // Long타입의 공격을 하는 몬스터는 StartSet_Attack()함수에서 Attack Ani를 실행시킨다.
    public override void StartSet_Attack()
    {
        // Monster가 죽어있으면 함수를 종료한다.
        if (state == STATE.DEAD) { return; }

        Target = PlayerManager.Get_Inctance().Get_RandomPlayer().GetComponent<PlayerAction>();

        // Attack Ani를 실행시킨다.
        ani.SetBool("Attack", true);
    }

    // Target을 공격하는 Coroutine. Attack Ani에서 호출한다.
    IEnumerator CSet_Attack()
    {
        state = STATE.ATTACK;
        // 비활동중인 Attack Effect가 담기는 변수
        GameObject Effect = null;

        // Target이 null이거나 죽어있으면 새로운 Target을 받는다.
        if (Target == null || Target.Check_Dead())
        {
            PlayerManager.Get_Inctance().Set_ReTarget(this);
        }

        // AttackEffect가 하나도 만들어져있지 않으면 Effect OBJ를 하나 만든다.
        if (AttackEffectList.Count == 0)
        {
            Create_AttackEffect();
        }
        // 활동중이지 않은 Effect를 체크해 변수에 넣는다.
        for (int i = 0; i < AttackEffectList.Count; i++)
        {
            if (AttackEffectList[i].GetComponent<GhostAttakEffect_Action>().active == false)
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
        Effect.transform.position = AttackPos.transform.position;
        // AttackEffect가 Active된 순간 Effect안에 있는 스크립트가 작동한다.
        Effect.SetActive(true);
        yield break;
    }
    // Attack Effect를 만드는 함수.
    void Create_AttackEffect()
    {
        GameObject obj = Instantiate(AttackEffect_Prefab).gameObject;
        obj.AddComponent<GhostAttakEffect_Action>();
        obj.GetComponent<GhostAttakEffect_Action>().AttackDamage = Attack;
        obj.SetActive(false);
        obj.name = "GhostAttack";
        obj.transform.parent = null;
        AttackEffectList.Add(obj);

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
}
