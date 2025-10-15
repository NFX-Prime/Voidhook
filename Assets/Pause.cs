using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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



    // Start is called before the first frame update
    void Start()
    {
        //Grab the topmost visual element in the UI Document
        var root = GetComponent<UIDocument>().rootVisualElement;


        //Close Button
        _closeButton = root.Q<Button>("Button_Close");

        //Registering buttons' callback methods
        _closeButton.RegisterCallback<ClickEvent>(OnCloseButtonClicked);
    }

    void Update()
    {

    }

    private void OnCloseButtonClicked(ClickEvent evt)
    {
        this.gameObject.SetActive(false);
    }
    
    private void pauseGame()
    {
        this.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

}
