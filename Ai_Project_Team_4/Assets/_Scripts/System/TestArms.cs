using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestArms : MonoBehaviour
{
    [Header("Collision")]
    public Collider2D Collider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TestArms 12 + armEnter");
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile item = collision.gameObject.GetComponent<Projectile>();
            item.SetDestoryed();
            item.OffRender();
            Debug.Log("TestArms 18 + Protect");
        }
    }
}
