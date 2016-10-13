using UnityEngine;
using System.Collections;

public class PlayerSkill_Manager : MonoBehaviour {

    public GameObject[] Skills = null;
    public UISprite[] Gauges = null;

    private static PlayerSkill_Manager instance = null;

    // Use this for initialization
    void Awake()
    {
        instance = this;

        Gauges = new UISprite[Skills.Length];

        for(int i = 0; i < Skills.Length; i++)
        {
            Gauges[i] = Skills[i].transform.FindChild("Sprite_SkillGauge").GetComponent<UISprite>();
        }

        StartCoroutine(C_Update());

    }
    public static PlayerSkill_Manager Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(PlayerSkill_Manager)) as PlayerSkill_Manager;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("PlayerSkill_Manager ");
            instance = obj.AddComponent(typeof(PlayerSkill_Manager)) as PlayerSkill_Manager;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

    public void Set_Skill(PlayerAction player, string ParentName)
    {
        int index = -1;

        switch (ParentName)
        {
            case "Center": { index = 0; break; }
            case "Sub1": { index = 1; break; }
            case "Sub2": { index = 2; break; }
        }

        if (index == -1) { return; }

        // 이벤트 등록 및 파라메타 추가
        EventDelegate onClick = new EventDelegate(player, "Special_Skill");
        EventDelegate.Add(Skills[index].GetComponent<UIButton>().onClick, onClick);

        UISprite Icon = Skills[index].transform.FindChild("Sprite_CharaIcon").GetComponent<UISprite>();
        Icon.spriteName = player.name;

    }


    IEnumerator C_Update()
    {
        while(true)
        {
            for (int i = 0; i < Skills.Length; i++)
            {
                GameObject PlayerObj = PlayerManager.Get_Inctance().Characters[i];
                if(PlayerObj == null) { continue; }

                PlayerAction Player = PlayerObj.GetComponent<PlayerAction>();

                float value = Player.SkillPoint / Player.InitSkillPoint;

                Gauges[i].fillAmount = value;
            }

            yield return null;
        }
    }

}
