using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class StateWinManager : MonoBehaviour {

   public UILabel Gold;
   public UILabel Time;
   public UILabel Level;
   public UILabel Exp_text;
   public UISlider Exp_slider;
   public UISprite[] Stars;
   public UIGrid Items;

    int get_gold = 0;

    public GameObject ItemPrefab;

    private static StateWinManager instance = null;

    // Use this for initialization
    void Awake()
    {
        instance = this;

    }
    public static StateWinManager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(StateWinManager)) as StateWinManager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("StateWinManager ");
            instance = obj.AddComponent(typeof(StateWinManager)) as StateWinManager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

    public void View_UI(float timer, int level, int exp)
    {
        get_gold = Random.Range(300, 500);

        Gold.text = get_gold.ToString();

        Time.text = timer.ToString();

        Level.text = level.ToString();

        Exp_text.text = exp.ToString() + " / " + (200f + level * 150f).ToString();

        float value = exp / (200f + level * 150f);
        Exp_slider.value = value;

        Send_ClearReward();

        GameObject Item1 = Instantiate(ItemPrefab, Items.transform) as GameObject;
        Item1.transform.localScale = Vector3.one;
        Item1.transform.localPosition = Vector3.zero;
        Item1.GetComponent<StageItemAction>().Set_UI(STATEITEM_TYPE.GOLD, "gold", get_gold, "돈");

        GameObject Item2 = Instantiate(ItemPrefab, Items.transform) as GameObject;
        Item2.transform.localScale = Vector3.one;
        Item2.transform.localPosition = Vector3.zero;
        Item2.GetComponent<StageItemAction>().Set_UI(STATEITEM_TYPE.ITEM, "bandage_rainbow", 1, "빛의 천");

        GameObject Item3 = Instantiate(ItemPrefab, Items.transform) as GameObject;
        Item3.transform.localScale = Vector3.one;
        Item3.transform.localPosition = Vector3.zero;
        Item3.GetComponent<StageItemAction>().Set_UI(STATEITEM_TYPE.CHARATER, "wizard", 1, "wizard");

        Items.repositionNow = true;
    }

    void Send_ClearReward()
    {
        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "SetClearReward");

        // 나중에 이부분 수정하기 ㅠㅠㅠㅠㅠㅠㅠㅠㅠㅠㅠㅠㅠ
        int[] types = { (int)STATEITEM_TYPE.GOLD, (int)STATEITEM_TYPE.ITEM, (int)STATEITEM_TYPE.CHARATER };
        int[] values = { get_gold, 21111, 4 };

        sendData.Add("types", types);
        sendData.Add("values", values);
        sendData.Add("user_id", GameManager.Get_Inctance().Index);

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, Reply_SendClearReward));
    }

    void Reply_SendClearReward(string json)
    {
        // JsonReader.Deserialize() : 원하는 자료형의 json을 만들 수 있다
        RecvClearReward data = JsonReader.Deserialize<RecvClearReward>(json);

        GameManager.Get_Inctance().Gold = data.gold;
        Gold.text = data.gold.ToString();
    }

    private class RecvClearReward
    {
        public int gold;
    }

}
