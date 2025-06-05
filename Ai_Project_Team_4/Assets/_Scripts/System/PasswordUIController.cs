using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordUIController : MonoBehaviour
{
    [Header("DoorController 참조")]
    public DoorController doorController;

    [Header("InputField (Legacy UI) 참조")]
    public TMP_InputField passwordInputField;

    public void OnSubmitPassword()
    {
        if (doorController == null || passwordInputField == null)
        {
            Debug.LogWarning("PasswordUIController: DoorController 또는 InputField가 할당되지 않았습니다.");
            return;
        }

        string raw = passwordInputField.text.Trim();

        doorController.TryOpen(raw);
    }
}