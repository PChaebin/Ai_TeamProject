using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestArms : MonoBehaviour
{
    [Header("Collision")]
    [SerializeField]
    private Collider2D Collider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile") && collision.gameObject.GetComponent<Projectile>().GetIsShooting())
        {
            Projectile item = collision.gameObject.GetComponent<Projectile>();
            item.SetDestoryed();
            item.OffRender();
            Debug.Log("Protect!!");
        }
    }
}
