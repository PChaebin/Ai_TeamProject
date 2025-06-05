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
            Debug.LogError("DoorController: 씬에 ItemSpawner가 없습니다.");
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
            Debug.LogWarning($"비밀번호는 4자리여야 합니다. (입력 길이: {inputPassword.Length})");
            return;
        }

        List<int> inputDigits = new List<int>();
        foreach (char c in inputPassword)
        {
            if (!char.IsDigit(c))
            {
                Debug.LogWarning("숫자만 입력해주세요.");
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
            Debug.Log("비밀번호 조합이 맞았습니다! 문이 열립니다.");
        }
        else
        {
            Debug.LogWarning("비밀번호가 틀렸습니다.");
        }
    }
}
