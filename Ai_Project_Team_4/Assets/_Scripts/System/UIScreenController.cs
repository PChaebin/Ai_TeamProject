using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class UIScreenController : MonoBehaviour
{
    [Header("UXML 에셋 연결")]
    [SerializeField] private UIDocument uiDocument;       
    [SerializeField] private VisualTreeAsset titleUXML;    
    [SerializeField] private VisualTreeAsset inventoryUXML; 
    [SerializeField] private VisualTreeAsset gameOVerUXML;    

    private VisualElement titleScreen;
    private VisualElement inventoryScreen;
    private VisualElement gameOVerScreen;
    private VisualElement root;

    public bool playerDead = false;

    void Awake()
    {
        root = uiDocument.rootVisualElement;

        titleScreen = titleUXML.CloneTree();
        inventoryScreen = inventoryUXML.CloneTree();
        gameOVerScreen = gameOVerUXML.CloneTree();

        root.Add(titleScreen);

        var buttonPlay = titleScreen.Q<Button>("Play");
        buttonPlay.clicked += OnButtonPlayClicked;
    }

    private void Update()
    {
        if(playerDead)
        {
            SwitchToOver();
            playerDead = false;
        }
    }

    private void OnButtonPlayClicked()
    {
        root.Clear();                  
        root.Add(inventoryScreen);       

    }

    private void SwitchToOver()
    {
        root.Clear();
        root.Add(gameOVerScreen);

        var buttonGO = gameOVerScreen.Q<Button>("Go");
        buttonGO.clicked += OnButtonGOClicked;
    }

    private void OnButtonGOClicked()
    {
        root.Clear();
        root.Add(titleScreen);
    }
}
