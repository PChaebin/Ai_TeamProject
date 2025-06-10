using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public List<int> passwordList;

    private GameObject closedDoorObj;
    private GameObject openedDoorObj;

    void Start()
    {
        ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        if (spawner != null)
        {
            passwordList = spawner.GetPasswordList();
        }
        else
        {
            Debug.LogError("DoorController: ���� ItemSpawner�� �����ϴ�.");
            passwordList = new List<int>();
        }

        Transform closedTrans = transform.Find("ClosedDoor");
        Transform openedTrans = transform.Find("OpenedDoor");

        if (closedTrans != null) closedDoorObj = closedTrans.gameObject;

        if (openedTrans != null) openedDoorObj = openedTrans.gameObject;

        if (closedDoorObj != null) closedDoorObj.SetActive(true);
        if (openedDoorObj != null) openedDoorObj.SetActive(false);
    }

    public void TryOpen(string inputPassword)
    {
        if (inputPassword.Length != 4)
        {
            Debug.LogWarning($"��й�ȣ�� 4�ڸ����� �մϴ�. (�Է� ����: {inputPassword.Length})");
            return;
        }

        List<int> inputDigits = new List<int>();
        foreach (char c in inputPassword)
        {
            if (!char.IsDigit(c))
            {
                Debug.LogWarning("���ڸ� �Է����ּ���.");
                return;
            }
            inputDigits.Add(c - '0');
        }

        List<int> correctDigits = new List<int>(passwordList);

        inputDigits.Sort();
        correctDigits.Sort();

        bool isMatch = inputDigits.SequenceEqual(correctDigits);

        if (isMatch)
        {
            if (closedDoorObj != null) closedDoorObj.SetActive(false);
            if (openedDoorObj != null) openedDoorObj.SetActive(true);
            Debug.Log("��й�ȣ ������ �¾ҽ��ϴ�! ���� �����ϴ�.");
        }
        else
        {
            Debug.LogWarning("��й�ȣ�� Ʋ�Ƚ��ϴ�.");
        }
    }
}
