using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DeathManager : MonoBehaviour
{
    public static DeathManager Instance;

    [Header("Player Settings")]
    public Transform player;
    public Transform checkpoint;

    [Header("UI")]
    public GameObject deathScreen;

    private float sceneStartTime;
    private float lastDeathTime;

    private string logFilePath;

    // Preventing double kills at the same time.
    private bool isDead = false;

    // Building folder/file to allow for saving file.
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Save next to the .exe (easy access)
        string buildFolder = Directory.GetParent(Application.dataPath).FullName;
        logFilePath = Path.Combine(buildFolder, "death_log.txt");

        sceneStartTime = Time.time;
        lastDeathTime = sceneStartTime;

        if (deathScreen != null)
            deathScreen.SetActive(false);

        // Setting is dead to false just incase
        isDead = false;
    }

    // Call from anywhere (USING DeathManager.Instance.KillPlayer();)
    public void KillPlayer()
    {

        if (isDead == true)
        {
            // Then ignore this since the player is already dead.
            return;
        }
        // Update GameManager stats
        GameManager.Instance.playerDies();

        // Getting stats to print out in debug
        int fish = GameManager.Instance.fishCount;
        int totalDeaths = GameManager.Instance.playerDeathCount;

        Vector3 pos = player.position;
        float timeSinceSceneStart = Time.time - sceneStartTime;
        float timeSinceLastDeath = Time.time - lastDeathTime;
        lastDeathTime = Time.time;

        Debug.Log($"Player died >>> Fish:{fish}, Deaths:{totalDeaths}, Pos:{pos}, Time:{timeSinceSceneStart}");

        // Saving it
        SaveDeathLog(totalDeaths, fish, pos, timeSinceSceneStart);

        // Show death UI
        deathScreen.SetActive(true);

        // Pause world
        Time.timeScale = 0f;
        
        // Setting isdead to true
        isDead = true;
    }

    /// <summary>
    /// Mehtod that restarts at given checkpoint. WE WILL SET THE CHECKPOINTS LATER. FOR NOW JUST MAKE
    /// IT THE START OF LEVEL 1.
    /// </summary>
    public void RestartAtCheckpoint()
    {

        // Hide UI
        deathScreen.SetActive(false);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Respawn player
        player.position = checkpoint.position;

        // Re-enabling controller after movement was done
        if (cc != null) cc.enabled = true;

        // Reduce fish count (2 fish penalty)
        GameManager.Instance.fishCount =
            Mathf.Max(0, GameManager.Instance.fishCount - 2);

        // Setting is dead to false;
        isDead = false;

        // Update UI via GameManager
        Time.timeScale = 1f;

    }

    /// <summary>
    /// Method that can call exiting to level (attach this to retry button)
    /// </summary>
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    /// <summary>
    /// Method that saves death stuff onto file
    /// </summary>
    /// <param name="deathNumber"></param>
    /// <param name="fish"></param>
    /// <param name="pos"></param>
    /// <param name="time"></param>
    private void SaveDeathLog(int deathNumber, int fish, Vector3 pos, float time)
    {
        string line =
            $"{deathNumber}, {fish}, {pos.x:F2}, {pos.y:F2}, {pos.z:F2}, {time:F2}";

        File.AppendAllText(logFilePath, line + "\n");

        Debug.Log($"Death log saved to: {logFilePath}");
    }
}