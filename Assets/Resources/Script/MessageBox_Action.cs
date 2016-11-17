using UnityEngine;
using System.Collections;

public class MessageBox_Action : MonoBehaviour {

    public UILabel Message;

    public void Set_Message(string message)
    {
        Message.text = message;
        gameObject.SetActive(true);
    }
}
