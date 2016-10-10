using UnityEngine;
using System.Collections;

public class StateAction : MonoBehaviour {

   public float LimitTime = -1f;
   public float Timer = 0f;
   public bool type = false;
   GameObject Icon = null;
    UIGrid grid = null;

   public void Start_Update(float time, string Symptom)
    {
        if(LimitTime != -1f)
        {
            LimitTime = time;
            Timer = LimitTime;
            type = false;
            return;
        }

        Icon = transform.FindChild(Symptom).gameObject;
        grid = GetComponent<UIGrid>();

        if(Icon == null) { return; }

        LimitTime = time;
        Timer = LimitTime;

        Icon.SetActive(true);
        grid.Reposition();
        
        StartCoroutine(C_Update());
        StartCoroutine(C_Flickering());
    }

    IEnumerator C_Update()
    {
        while(true)
        {
            Timer -= Time.deltaTime;

            if (Timer < LimitTime * 0.5f)
            {
                type = true;
            }

            if (Timer < 0)
            {
                type = false;
                Icon.SetActive(false);
                grid.Reposition();
                LimitTime = -1f;
                Timer = 0f;
                StopAllCoroutines();
            }
            yield return null;
        }
    }

    IEnumerator C_Flickering()
    {
        while (true)
        {
            if (type == false)
            {
                Icon.SetActive(true);
            }
            else
            {
                Icon.SetActive(false);
                yield return new WaitForSeconds(0.1f);
                Icon.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }
    }
}
