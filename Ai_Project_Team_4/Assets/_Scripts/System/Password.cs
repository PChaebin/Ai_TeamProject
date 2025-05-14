using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Password : MonoBehaviour
{
    public TextMeshPro num;

    public void SetNum(int passwordNum)
    {
        num.text = passwordNum.ToString();
    }
}
