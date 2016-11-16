using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using JsonFx.Json;
using System;


public class LoginManager : MonoBehaviour {

    public UIInput Login_ID;
    public UIInput Login_PW;
    public UIInput Join_ID;
    public UIInput Join_PW;

	public void Set_Login()
    {
        if (Login_ID.value.Equals("아이디를 입력해주세요") || Login_PW.value.Equals("비밀번호를 입력해주세요"))
        {
            Debug.Log("계정  및 암호가 입력되지 않았습니다. 확인하고 다시 시도 하시기 바랍니다.");
            return;
        }

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "SetLogin");

        sendData.Add("user_id", Login_ID.value);
        sendData.Add("user_pw", Login_PW.value);
        sendData.Add("login_time", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"));

        Debug.Log(DateTime.Now.ToString());

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyLogin));
    }
    void ReplyLogin(string json)
    {
        // JSON Data 변환
        RecvPlayerInfo data = JsonReader.Deserialize<RecvPlayerInfo>(json);

        if (data == null) { return; }


        GameManager.Get_Inctance().Set_PlayerInfo(data.Level, data.Exp, data.Gold, data.User_Index);

        // 회원 가입에 성공 했으므로 바로 로그인을 시도한다.
        StartCoroutine(Login());
    }
    IEnumerator Login()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("Main");

        while (!async.isDone)
        {
            yield return null;
        }
        yield return null;
    }

    public void Set_Join()
    {
        if (Join_ID.value.Equals("아이디를 입력해주세요") || Join_PW.value.Equals("비밀번호를 입력해주세요"))
        {
            Debug.Log("계정  및 암호가 입력되지 않았습니다. 확인하고 다시 시도 하시기 바랍니다.");
            return;
        }

        if (Join_ID.value.Length < 2 || Join_PW.value.Length < 1)
        {
            Debug.Log("계정과 암호는 4글자 이상으로 만들어야 합니다. 확인하고 다시 시도 하시기 바랍니다.");
            return;
        }

        Dictionary<string, object> sendData = new Dictionary<string, object>();
        sendData.Add("contents", "SetJoin");

        sendData.Add("user_id", Join_ID.value);
        sendData.Add("user_pw", Join_PW.value);
        sendData.Add("join_time", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"));

        Debug.Log(DateTime.Now.ToString());

        StartCoroutine(NetworkManager.Instance.ProcessNetwork(sendData, ReplyJoin));
    }
    void ReplyJoin(string json)
    {
        // JSON Data 변환
        RecvPlayerInfo data = JsonReader.Deserialize<RecvPlayerInfo>(json);

        if(data == null) { return; }

        GameManager.Get_Inctance().Set_PlayerInfo(data.Level, data.Exp, data.Gold, data.User_Index);

        // 회원 가입에 성공 했으므로 바로 로그인을 시도한다.
        StartCoroutine(Login());
    }

}

public class RecvPlayerInfo
{
    //  public string Charaters;
    ////    public string Items;
    //   public string Charater_Equipments;
    public int User_Index;
    public string ID;
    public int Level;
    public float Exp;
    public float Gold;
}