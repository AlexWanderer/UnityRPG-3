using UnityEngine;
using System.Collections;

// Hpbar 오브젝트에 대한 스크립트.
public class HpbarAction : MonoBehaviour {

   public GameObject Target = null;                                     // 해당 Hpbar의 HP 정보를 가지고있는 Object 
    public UISprite Gauge = null;                                           // Hpbar에 있는 Gauge

    // Gauge 변수의 초기값을 설정하고 C_Update를 실행한다.
    public void Start_Update()
    {
        Gauge = transform.FindChild("Sprite_HP").GetComponent<UISprite>();
        StartCoroutine(C_Update());
    }
    

    IEnumerator C_Update()
    {
        while(true)
        {
            // 만약 Hpbar의 대상이 없으면 Hpbar의 Active를 false한다.
            if (Target == null)
            {
                gameObject.SetActive(false);
                yield break;
            }

            // Hpbar의 대상이 죽거나 모종의 이유로 active가 false가 됬을경우 Hpbar의 active를 false한다.
            if (Target.activeSelf == false)
            {
                gameObject.SetActive(false);
                yield break;
            }

            // Hp / Hp초기값을 담을 변수
            float value = 0f;

            //Target에 해당하는 스크립트를 가져와 value값을 준다. 
            //만약 Target이 잘못된경우 Log를 찍고 코루틴을 종료한다.
            if (Target.CompareTag("Monster"))
            {
                MonsterAction data = Target.GetComponent<MonsterAction>();
                value = data.Get_HP() / data.InitHP;
            }
            else if (Target.CompareTag("Player"))
            {
                PlayerAction data = Target.GetComponent<PlayerAction>();
                value = data.Get_HP() / data.Init_Infomation.Hp;
            }
            else
            {
                Debug.Log("잘못된 Target입니다!");
                yield break;
            }

            //Gauge의 길이를 value에 맞게 설정한다.
            Gauge.fillAmount = value;

            //만약 Gauge가 0이하일경우 0으로 고정한다.
            if (Gauge.fillAmount <= 0)
                Gauge.fillAmount = 0;

            //Target의 월드상위치를 Camera의 뷰위치로 변환한다.
            if(Camera.main == null)
            {
                yield return new WaitForSeconds(1.0f);
            }

            Vector3 p = Camera.main.WorldToViewportPoint(Target.transform.position);
            //Taget의 뷰위치를 UICamera의 뷰위치로 변환후 Hpbar의 위치로 할당한다.
            this.transform.position = UICamera.mainCamera.ViewportToWorldPoint(p);

            p = this.transform.localPosition;
            // RoundToInt(value) == value값과 가까운 정수를 반환한다.
            p.x = Mathf.RoundToInt(p.x);
            //실제 오브젝트보다 Hpbar가 위에 있어야하기때문에 y값을 추가한다.
            p.y = Mathf.RoundToInt(p.y) + 200f;
            p.z = 0f;
            this.transform.localPosition = p;


            yield return null;

        }
    }
}
