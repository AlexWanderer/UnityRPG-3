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
    public int Special_value;
    public int Special_time;
    public int Star;
}

public class InfoManager: MonoBehaviour {

    Dictionary<string, RecvCharaterInfo> CharaterInfos = new Dictionary<string, RecvCharaterInfo>();

    public GameObject CharaterView = null;
    public GameObject CharaterInfo_Prefab = null;

    private static InfoManager instance = null;

    public static InfoManager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(InfoManager)) as InfoManager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("InfoManager ");
            instance = obj.AddComponent(typeof(InfoManager)) as InfoManager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

    void Awake()
    {
        instance = this;
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
            ReadyViewCharaterInfo(CharaterInfos[data.Name]);
        }

    }
    private void ReadyViewCharaterInfo(RecvCharaterInfo data)
    {
        GameObject Info = Instantiate(CharaterInfo_Prefab, CharaterView.transform) as GameObject;
        Info.name = data.Name;
        Info.transform.localScale = Vector3.one;
        Info.GetComponent<CaraterInfo_Action>().Set_CharaterInfo(data.Name, data.Attack, data.Defense, data.Type, data.Star);

        CharaterView.GetComponent<UIGrid>().repositionNow = true;
    }
    public RecvCharaterInfo Get_CharaterInfo(string name)
    {
        return CharaterInfos[name];
    }


}
