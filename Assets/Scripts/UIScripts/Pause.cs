using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UIElements;

public class Pause : MonoBehaviour
{
    //Bottom Sheet Group
    private VisualElement _bottomContainer;
    //Open Button

    //Close Button
    private Button _closeButton;
    //Bottom Sheet
    private VisualElement _bottomSheet;
    //Scrim
    private VisualElement _scrim;

    // Pause input actions
    public InputActionReference pauseAction;
    public InputActionReference resumeAction;

    public GameObject pauseScreen;

    public bool paused = false;


    // Start is called before the first frame update
    void Start()
    {
        /*
        // Grab the topmost visual element in the UI Document
        var root = GetComponent<UIDocument>().rootVisualElement;


        //Close Button
        _closeButton = root.Q<Button>("Button_Close");

        //Registering buttons' callback methods
        _closeButton.RegisterCallback<ClickEvent>(OnCloseButtonClicked);
        */
    }

    void Update()
    {
        pauseGame();
    }

    private void pauseGame()
    {
        // If pause triggered
        if (pauseAction.action.triggered && paused == false)
        {
            // Then pause game
            this.gameObject.SetActive(true);
            Time.timeScale = 0f;
            paused = true;
            pauseScreen.SetActive(true);
        }
        // If pressed Q
        if (resumeAction.action.triggered && paused == true)
        {
            Time.timeScale = 1f;
            paused = false;
            pauseScreen.SetActive(false);
        }
    }

}
