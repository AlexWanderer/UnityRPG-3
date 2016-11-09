using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// 게임 전체적인 부분을 담당하는 스크립트.
// 플레이어의 정보 ( Level이나 선택한 Player 캐릭터, 서포트 , 전체 획득 골드 등등등 )
public class GameManager : MonoBehaviour {

    public string Select_Support_Name = null;
    public List<string> SelectCharaters = new List<string>();
    public List<string> GetCharaters = new List<string>();
    List<string> Get_Supporter = new List<string>();
   public List<int> Get_Item = new List<int>();
   public Dictionary<string, int> Charater_Equipment = new Dictionary<string, int>();

    public float Level = 1f;
    public float Exp = 100f;
    public float Gold = 10000f;

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
        GetCharaters.Add("warrior");
        GetCharaters.Add("nurse");
        GetCharaters.Add("pirate");
        GetCharaters.Add("wizard");
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

            RecvCharaterInfo info = CharaterManager.Instance.Get_CharaterInfo(name);
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

            SelectCharaterInfos[index] = CharaterManager.Instance.Get_CharaterInfo(SelectCharaters[index]);
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

    public void Set_BuyItem(int item_id, int user_gold)
    {
       Get_Item.Add(item_id);
        InventoryManager.Get_Inctance().View_AddItem(item_id);
    }
    public void Set_Charater_Equipment(string charater_name, int item_id)
    {
        if(Charater_Equipment.ContainsKey(charater_name))
        {
            Charater_Equipment[charater_name] = item_id;
        }
        else
        {
            Charater_Equipment.Add(charater_name, item_id);
        }
    }
    public int Get_CharaterEquipment_id(string charater_name)
    {
        if (Charater_Equipment.ContainsKey(charater_name) == false)
        {
            return -1;
        }
        else
        {
            return Charater_Equipment[charater_name];
        }
    }
}
