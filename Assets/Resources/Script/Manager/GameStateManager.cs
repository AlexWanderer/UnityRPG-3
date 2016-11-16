using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//스테이지의 흐름을 관리하는 스크립트
public class GameStateManager : MonoBehaviour {

    public enum GMSTATE
    {
        IDLE,                               // 대기
        NEXT,                             // 다음장소로 이동
        BOSS,                             // 보스 만남
        FAILD,                            // 실패
        WIN,                              // 승리
        MAX 
    };

    public GMSTATE GMstate = GMSTATE.IDLE;                                                 // 스테이지의 상태

    Vector3 PlayerStandPos = Vector3.zero;
    float Timer = 0f;

    public GameObject Support_obj = null;
   
    private static GameStateManager instance = null;
    GameObject Boss = null;

    int All_Wave = 0;
    int Now_Wave = 0;

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }


    public static GameStateManager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(GameStateManager)) as GameStateManager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("GameStateManager ");
            instance = obj.AddComponent(typeof(GameStateManager)) as GameStateManager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

    public void StartState(RecvCharaterInfo[] SelectCharaterNames)
    {
        PlayerStandPos = PlayerManager.Get_Inctance().transform.position;
        Boss = MonsterManager.Get_Inctance().gameObject.transform.FindChild("Boss_Collision").FindChild("EndPos").gameObject;

        All_Wave = MonsterManager.Get_Inctance().transform.childCount;
        Now_Wave = 0;

        PlayerManager.Get_Inctance().Ready_State(SelectCharaterNames);

        GMstate = GMSTATE.NEXT;

        StartCoroutine(C_StartState());
    }

    IEnumerator C_StartState()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {

            Timer += Time.deltaTime;
            UIManager.Get_Inctance().Set_Time((int)Timer);

            // 게임 상황을 Update문으로 계속 확인한다.
            switch (GMstate)
            {
                case GMSTATE.IDLE:
                    {
                        break;
                    }
                case GMSTATE.NEXT:
                    {
                        // 다음 장소로 가기 위해 Player들을 전부 대기상태로 한번 만들고 이동한다.
                        PlayerManager.Get_Inctance().Set_Idle();
                        PlayerManager.Get_Inctance().Invoke("Set_Move", 0.6f);

                        Now_Wave = 1;

                        for(int i = 0; i < All_Wave; i++)
                        {
                            if(MonsterManager.Get_Inctance().transform.GetChild(i).GetComponent<BoxCollider>().enabled == false)
                            {
                                Now_Wave++;
                            }
                        }

                        UIManager.Get_Inctance().Set_WaveUI(Now_Wave, All_Wave);
                        GMstate = GMSTATE.IDLE;
                        break;
                    }

                case GMSTATE.BOSS:
                    {
                        PlayerManager.Get_Inctance().Set_Idle();
                        MonsterManager.Get_Inctance().Set_Idle();
                        GMstate = GMSTATE.IDLE;
                        break;
                    }

                case GMSTATE.WIN:
                    {
                        PlayerManager.Get_Inctance().Set_Off();
                        UIManager.Get_Inctance().Set_WinUI();
                        StateWinManager.Get_Inctance().View_UI(Timer, GameManager.Get_Inctance().Level, 50);
                        GMstate = GMSTATE.IDLE;
                        break;
                    }
                case GMSTATE.FAILD:
                    {
                        MonsterManager.Get_Inctance().Set_Idle();
                        UIManager.Get_Inctance().Set_FaildUI();
                        break;
                    }
            }

            float distance = Distance_Percent(PlayerStandPos, Boss.transform.position, PlayerManager.Get_Inctance().transform.position);
            UIManager.Get_Inctance().Set_Space(distance);

            if (distance > 0.85f && distance < 0.95f)
            {
                UIManager.Get_Inctance().Set_Warning();
            }

            yield return null;
        }

    }

    // 몬스터를 모두 해치우고 다음장소로 넘어가기위한 함수.
    public void Set_Next()
    {
        GMstate = GMSTATE.NEXT;
    }
    // 스테이지를 실패하면 실행되는 함수
    public void Set_Faild()
    {
        GMstate = GMSTATE.FAILD;
    }
    public void Set_Win()
    {
        GMstate = GMSTATE.WIN;
    }
    public void Set_Boss()
    {
        GMstate = GMSTATE.BOSS;
    }

    public float Distance_Percent(Vector3 A, Vector3 B, Vector3 NowPos)
    {
        float T = Vector3.Distance(A, B);
        float C = Vector3.Distance(A, NowPos);

        float value = C / T;

        return value;
    }


    public void MainScene_Load()
    {
        StartCoroutine(C_MainScene());
    }
    IEnumerator C_MainScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("Main");

        while (!async.isDone)
        {
            yield return null;
        }
    }
}
