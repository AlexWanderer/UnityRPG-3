using UnityEngine;
using System.Collections;

public class UI_StateManager : MonoBehaviour {

    public GameObject[] Icons = null;

	// Use this for initialization
	void Awake ()
    {
        Icons = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Icons[i] = transform.GetChild(i).gameObject;
            Icons[i].SetActive(false);
        }
    }

    public void Start_State(string Symptom, float time)
    {
        for(int i = 0; i < Icons.Length; i++)
        {
            if(Icons[i].name.Contains(Symptom))
            {
                Icons[i].GetComponent<StateIconAction>().Start_Icon(time, GetComponent<UIGrid>());
                return;
            }
        }
    }


}
