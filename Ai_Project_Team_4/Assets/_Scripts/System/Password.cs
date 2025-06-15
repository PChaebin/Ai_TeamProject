using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Password : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public SpriteRenderer current;

    public void SetNum(int passwordNum)
    {
        current.sprite = sprites[passwordNum];
    }

    public void dest()
    {
        Destroy(this.gameObject);
    }
}
