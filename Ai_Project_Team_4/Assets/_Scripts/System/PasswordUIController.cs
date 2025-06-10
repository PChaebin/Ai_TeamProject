using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordUIController : MonoBehaviour
{
    [Header("DoorController ����")]
    public DoorController doorController;

    [Header("InputField (Legacy UI) ����")]
    public TMP_InputField passwordInputField;

    public void OnSubmitPassword()
    {
        if (doorController == null || passwordInputField == null)
        {
            Debug.LogWarning("PasswordUIController: DoorController �Ǵ� InputField�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        string raw = passwordInputField.text.Trim();

        doorController.TryOpen(raw);
    }
}