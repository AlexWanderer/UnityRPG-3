using UnityEngine;
using System.Collections;

public class GameUIManager : MonoBehaviour {

   public GameObject[] SelectWindow = new GameObject[3];

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
}
