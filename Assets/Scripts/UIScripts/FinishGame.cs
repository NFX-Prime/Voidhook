using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishZone : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Number of fish required to finish the level")]
    public int requiredFish = 5;

    [Tooltip("Name of the scene to load when finished")]
    public string finishSceneName = "FinishScene";

    private void OnTriggerEnter(Collider other)
    {
        // Only react if the player enters
        if (!other.CompareTag("Player"))
            return;

        // Check player's fish count
        if (GameManager.Instance.depositedFishCount >= requiredFish)
        {
            SceneManager.LoadScene(finishSceneName);
        }
        else
        {
            Debug.Log("Player doesn't have enough fish yet");
        }
    }
}