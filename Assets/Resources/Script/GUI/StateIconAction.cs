using UnityEngine;
using System.Collections;

public class StateIconAction : MonoBehaviour {

    public float LimitTime = -1f;
    public float Timer = 0f;
    public bool type = false;
    public UIGrid grid = null;
    UISprite Icon = null;

    public void Start_Icon(float time, UIGrid ParentGrid)
    {
        grid = ParentGrid;
        Icon = GetComponent<UISprite>();

        LimitTime = time;
        Timer = LimitTime;
        type = false;
        gameObject.SetActive(true);
        grid.repositionNow = true;

        StartCoroutine(C_Update());
        StartCoroutine(C_Flickering());

        
    }

    IEnumerator C_Update()
    {
        while (true)
        {
            Timer -= Time.deltaTime;

            if (Timer < LimitTime * 0.5f)
            {
                type = true;
            }

            if (Timer < 0)
            {
                type = false;
                gameObject.SetActive(false);
                grid.repositionNow = true;
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
                Icon.enabled = true;
            }
            else
            {
                Icon.enabled = false;
                yield return new WaitForSeconds(0.1f);
                Icon.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }
    }
}
