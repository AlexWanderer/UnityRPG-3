using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 게임 전체적인 부분을 담당하는 스크립트.
// 플레이어의 정보 ( Level이나 선택한 Player 캐릭터, 서포트 , 전체 획득 골드 등등등 )
public class GameManager : MonoBehaviour {

    public string Select_Support_Name = null;
    public GameObject[] Select_PlayerCharate = new GameObject[3];

    public float Level = 1f;
    public float Exp = 100f;
    public float Gold = 0f;
    public List<GameObject> Get_Charater = new List<GameObject>();
    public List<GameObject> Get_Supporter = new List<GameObject>();
    public List<GameObject> Get_Item = new List<GameObject>();


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
}
