using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// 게임 전체적인 부분을 담당하는 스크립트.
// 플레이어의 정보 ( Level이나 선택한 Player 캐릭터, 서포트 , 전체 획득 골드 등등등 )
public class GameManager : MonoBehaviour {

    public string Select_Support_Name = null;
    public List<string> SelectCharaters = new List<string>();
    List<string> GetCharaters = new List<string>();
    List<string> Get_Supporter = new List<string>();
    List<string> Get_Item = new List<string>();

    public float Level = 1f;
    public float Exp = 100f;
    public float Gold = 0f;


    private static GameManager instance = null;

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

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    public bool SelectCharater(string name, bool check)
    {
        if (check == false)
        {
            int index = SelectCharaters.IndexOf(name);
            SelectCharaters[index] = null;
            GameUIManager.Get_Inctance().Off_SelectCharaterUI(name);
            return true;
        }
        else
        {
            int index = -1;

            index = SelectCharaters.IndexOf(null);

            if (index == -1)
            {
                if (SelectCharaters.Count >= 3)
                {
                    Debug.Log("3명이상은 선택하실수 없습니다. // 나중에 캐릭터 교체되게 수정");
                    return false;
                }

                SelectCharaters.Add(name);
                index = SelectCharaters.Count - 1;
            }
            else
            {
                SelectCharaters[index] = name;
            }

            RecvCharaterInfo info = InfoManager.Get_Inctance().Get_CharaterInfo(name);
            GameUIManager.Get_Inctance().On_SelectCharaterUI(index, info.Type, info.Name, info.Level);
            return true;

        }
    }

    public void StateScene_Load()
    {
        StartCoroutine(C_StartState());
    }
    IEnumerator C_StartState()
    {
        RecvCharaterInfo[] SelectCharaterInfos = new RecvCharaterInfo[3];

        for (int index = 0; index < SelectCharaters.Count; index++)
        {
            if (SelectCharaters[index] == null) { continue; }

            SelectCharaterInfos[index] = InfoManager.Get_Inctance().Get_CharaterInfo(SelectCharaters[index]);
        }

        AsyncOperation async = SceneManager.LoadSceneAsync("Stage");
        transform.DestroyChildren();

        while (!async.isDone)
        {
            yield return null;
        }

        GameStateManager.Get_Inctance().StartState(SelectCharaterInfos);
    }
    public void EndState()
    {

    }
}
