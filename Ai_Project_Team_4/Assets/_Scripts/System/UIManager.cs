using Cainos.PixelArtTopDown_Basic;
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
    public GameObject passwordUI;

    public GameObject item;
    public GameObject monster;
    public GameObject player;
    public ItemSpawner spawner;

    public Button startBtn;
    public Button retryBtn;
    public Button regameBtn;

    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(() => {
            TurnInventory();
        });

        retryBtn.onClick.AddListener(() => {
            TurnTitle();
        });

        regameBtn.onClick.AddListener(() =>{
            TurnTitle();
        });

        titleUI.SetActive(true);
        inventoryUI.SetActive(false);
        gameoverUI.SetActive(false);
        gameclearUI.SetActive(false);
        passwordUI.SetActive(false);
        monster.SetActive(false);
        player.SetActive(false);
    }


    public void TurnInventory()
    {
        Debug.Log("click!");
        titleUI.SetActive(false);
        inventoryUI.SetActive(true);
        gameoverUI.SetActive(false);
        gameclearUI.SetActive(false);
        passwordUI.SetActive(false);
        monster.SetActive(true);
        player.SetActive(true);
        monster.GetComponent<EnemyFSM>().StartGame();
        player.GetComponent<TopDownCharacterController>().SetMove(false);
        spawner.SetItems();
    }

    public void TurnTitle()
    {
        titleUI.SetActive(true);
        inventoryUI.SetActive(false);
        gameoverUI.SetActive(false);
        gameclearUI.SetActive(false);
        passwordUI.SetActive(false);
        monster.SetActive(false);
        player.SetActive(false);
        spawner.ClearItems();
    }

    public void TurnGameover()
    {
        titleUI.SetActive(false);
        inventoryUI.SetActive(false);
        gameoverUI.SetActive(true);
        gameclearUI.SetActive(false);
        passwordUI.SetActive(false);
        monster.GetComponent<EnemyFSM>().EndGame(); 
        player.GetComponent<TopDownCharacterController>().SetMove(true);
        player.SetActive(false);
    }

    public void TurnGameclear()
    {
        titleUI.SetActive(false);
        inventoryUI.SetActive(false);
        gameoverUI.SetActive(false);
        gameclearUI.SetActive(true);
        passwordUI.SetActive(false);
        monster.GetComponent<EnemyFSM>().EndGame();
        player.GetComponent<TopDownCharacterController>().SetMove(true);
    }

    public void SetItem() 
    { 
        item.SetActive(true);
    }

    public void UseItem()
    {
        item.SetActive(false);
    }
}
