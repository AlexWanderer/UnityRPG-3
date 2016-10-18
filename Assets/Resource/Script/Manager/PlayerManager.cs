using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
   public enum STATE
    {
        IDLE,                               // 대기
        MOVE,                            // 이동
        ATTACK,                         // 공격
        SKILL,                             // 스킬중
        MAX,   
    };

    public STATE state = STATE.IDLE;
    // Player의 Character가 들어갈 배열이다.
    // 0 = Center       1 = Sub1          2 = Sub2
    public GameObject[] Characters = new GameObject[3];

    public GameObject LongAttackPoint = null; 

    private static PlayerManager instance = null;

    void Awake()
    {
        Characters[0] = transform.FindChild("Center").GetChild(0).gameObject;
        Characters[1] = transform.FindChild("Sub1").GetChild(0).gameObject;
        Characters[2] = transform.FindChild("Sub2").GetChild(0).gameObject;

        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i].activeSelf == false)
            {
                Characters[i] = null;
            }
        }

        state = STATE.MOVE;
        instance = this;
    }

    public static PlayerManager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("PlayerManager ");
            instance = obj.AddComponent(typeof(PlayerManager)) as PlayerManager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }


    void Update()
    {
        switch (state)
        {
            case STATE.IDLE:
                break;

            case STATE.MOVE:
                    break;

            case STATE.ATTACK:
                {
                    if (Input.GetMouseButton(0) == true)
                    {
                        Touch_Player();
                    }
                    break;
                }
        }
    }

    public void Set_Move()
    {
        state = STATE.MOVE;
        // Character의 이동 Ani를 호출한다.
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == null)
            {
                break;
            }

            Characters[i].GetComponent<PlayerAction>().Set_Move();
        }
        GetComponent<BoxCollider>().enabled = true;
        StartCoroutine(C_PlayerMove());

    }
    // Monster출연지점을 만날때까지 이동한다.
    IEnumerator C_PlayerMove()
    {
        while (true)
        {
            if (state != STATE.MOVE)
                yield break;

            transform.Translate(Vector3.forward * Time.deltaTime * 3f);
            yield return null;
        }
    }
    // Character의 Target을 설정해주고 Attack함수를 호출한다.
    public void Set_Attack()
    {
        state = STATE.ATTACK;

        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == null || Characters[i].GetComponent<PlayerAction>().Check_Dead())
            {
                continue;
            }


            PlayerAction Player = Characters[i].GetComponent<PlayerAction>();

            MonsterManager.Get_Inctance().Set_ReTarget(Player);

            Player.Set_Attack();
        }
    }

    // Character의 Ani를 Idle로 설정한다.
    public void Set_Idle()
    {
        state = STATE.IDLE;
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == null || Characters[i].GetComponent<PlayerAction>().Check_Dead())
            {
                continue;
            }


            Characters[i].GetComponent<PlayerAction>().Set_Idle();
        }
    }

    // 매개변수 Monster의 Target에 살아있는 Character를 찾아 할당해주는 함수.
    public void Set_ReTarget(MonsterAction Monster)
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == null || Characters[i].GetComponent<PlayerAction>().Check_Dead())
            {
                continue;
            }

            Monster.Target = Characters[i].GetComponent<PlayerAction>();
        }
    }

    // 가장 피가 낮은 Player를 찾는 함수를 실행후 heal_value만큼 피를 채운다.
    // 그리고 피가 채워진 해당 Player를 반환한다.
    public GameObject Set_PlayerHeal(float heal_value)
    {
        PlayerAction target = Get_Player_LowHp().GetComponent<PlayerAction>();
        target.Hp += heal_value;

        if(target.Hp > target.InitHP)
        {
            target.Hp = target.InitHP;
        }

        UIManager.Get_Inctance().Set_Heal(target.gameObject, heal_value);

        return target.gameObject;
    }

    // 가장 피가 낮은 (%적으로) Player를 찾아 반환하는 함수.
    public GameObject Get_Player_LowHp()
    {
        float hp = 1000f;
        GameObject return_Player = null;

        for(int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == null) { continue; }

            PlayerAction player = Characters[i].GetComponent<PlayerAction>();
            float value = player.Hp / player.InitHP;

            if(hp > value)
            {
                return_Player = Characters[i];
                hp = value;
            }
        }

        return return_Player;

    }

    public GameObject Get_RandomPlayer()
    {
        if (Characters.Length == 0) { return null; }

        int rand = 0;

        do
        {
            rand = Random.Range(0, Characters.Length);

        } while (Characters[rand].activeSelf == false || Characters[rand] == null);

        return Characters[rand];
    }
    // Player의 Target이 null이거나 active가 false면 MonsterManager에서 ReTarget함수를 실행시킨다. 
    public void Check_Target()
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == null)
            {
                continue;
            }

            PlayerAction Player = Characters[i].GetComponent<PlayerAction>();
            if (Player.Target == null || Player.Target.gameObject.activeSelf == false)
            {
                MonsterManager.Get_Inctance().Set_ReTarget(Player);
            }
        }
    }

    // 사용자가 Player캐릭터를 터치했는지 체크하는 함수. 
    // 만약 터치했으면 해당 Player의 Touch_Skill을 실행시킨다.
    void Touch_Player()
    {
        // 카메라에서 화면상의 마우스 좌표에 해당하는 공간으로 레이를 쏜다.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Physics.Raycast(쏜 레이 정보, 충돌 정보, 거리)
        //  => 충돌이 되면 true를 리턴하면서 충돌 정보를 확인 할 수 있다.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 충돌한 obj를 가져와 obj가 Player일 경우 Skill을 발동시킨다.
            GameObject obj = hit.collider.gameObject;

            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<PlayerAction>().Touch_Skill();
            }

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);

        }
    }

    // 모든 Player들의 Active를 조종하는 함수. type이 ON이면 true, OFF면 false한다.
    // 만약 target이 null이 아니라면 target만 type에 따라 active를 조종한다.
    public void Players_Active(GameObject target, string type)
    {
        if (type.Equals("OFF"))
        {
            if (target == null)
            {
                for (int i = 0; i < Characters.Length; i++)
                {
                    Characters[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < Characters.Length; i++)
                {
                    if (Characters[i] == target)
                        continue;

                    Characters[i].SetActive(false);
                }
            }
        }
        else if(type.Equals("ON"))
        {
            for (int i = 0; i < Characters.Length; i++)
            {
                Characters[i].SetActive(true);
            }
        }

        return;
    }

    // Player가 죽으면 실행되는 함수.
    // Player가 다 죽으면 GM의 Set_Faild()를 호출한다.
    public void Check_Dead()
    {
        MonsterManager.Get_Inctance().Check_Target();

        int DeadCount = 0;

        //Character의 state를 비교후 Player후만큼 DeadCount가 쌓이면 GM에게 Faild시킨다.
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i].GetComponent<PlayerAction>().state.ToString().Equals("DEAD"))
            {
                DeadCount++;


                if(DeadCount == Characters.Length)
                {
                    GameManager.Get_Inctance().Set_Faild();
                    StopAllCoroutines();
                }
            }
        }
    }



    void OnTriggerEnter(Collider obj)
    {
        // Monster의 출현범위 안에 들어가면 Monster들을 활성화시키고 MonsterManager와 Player에게 Attack을 준비하는 함수를 호출한다.
        // state가 STATE.ATTACK으로 변한다.
        // Touch_Skill을 위해 PlayerManager의 Collider를 비활성화시킨다.
        if(obj.gameObject.name.Equals("Mob_Collision"))
        {
            state = STATE.ATTACK;
            obj.gameObject.SetActive(true);
            MonsterManager.Get_Inctance().Ready_Attack(obj.gameObject);
            Set_Attack();
            GetComponent<BoxCollider>().enabled = false;
        }

        // 보스 몬스터를 만나면 실행된다.
        if (obj.gameObject.name.Equals("Boss_Collision"))
        {
            state = STATE.ATTACK;
            obj.gameObject.SetActive(true);
            MonsterManager.Get_Inctance().Ready_BossAttack(obj.gameObject);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
