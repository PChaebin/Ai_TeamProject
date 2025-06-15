using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorOver : MonoBehaviour
{
    public GameObject gameoverUI;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameoverUI.GetComponent<UIManager>().TurnGameover();
        }
    }
}
