using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    // Set the videoplayer object (this holds the video itself)
    public VideoPlayer videoPlayer;
    // Set what scene goes after the video is played
    public string nextSceneName;
    // Assign canvas so that we can hide it when the video is playing
    public Canvas mainCanvas;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene("December 5th Build");    
        Debug.Log("Game Started");
    }

    // Called by the Start button
    public void PlayCutscene()
    {
        // Simply disabling the canvas when the game starts.
        if (mainCanvas != null)
        {
            mainCanvas.enabled = false;
        }

        // Make sure the VideoPlayer is active
        videoPlayer.gameObject.SetActive(true); 
        videoPlayer.Play();
        // Event when video ends
        videoPlayer.loopPointReached += OnVideoFinished; 
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Remove listener
        videoPlayer.loopPointReached -= OnVideoFinished;
        // Load game scene
        SceneManager.LoadScene(nextSceneName); 
    }

    public void Back2MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
    

}
