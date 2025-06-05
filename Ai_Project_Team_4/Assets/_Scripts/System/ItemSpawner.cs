using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject password;
    public GameObject projectile;

    [Header("List")]
    public List<int> passwordList;
    public List<GameObject> projectileList;
    public List<Projectile> projectileScriptsList;

    [Header("count")]
    public int count = 0;

    public int passwordNum = 4;
    public int projectileNum = 5;

    // Start is called before the first frame update
    void Awake()
    {
        CreatePassword();
        SetProjectileList();
        StartCoroutine(SpawnProjectile(0f));
    }

    //void Update()
    //{
    //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        projectileScriptsList[count].Fire(this.transform.position, pos);
    //        count += 1;
    //        Debug.Log(count);
    //    }
    //    if(count >= 5)
    //    {
    //        count = 0;
    //    }
    //}

    public List<Projectile> GetProjectileListcs()
    {
        return projectileScriptsList;
    }

    public List<int> GetPasswordList()
    {
        return passwordList;
    }
    public void CallRespawn()
    {
        for (int i = 0; i < projectileNum; i++)
        {
            if (projectileScriptsList[i].GetIsDestoryed())
            {
                StartCoroutine(ReSpawnProjectile(3f, i));
            }
        }
    }

    public void CreatePassword()
    {
        for(int i = 0; i < passwordNum; i++)
        {
            float x = Random.Range(-10, 11);
            float y = Random.Range(-10, 11);
            Vector3 randomPos = new Vector3(x, y, -1);
            GameObject passwordInst = Instantiate(password, randomPos, password.transform.rotation);

            int num = Random.Range(0, 10);
            Password passwordScript = passwordInst.GetComponent<Password>();
            passwordScript.SetNum(num);

            passwordList.Add(num);
        }
    }
    public void SetProjectileList()
    {
        for (int i = 0; i < projectileNum; i++)
        {
            GameObject projectileInst = Instantiate(projectile, transform.position, transform.rotation);
            Projectile projectileScript = projectileInst.GetComponent<Projectile>();
            projectileScript.SetSpawner(this.GetComponent<ItemSpawner>());
            projectileScript.SetIndex(i);
            projectileScript.OffRender();
            projectileList.Add(projectileInst);
            projectileScriptsList.Add(projectileScript);
        }
    }

    IEnumerator SpawnProjectile(float time)
    {
        for (int i = 0; i < projectileNum; i++)
        {
            yield return new WaitForSeconds(time);

            float x = Random.Range(-10, 11);
            float y = Random.Range(-10, 11);
            Vector3 randomPos = new Vector3(x, y, 0);
            projectileList[i].transform.position = randomPos;
            projectileList[i].transform.up = Vector3.zero;

            projectileScriptsList[i].OnRender();
            projectileScriptsList[i].SetSaved();
        }
    }

    IEnumerator ReSpawnProjectile(float time, int i)
    {
       
        yield return new WaitForSeconds(time);

        float x = Random.Range(-10, 11);
        float y = Random.Range(-10, 11);
        Vector3 randomPos = new Vector3(x, y, 0);
        projectileList[i].transform.position = randomPos;
        projectileList[i].transform.up = Vector3.zero;

        projectileScriptsList[i].OnRender();
        projectileScriptsList[i].SetSaved();
       
    }
    public bool CheckAllProjectileLive()
    {
        for(int i = 0; i < projectileNum; i++)
        {
            if(projectileScriptsList[i].GetIsDestoryed())
            {
                return false;
            }
        }
        return true;
    }
}
