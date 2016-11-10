using UnityEngine;
using System.Collections;

public class GameUIManager : MonoBehaviour {

   public GameObject[] SelectWindow = new GameObject[3];
    public UILabel Label_Level;
    public UISlider Slider_Exp;
    public UILabel Label_Gold;

    private static GameUIManager instance = null;

    public static GameUIManager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(GameUIManager)) as GameUIManager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("GameManager ");
            instance = obj.AddComponent(typeof(GameUIManager)) as GameUIManager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

    void Awake()
    {
        Label_Level.text = GameManager.Get_Inctance().Level.ToString();
        float value = GameManager.Get_Inctance().Exp * (200f + GameManager.Get_Inctance().Level * 150f) * 0.001f;
        Slider_Exp.value = value;
        Label_Gold.text = GameManager.Get_Inctance().Gold.ToString();
    }

    public void Set_PlayerLevel(float level)
    {
        Label_Level.text = level.ToString();
    }
    public void Set_PlayerExp(float exp, float level)
    {
        float value = exp * (200f + level * 150f) * 0.001f;
        Slider_Exp.value = value;
    }
    public void Set_PlayerGold(float gold)
    {
        Label_Gold.text = gold.ToString();
    }

    public void On_SelectCharaterUI(int count, string type, string name, int level)
    {
        SelectWindow[count].GetComponent<SelectCharaterInfo_Action>().SelectCharaterInfo(level, name, type);
        SelectWindow[count].SetActive(true);
    }
    public void Off_SelectCharaterUI(string name)
    {
        for(int i = 0; i < SelectWindow.Length; i++)
        {
            string UI_name = SelectWindow[i].transform.FindChild("Sprite_CharaterIcon").GetComponent<UISprite>().spriteName;

            if (UI_name.Contains(name))
            {
                SelectWindow[i].SetActive(false);
                return;
            }
        }
    }
    public void Set_StartStage()
    {
        GameManager.Get_Inctance().StateScene_Load();
    }
}
