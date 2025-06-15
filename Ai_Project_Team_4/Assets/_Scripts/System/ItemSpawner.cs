using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSpawner : MonoBehaviour
{
    public GameObject password;
    public GameObject projectile;

    [Header("List")]
    public List<int> passwordList;
    public List<GameObject> passwordObjList;
    public List<GameObject> projectileList;
    public List<Projectile> projectileScriptsList;

    [Header("count")]
    public int count = 0;

    public int passwordNum = 4;
    public int projectileNum = 5;

    public List<int> passX = new List<int>() { -5, 0, 5, 10, 15 };
    public List<int> passY = new List<int>() { -5, 0, 5, 10, 15, 20, 25, 30 };
    public List<int> itemX = new List<int>() { -10, -5, 0, 5, 10, 15, 20 };
    public List<int> itemY = new List<int>() { -15, -10, -5, 0, 5, 10, 15, 20 };

    public void SetItems()
    {
        CreatePassword();
        SetProjectileList();
        StartCoroutine(SpawnProjectile(0f));
    }

    public void ClearItems()
    {
        for(int i =0; i < passwordList.Count; i++)
        {
            Destroy(passwordObjList[i]);
        }
        passwordList.Clear();
        for(int j = 0; j < projectileList.Count; j++)
        {
            Destroy(projectileList[j]);
            Destroy(projectileScriptsList[j]);
        }
        projectileList.Clear();
        projectileScriptsList.Clear();
    }
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
        for (int i = 0; i < passwordNum; i++)
        {
            int xind = Random.Range(0, passX.Count);
            int yind =Random.Range(0, passY.Count);
            float x = passX[xind];
            float y = passY[yind];
            Vector3 randomPos = new Vector3(x, y, -1);
            GameObject passwordInst = Instantiate(password, randomPos, password.transform.rotation);

            int num = Random.Range(0, 10);
            Password passwordScript = passwordInst.GetComponent<Password>();
            passwordScript.SetNum(num);

            passwordObjList.Add(passwordInst);
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

            int xind = Random.Range(0, itemX.Count);
            int yind = Random.Range(0, itemY.Count);
            float x = itemX[xind];
            float y = itemY[yind];

            Vector3 randomPos = new Vector3(x, y, 0);
            projectileList[i].transform.position = randomPos;
            projectileList[i].transform.up = Vector3.zero;

            projectileScriptsList[i].OnRender();
            projectileScriptsList[i].SetSaved();
            projectileScriptsList[i].SetStop();
        }
    }

    IEnumerator ReSpawnProjectile(float time, int i)
    {
       
        yield return new WaitForSeconds(time);

        int xind = Random.Range(0, itemX.Count);
        int yind = Random.Range(0, passY.Count);
        float x = itemX[xind];
        float y = itemY[yind];

        Vector3 randomPos = new Vector3(x, y, 0);
        projectileList[i].transform.position = randomPos;
        projectileList[i].transform.up = Vector3.zero;

        projectileScriptsList[i].OnRender();
        projectileScriptsList[i].SetSaved();
        projectileScriptsList[i].SetStop();
       
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
