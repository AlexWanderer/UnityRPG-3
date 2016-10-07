using UnityEngine;
using System.Collections;

public class StateAction : MonoBehaviour {

    public float LimitTime = -1f;
   public float Timer = 0f;
   public bool type = false;
    UIPanel Icon = null;

   public void Start_Update(float time)
    {
        if(LimitTime != -1f)
        {
            LimitTime = time;
            Timer = LimitTime;
            type = false;
            return;
        }

        Icon = GetComponent<UIPanel>();

        if(Icon == null) { return; }

        LimitTime = time;
        Timer = LimitTime;

        Icon.alpha = 1;
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
                Icon.alpha = 0;
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
                Icon.alpha = 1;
            }
            else
            {
                Icon.alpha = 0;
                yield return new WaitForSeconds(0.1f);
                Icon.alpha = 1;
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }
    }
}
