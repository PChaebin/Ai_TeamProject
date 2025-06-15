using Cainos.PixelArtTopDown_Basic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("Item")]
    public List<Projectile> projectiles;
    [Header("Index")]
    public int itemIndex;
    [Header("Getting")]
    public bool getting;

    [Header("UI")]
    public UIManager manager;
    [Header("controller")]
    public TopDownCharacterController characterController;

    private void Start()
    {
        projectiles = FindObjectOfType<ItemSpawner>().GetProjectileListcs();
        getting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(characterController.GetNotMove())
        {
            return;
        }
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (getting && Input.GetMouseButtonUp(0))
        {
            manager.UseItem();
            projectiles[itemIndex].Fire(transform.position, pos);
            StartCoroutine(fireCoolTimer());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!getting && collision.gameObject.CompareTag("Projectile"))
        {
            manager.SetItem();
            Projectile item = collision.gameObject.GetComponent<Projectile>();
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (i == item.GetIndex())
                {
                    itemIndex = i;
                    getting = true;
                    item.OffRender();
                    break;
                }
            }
        }
    }

    IEnumerator fireCoolTimer()
    {
        yield return new WaitForSeconds(1.2f);
        getting = false;
    }
}
