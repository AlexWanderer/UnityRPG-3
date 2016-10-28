using UnityEngine;
using System.Collections;

public class Base_WindowAction : MonoBehaviour {

    public void Set_CloseButton()
    {
        gameObject.SetActive(false);
        return;
    }
    public void Set_Window()
    {
        gameObject.SetActive(true);
    }
}
