using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

// 수신받을 변수명은 서버에서 전달된 변수명과 동일해야함.
// 캐릭터의 정보를 받는 클래스
public class RecvCharaterInfo
{
    public string Name;
    public string Type;
    public int Level;
    public int Hp;
    public int Attack;
    public int Defense;
    public int Skillpoint;
    public int Speed;
    public int Touch_value;
    public int Touch_time;
    public string Touch_text;
    public int Special_value;
    public int Special_time;
    public string Special_text;
    public int Star;
}


public class CharaterManager: MonoBehaviour {

    private static volatile CharaterManager uniqueInstance;
    private static object _lock = new System.Object();

    Dictionary<string, RecvCharaterInfo> CharaterInfos = new Dictionary<string, RecvCharaterInfo>();

    public GameObject SelectCharaterView = null;
    public GameObject SelectCharaterInfo_Prefab = null;

    public GameObject CharaterInfoView = null;
    public GameObject CharaterInfo_Prefab = null;

    public GameObject CharaterDetailedInfo = null;

    private CharaterManager() { }

    public static CharaterManager Instance
    {
        get
        {
            if (uniqueInstance == null)
            {
                lock (_lock)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new CharaterManager();
                    }
                }
            }
            return uniqueInstance;
        }
    }

    void Awake()
    {
        uniqueInstance = this;
    }

    // Use this for initialization
    void Start ()
    {

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "GetCharaterInfo");

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyCharaterInfo));

    }

    //php에서 보낸 아이템의 모든 정보를 가져와 CharaterInfos에 저장하는 함수.
    public void ReplyCharaterInfo(string json)
    {
        // JsonReader.Deserialize() : 원하는 자료형의 json을 만들 수 있다
        Dictionary<string, object> dataDic = (Dictionary<string, object>)JsonReader.Deserialize(json, typeof(Dictionary<string, object>));

        foreach (KeyValuePair<string, object> info in dataDic)
        {
            RecvCharaterInfo data = JsonReader.Deserialize<RecvCharaterInfo>(JsonWriter.Serialize(info.Value));

            CharaterInfos.Add(data.Name, data);
            ReadyViewSelectCharaterInfo(CharaterInfos[data.Name]);
        }
        View_GetCharaterInfo(GameManager.Get_Inctance().GetCharaters);
    }

    private void ReadyViewSelectCharaterInfo(RecvCharaterInfo data)
    {
        GameObject Info = Instantiate(SelectCharaterInfo_Prefab, SelectCharaterView.transform) as GameObject;
        Info.name = data.Name;
        Info.transform.localScale = Vector3.one;
        Info.GetComponent<CaraterInfo_Action>().Set_CharaterInfo(data.Name, data.Attack, data.Defense, data.Type, data.Star);

        SelectCharaterView.GetComponent<UIGrid>().repositionNow = true;
    }

    public void View_GetCharaterInfo(List<string> names)
    {
        for (int i = 0; i < names.Count; i++)
        {
            GameObject Info = Instantiate(CharaterInfo_Prefab, CharaterInfoView.transform) as GameObject;
            RecvCharaterInfo data = Get_CharaterInfo(names[i]);
            Info.name = data.Name;
            Info.transform.localScale = Vector3.one;
            Info.GetComponent<CharaterInfo_Action>().Set_CharaterInfo(data.Name, data.Level, data.Star, data.Type);

            CharaterInfoView.GetComponent<UIGrid>().repositionNow = true;

            // GameManager에 캐릭터 Get하는 부분 구현

            Set_ButtonCharaterDetailedInfo(Info.GetComponent<UIButton>(), data.Name);
        }
    }

    public void Set_ButtonCharaterDetailedInfo(UIButton button, string charater_name)
    {
        EventDelegate.Parameter param = new EventDelegate.Parameter();

        param.value = charater_name;
        param.expectedType = typeof(string);

        EventDelegate onClick = new EventDelegate(CharaterDetailedInfo.GetComponent<Charater_DetailedInfo_Action>(), "Set_Charater_DetailedInfo");

        onClick.parameters[0] = param;
        EventDelegate.Add(button.onClick, onClick);
    }

    public RecvCharaterInfo Get_CharaterInfo(string name)
    {
        return CharaterInfos[name];
    }
}
