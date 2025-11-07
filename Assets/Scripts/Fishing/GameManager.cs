using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BossAI boss;

    public int fishCount = 0;
    public int depositedFishCount = 0;
    [Header("UI Reference")]
    public TMP_Text fishCountText;
    public TMP_Text fishDepositedText;
    public TMP_Text suspicionText;
    public TMP_Text playerWin;

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
        UpdateDepositedFishUI();
        UpdateSuspicionUI();
    }

    private void Update()
    {
        
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

    /// <summary>
    /// Function to update deposited fish
    /// </summary>
    void UpdateDepositedFishUI()
    {
        if (fishDepositedText != null)
        {
            fishDepositedText.text = "Deposited Fish: " + depositedFishCount;
        }
    }
    /// <summary>
    /// function to update suspicion
    /// </summary>
    void UpdateSuspicionUI()
    {
        if (suspicionText != null)
        {
            suspicionText.text = "Suspicion: " + boss.suspicion;
        }
    }

    // Function is called by BaseZone when player deposits fish
    public void DepositFish()
    {
        // Depositing fish
        Debug.Log("DepositFish function activated");
        depositedFishCount = fishCount;
        fishCount = 0;

        // Updating the UIs
        UpdateFishUI();
        UpdateDepositedFishUI();

        // Check if player won!
        if (depositedFishCount == 5)
        {
            // Then player wins! Activate win UI.
            playerWin.text = "You Win!";

        }


    }



}