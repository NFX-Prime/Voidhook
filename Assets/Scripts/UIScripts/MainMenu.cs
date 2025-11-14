using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void StartGame()
    {
        // Load the main game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("November 15th Build");    
        Debug.Log("Game Started");
    }
}
