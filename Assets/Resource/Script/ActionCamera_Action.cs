using UnityEngine;
using System.Collections;

// Skill부분의 Camera를 담당하는 스크립트.
public class ActionCamera_Action : MonoBehaviour {

    Animator ani = null;
    public GameObject MainCamera = null;                        // Main Camera ( Active false될때가 있어서 )

    private static ActionCamera_Action instance = null;

    // Use this for initialization
    void Awake()
    {
        instance = this;

        ani = GetComponent<Animator>();

    }
    public static ActionCamera_Action Get_Inctance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(ActionCamera_Action)) as ActionCamera_Action;
        }

        if (null == instance)
        {
            GameObject obj = new GameObject("ActionCamera ");
            instance = obj.AddComponent(typeof(ActionCamera_Action)) as ActionCamera_Action;

            Debug.Log("Fail to get Manager Instance");
        }
        return instance;
    }

    // type == Player에 따라서 Camera의 애니메이션을 실행시킨다.
    public void Set_preparation(Transform Player, string type)
    {

        Vector3 pos = Player.position;

        switch (type)
        {
            case "Warrior":
                {
                    pos.y = 0.7f;
                    pos.z += 12f;
                    break;
                }

            case "Wizard":
                {
                    pos.y = 0.7f;
                    pos.z += 13f;
                    break;
                }

            case "Wizard2":
                {
                    GameObject Camera = transform.FindChild("UpDownCamera").gameObject;
                    Vector3 TargetPos = Player.GetComponent<PlayerAction>().Target.transform.position;
                    TargetPos.y += 1f;
                    TargetPos.z -= 5f;
                    Camera.transform.position = TargetPos;
                    Camera.transform.localRotation = Player.rotation;
                    ani.SetTrigger(type);
                    return;
                }

            case "Nurse":
                {
                    pos.y = 0.7f;
                    pos.z += 12f;
                    break;
                }
        }

        MainCamera.SetActive(false);
        transform.position = pos;
        ani.SetTrigger(type);
    }

    public void Stop_Animation()
    {
        ani.speed = 0f;
    }
    public void Play_Animation()
    {
        ani.speed = 1f;
    }
    
    // Main Camera를 활성화하고 모든 카메라를 끈 후 IDLE 상태로 바꾼다.
    public void CameraOff()
    {
        MainCamera.SetActive(true);

        Camera[] Cameras = GetComponentsInChildren<Camera>();

        for(int i =0; i < Cameras.Length; i++)
        {
            Cameras[i].enabled = false;
        }
        ani.SetTrigger("Idle");
    }
}
