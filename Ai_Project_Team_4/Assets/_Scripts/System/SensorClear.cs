using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorClear : MonoBehaviour
{
    public GameObject gameclearUI;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameclearUI.GetComponent<UIManager>().TurnGameclear();
        }
    }
}
