using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int fishCount = 0;
    [Header("UI Reference")]
    public TMP_Text fishCountText;

    private void Awake()
    {
        // Singleton pattern for easy access
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateFishUI();
    }

    public void AddFish()
    {
        fishCount++;
        UpdateFishUI();
    }

    void UpdateFishUI()
    {
        if (fishCountText != null)
            fishCountText.text = "Fish: " + fishCount;
    }
}