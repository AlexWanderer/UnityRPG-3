using UnityEngine;
using System.Collections;

public class EventOBJ_Action : MonoBehaviour
{

    public GameObject EventOBJButton = null;

    void ButtonPosition_Setting()
    {
        Vector3 Position = Camera.main.WorldToViewportPoint(transform.position);
        Position.z = 0;
        Position.y += 0.35f;
        EventOBJButton.transform.position = UICamera.mainCamera.ViewportToWorldPoint(Position);
    }

    void OnTriggerStay(Collider obj)
    {
        if (obj.transform.CompareTag("Player"))
        {
            EventOBJButton.SetActive(true);
            ButtonPosition_Setting();
        }
    }
    void OnTriggerExit(Collider obj)
    {
        if (obj.transform.CompareTag("Player"))
        {
            EventOBJButton.SetActive(false);
        }
    }
}
