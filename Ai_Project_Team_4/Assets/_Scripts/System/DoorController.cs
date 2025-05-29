using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorController : MonoBehaviour
{
    public List<int> passwordList;

    void Start()
    {
        ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        if (spawner != null)
        {
            passwordList = spawner.GetPasswordList();
        }
        else
        {
            Debug.LogError("�����ʰ� �����ϴ�.");
        }
    }
}
