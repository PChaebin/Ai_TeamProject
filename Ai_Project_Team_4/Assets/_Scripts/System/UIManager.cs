using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject titleUI;
    public GameObject inventoryUI;
    public GameObject gameoverUI;
    public GameObject gameclearUI;

    public GameObject item;

    public Button startBtu;
    public Button retryBtu;

    // Start is called before the first frame update
    void Start()
    {
        titleUI.SetActive(true);
        inventoryUI.SetActive(false);
        gameoverUI.SetActive(false);
        gameclearUI.SetActive(false);

        startBtu.onClick.AddListener(() => {
            TurnInventory();
        });

        retryBtu.onClick.AddListener(() => {
            TurnTitle();
        });

    }

    private void Update()
    {
        if (inventoryUI.activeSelf)
        {
            if (Input.GetKeyUp(KeyCode.O))
            {
                TurnGameover();
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                StartCoroutine(TurnningTimer());
            }
            if (Input.GetKeyUp(KeyCode.I))
            {
                if(item.activeSelf)
                {
                    UseItem();
                    return;
                }
                SetItem();
            }
        }

    }

    public void TurnInventory()
    {
        titleUI.SetActive(false);
        inventoryUI.SetActive(true);
        gameoverUI.SetActive(false);
        gameclearUI.SetActive(false);
    }

    public void TurnTitle()
    {
        titleUI.SetActive(true);
        inventoryUI.SetActive(false);
        gameoverUI.SetActive(false);
        gameclearUI.SetActive(false);
    }

    public void TurnGameover()
    {
        titleUI.SetActive(false);
        inventoryUI.SetActive(false);
        gameoverUI.SetActive(true);
        gameclearUI.SetActive(false);
    }

    public void TurnGameclear()
    {
        titleUI.SetActive(false);
        inventoryUI.SetActive(false);
        gameoverUI.SetActive(false);
        gameclearUI.SetActive(true);
    }

    public void SetItem() 
    { 
        item.SetActive(true);
    }

    public void UseItem()
    {
        item.SetActive(false);
    }

    IEnumerator TurnningTimer()
    {
        TurnGameclear();
        yield return new WaitForSeconds(5f);
        TurnTitle();
    }
}
