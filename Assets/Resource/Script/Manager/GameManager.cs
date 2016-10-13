using UnityEngine;
using System.Collections;

//게임의 흐름을 관리하는 스크립트
public class GameManager : MonoBehaviour {

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
   
    private static GameManager instance = null;
    GameObject Boss = null;

    // Use this for initialization
    void Awake ()
    {
        instance = this;
        GMstate = GMSTATE.NEXT;
        PlayerStandPos = PlayerManager.Get_Inctance().transform.position;
        Boss =  MonsterManager.Get_Inctance().gameObject.transform.FindChild("Boss_Collision").FindChild("EndPos").gameObject;
    }
    public static GameManager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(GameManager)) as GameManager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("GameManager ");
            instance = obj.AddComponent(typeof(GameManager)) as GameManager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

    void Update ()
    {
        // 게임 상황을 Update문으로 계속 확인한다.
	    switch(GMstate)
        {
            case GMSTATE.IDLE:
                {
                    break;
                }
            case GMSTATE.NEXT:
                {
                    // 다음 장소로 가기 위해 Player들을 전부 대기상태로 한번 만들고 이동한다.
                    PlayerManager.Get_Inctance().Invoke("Set_Idle", 0.5f);
                    PlayerManager.Get_Inctance().Invoke("Set_Move", 0.6f);
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
        }

        UIManager.Get_Inctance().Set_Space(Distance_Percent(PlayerStandPos, Boss.transform.position, PlayerManager.Get_Inctance().transform.position));

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

}
