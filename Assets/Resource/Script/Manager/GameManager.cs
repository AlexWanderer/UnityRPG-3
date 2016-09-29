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
    public PlayerManager PlayerManager = null;                                              // 플레이어 관리 스크립트
   


    private static GameManager instance = null;

    // Use this for initialization
    void Awake ()
    {
        instance = this;
        GMstate = GMSTATE.NEXT;
      
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
                break;

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
                    break;
                }
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

    public void Set_Boss()
    {
        GMstate = GMSTATE.BOSS;
    }

}
