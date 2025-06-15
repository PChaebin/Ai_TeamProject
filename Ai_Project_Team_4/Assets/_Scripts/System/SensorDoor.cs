using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorDoor : MonoBehaviour
{
    public GameObject passwordUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            passwordUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            passwordUI.SetActive(false);
        }
    }
}
