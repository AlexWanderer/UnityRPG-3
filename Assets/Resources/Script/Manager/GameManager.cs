using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// 게임 전체적인 부분을 담당하는 스크립트.
// 플레이어의 정보 ( Level이나 선택한 Player 캐릭터, 서포트 , 전체 획득 골드 등등등 )
public class GameManager : MonoBehaviour {

    public string Select_Support_Name = null;
    public List<int> SelectCharaters = new List<int>();
    public List<int> GetCharaters = new List<int>();
    List<string> Get_Supporter = new List<string>();
   public Dictionary<int, int> Get_Item = new Dictionary<int, int>();
   public Dictionary<int, int> Charater_Equipment = new Dictionary<int, int>();

    public int Index;
    public float Level;
    public float Exp;
    public float Gold;

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
    public bool SelectCharater(int index, bool check)
    {
        if (check == false)
        {
            int List_index = SelectCharaters.IndexOf(index);
            SelectCharaters[List_index] = 0;
            GameUIManager.Get_Inctance().Off_SelectCharaterUI(name);
            return true;
        }
        else
        {
            int List_index = -1;

            List_index = SelectCharaters.IndexOf(0);

            if (List_index == -1)
            {
                if (SelectCharaters.Count >= 3)
                {
                    Debug.Log("3명이상은 선택하실수 없습니다. // 나중에 캐릭터 교체되게 수정");
                    return false;
                }

                SelectCharaters.Add(index);
                List_index = SelectCharaters.Count - 1;
            }
            else
            {
                SelectCharaters[List_index] = index;
            }

            RecvCharaterInfo info = CharaterManager.Instance.Get_CharaterInfo(index);
            GameUIManager.Get_Inctance().On_SelectCharaterUI(List_index, info.Type, info.Name, info.Level);
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
            if (SelectCharaters[index] == 0) { continue; }

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

    public void Set_BuyItem(int item_id, float user_gold, int count)
    {
        if (Get_Item.ContainsKey(item_id))
        {
            Get_Item[item_id] = count;
        }
        else
        {
            Get_Item.Add(item_id, count);
        }

        InventoryManager.Get_Inctance().View_AddItem(item_id, count);
        GameUIManager.Get_Inctance().Set_PlayerGold(Gold);
    }
    public void Set_Charater_Equipment(int charater_index, int item_id)
    {
        if(Charater_Equipment.ContainsKey(charater_index))
        {
            Charater_Equipment[charater_index] = item_id;
        }
        else
        {
            Charater_Equipment.Add(charater_index, item_id);
        }
    }
    public int Get_CharaterEquipment_id(int charater_index)
    {
        if (Charater_Equipment.ContainsKey(charater_index) == false)
        {
            return -1;
        }
        else
        {
            return Charater_Equipment[charater_index];
        }
    }

    public void Set_PlayerInfo(float level, float exp, float gold, int index)
    {
        Index = index;
        Level = level;
        Exp = exp;
        Gold = gold;
    }

    public void Save_PlayerInfo(float level, float exp, float gold)
    {

    }
}


