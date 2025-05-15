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
    [Header("spawn position")]
    public GameObject spawnPos;

    private void Start()
    {
        projectiles = FindObjectOfType<ItemSpawner>().GetProjectileListcs();
        getting = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (getting && Input.GetMouseButtonUp(0))
        {
            projectiles[itemIndex].Fire(spawnPos.transform.position, pos);
            StartCoroutine(fireCoolTimer());
            Debug.Log("shooting : " +itemIndex);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!getting && collision.gameObject.CompareTag("Projectile"))
        {
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
