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

    // Start is called before the first frame update
    void Start()
    {
        titleUI.SetActive(true);
        inventoryUI.SetActive(false);
        gameoverUI.SetActive(false);
        gameclearUI.SetActive(false);
        passwordUI.SetActive(false);
        monster.SetActive(false);
        player.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void TurnInventory()
    {
        titleUI.SetActive(false);
        inventoryUI.SetActive(true);
        UseItem();
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
        spawner.ClearItems();
        player.transform.position = new Vector3(0, 30, 0);
        monster.SetActive(false);
        player.SetActive(false);
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
