using UnityEngine;
using System.Collections;

public class Support_Magica : Support_Action {

    public GameObject SkillEffect_Prefab = null;

    void Start()
    {
        // 임시 멘트
        Ment = new string[] { "와↗가↘나→와~~\n메구밍!!!", "연어는 최고죠!!\n연어연어연어!!", "Go!\n힘차게 갑시다!" , "취업하고싶다!", "영웅은 죽지 않아요.", "도와줘요 코딩요정!", "코딩의 요정은\n컴파일러의 복수를 하러왔어." };

        Skill_Damage = 30f;

        StartCoroutine(C_Update());
    }

    IEnumerator C_Update()
    {
        while (true)
        {
            Support_MentLabel.transform.parent.gameObject.SetActive(false);

            float R = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(R);

            int ViewTime = Random.Range(0, Ment.Length);
            Support_MentLabel.text = Ment[ViewTime];
            Support_MentLabel.transform.parent.gameObject.SetActive(true);
           
            yield return new WaitForSeconds(1.3f);
        }
    }

    public override void Support_Skill()
    {
       if( Skill_Use == true) { return; }

       if(PlayerManager.Get_Inctance().Check_StateAttack() == false) { return; }

        Skill_Use = true;

        StartCoroutine(C_Support_Skill());
    }
    IEnumerator C_Support_Skill()
    {
        Vector3 Position = Vector3.zero;
        Position = MonsterManager.Get_Inctance().Get_AliveMonster().transform.position;
        Position.y = 10f;

        GameObject Effect = Instantiate(SkillEffect_Prefab);
        Effect.transform.position = Position;

        float Check_y = Effect.transform.position.y;
        while (Check_y > 0.8f)
        {
            Check_y = Effect.transform.position.y;

            Effect.transform.Translate(Vector3.down * Time.deltaTime * 5f);

            yield return null;
        }

        Effect.transform.FindChild("Default").gameObject.SetActive(false);
        Effect.transform.FindChild("Explosion").gameObject.SetActive(true);

        float time = 1f;
        Camera PlayerCamera = PlayerManager.Get_Inctance().GetComponentInChildren<Camera>();
        Vector3 StandPos = PlayerCamera.transform.position;
        while(time > 0f)
        {
            time -= Time.deltaTime * 3f;

            Vector3 position = StandPos;

            float x = Random.Range(-1, 1);
            float y = Random.Range(-1, 1);
            float z = Random.Range(-1, 1);

            position += new Vector3(x, y, z);

            PlayerCamera.transform.position = position;

            yield return new WaitForSeconds(0.02f);
        }

        PlayerCamera.transform.position = StandPos;

        MonsterManager.Get_Inctance().Set_AllMonsterDamage(Skill_Damage);

        yield return new WaitForSeconds(0.5f);

        Effect.SetActive(false);
        yield break;
    }
}
