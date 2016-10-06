using UnityEngine;
using System.Collections;

// Condition 이나 기타등등 공용으로 쓰이는 Effect를 관리하는 스크립트 
public class EffectManager : MonoBehaviour {

    public GameObject Charm_Effect = null;
    public GameObject Poison_Effect = null;



    private static EffectManager instance = null;

    // Use this for initialization
    void Awake()
    {
        instance = this;

    }
    public static EffectManager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(EffectManager)) as EffectManager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("EffectManager ");
            instance = obj.AddComponent(typeof(EffectManager)) as EffectManager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

}
