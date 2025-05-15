using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : MonoBehaviour
{
    [Header("Collision")]
    public Collider2D Collider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TestMonster 12 + monsterEnter");
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile item = collision.gameObject.GetComponent<Projectile>();
            item.SetDestoryed();
            item.OffRender();
            Debug.Log("TestMonster 18 + Hit");
        }
    }
}
