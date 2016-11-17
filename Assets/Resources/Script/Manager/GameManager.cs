using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using JsonFx.Json;

// 게임 전체적인 부분을 담당하는 스크립트.
// 플레이어의 정보 ( Level이나 선택한 Player 캐릭터, 서포트 , 전체 획득 골드 등등등 )
public class GameManager : MonoBehaviour
{

    public string Select_Support_Name = null;
    public List<int> SelectCharaters = new List<int>();
    public List<int> GetCharaters = new List<int>();
    List<string> Get_Supporter = new List<string>();
    public Dictionary<int, int> Get_Item = new Dictionary<int, int>();
    public Dictionary<int, int> Charater_Equipment = new Dictionary<int, int>();

    public int Index;
    public int Level;
    public float Exp;
    public float Gold;

    GameObject MessageBox = null;
    public GameObject MessageBox_Prefab = null;


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
            GameUIManager.Get_Inctance().Off_SelectCharaterUI(List_index);
            return true;
        }
        else
        {
            int List_index = -1;

            //  선택했다 취소한 애가 있는지 체크
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

            GameUIManager.Get_Inctance().On_SelectCharaterUI(index, List_index);
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
        if (Charater_Equipment.ContainsKey(charater_index))
        {
            Charater_Equipment[charater_index] = item_id;
        }
        else
        {
            Charater_Equipment.Add(charater_index, item_id);
        }

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "SetCharaterEquipment");

        sendData.Add("user_id", Index);
        sendData.Add("charater_index", charater_index);
        sendData.Add("item_id", item_id);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, Reply_SetCharaterEquipment));
    }
    void Reply_SetCharaterEquipment(string json)
    {
        Debug.Log("장비를 선택할때마다 서버에 연결하는데 좀더 효과적인 방법이 없을까?");
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

    public void Set_PlayerInfo(int level, float exp, float gold, int index)
    {
        Index = index;
        Level = level;
        Exp = exp;
        Gold = gold;

        Get_PlayerCharaterInfo();
    }

    public void Get_PlayerCharaterInfo()
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "GetUserCharaterInfo");

        sendData.Add("user_id", Index);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyPlayerCharaterInfo));
    }
    public void ReplyPlayerCharaterInfo(string json)
    {
        // JsonReader.Deserialize() : 원하는 자료형의 json을 만들 수 있다
        Dictionary<string, object> dataDic = (Dictionary<string, object>)JsonReader.Deserialize(json, typeof(Dictionary<string, object>));

        if (dataDic == null) { return; }

        foreach (KeyValuePair<string, object> info in dataDic)
        {
            RecvPlayerCharaterInfo data = JsonReader.Deserialize<RecvPlayerCharaterInfo>(JsonWriter.Serialize(info.Value));

            GetCharaters.Add(data.id);
            Charater_Equipment.Add(data.id, data.equipment_id);
        }
    }

    void LoadPlayerData()
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "GetUserCharaterInfo");

        sendData.Add("user_id", Index);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyPlayerCharaterInfo));
    }

    public void Set_Message(string message)
    {
        if(MessageBox == null)
        {
            GameObject MB = Instantiate(MessageBox_Prefab, GameObject.Find("UI Root").gameObject.transform) as GameObject;
            MB.transform.localPosition = Vector3.zero;
            MB.transform.localScale = Vector3.one;
            MessageBox = MB;
        }

        MessageBox.GetComponent<MessageBox_Action>().Set_Message(message);
        return;
    }

    public bool Check_SelectCharater(int index)
    {
        return SelectCharaters.Contains(index);
    }
    private class RecvPlayerCharaterInfo
    {
        public int id;
        public int equipment_id;
    }

}


