using UnityEngine;
using System.Collections;

public class WarningAction : MonoBehaviour {

    bool Is_C_ing = false; 
    UIPanel IconPanel = null;

    void Awake()
    {
        IconPanel = GetComponent<UIPanel>();
    }
    public void Set_Warning()
    {
        if(Is_C_ing) { return; }

        StartCoroutine(C_Warning());
    }
    IEnumerator C_Warning()
    {
        Is_C_ing = true;
        IconPanel.alpha = 1f;
        yield return new WaitForSeconds(0.2f);
        IconPanel.alpha = 0f;
        yield return new WaitForSeconds(0.2f);
        Is_C_ing = false;
        yield break;
    }

}
