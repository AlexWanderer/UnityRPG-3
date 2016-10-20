using UnityEngine;
using System.Collections;

public class BossHpAction : MonoBehaviour {

    public GameObject Target = null;
    public string name = "";
    public UISlider Gauge = null;

   public void Set_Start()
    {
        GetComponentInChildren<UILabel>().text = name;

        Gauge = GetComponentInChildren<UISlider>();
        StartCoroutine(C_Update());
    }

    IEnumerator C_Update()
    {
        while (true)
        {
            // Hpbar의 대상이 죽거나 모종의 이유로 active가 false가 됬을경우 Hpbar의 active를 false한다.
            if (Target.GetComponent<MonsterAction>().Check_Dead())
            {
                gameObject.SetActive(false);
                yield break;
            }

            float value = 0f;


            MonsterAction data = Target.GetComponent<MonsterAction>();
            value = data.Get_HP() / data.InitHP;

            Gauge.value = value;

            yield return null;
        }
    }


}
