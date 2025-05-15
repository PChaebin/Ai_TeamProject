using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //해야할 것 -> 플레이어가 몇번째 발사체와 부딪혔는지 제공해주는 함수 작성

    [Header("Velocity")]
    public float speed = 5f;

    [Header("ItemSpawner")]
    public ItemSpawner spawner;
    [Header("Index")]
    public int index;

    [Header("isDestoryed")]
    public bool isDestoryed = false;

    public Rigidbody2D projectileRb;
    public SpriteRenderer projectileSprite;
    public Collider2D projectileCol;

    public void SetSpawner(ItemSpawner spawnerScript)
    {
        spawner = spawnerScript;
    }

    public void SetIndex(int indexNum)
    {
        index = indexNum;
    }

    public int GetIndex() 
    { 
        return index; 
    }
    public bool GetIsDestoryed()
    {
        return isDestoryed;
    }

    public void SetDestoryed()
    {
        isDestoryed = true;
    }

    public void SetSaved()
    {
        isDestoryed = false;
    }

    public void CheckRespawn()
    {
        if (spawner.CheckAllProjectileDestory())
        {
            spawner.CallRespawn();
        }
    }

    public void Fire(Vector2 playerPos, Vector2 mousePos)
    {
        this.transform.position = playerPos;
        OnRender();
        Vector2 direction = (mousePos - playerPos).normalized;
        this.transform.up = direction;
        projectileRb.velocity = direction * speed;
        StartCoroutine(DestoryTimer());
    }

    IEnumerator DestoryTimer()
    {
        yield return new WaitForSeconds(1.2f);
        if(!GetIsDestoryed())
        {
            Debug.Log("Projectile 77 + destory");
            SetDestoryed();
            OffRender();
        }
        CheckRespawn();
        Debug.Log("Projectile 82 + destory");
    }

    public void OnRender()
    {
        projectileSprite.enabled = true;
        projectileCol.enabled = true;
    }

    public void OffRender()
    {
        projectileRb.velocity = Vector2.zero;
        projectileCol.enabled = false;
        projectileSprite.enabled = false;
    }

}
