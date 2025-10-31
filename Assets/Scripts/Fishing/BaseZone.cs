using UnityEngine;

public class BaseZone : MonoBehaviour
{
    // Total Fish stored
    public int totalFishStored = 0;

    public int fishThresholdForDialogue = 3;


    // Set up playerFishing script
    public FishingSystem playerFishing;
    public GameManager gameManager;
    // Assign next dialogue trigger
    public GameObject nextDialogueTrigger;

    private void Awake()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        // Only allow player to deposit
        if (other.CompareTag("Player"))
        {
            // Deposit fish via GameManager
            if (gameManager.fishCount > 0)
            {
                gameManager.DepositFish();
                totalFishStored += gameManager.fishCount;

                Debug.Log("Deposited fish! Total stored: " + totalFishStored);

                // Check if enough fish are deposited to enable next dialogue
                if (totalFishStored >= fishThresholdForDialogue && nextDialogueTrigger != null)
                {
                    // Enable the collider so the player can trigger the next dialogue
                    Collider col = nextDialogueTrigger.GetComponent<Collider>();
                    if (col != null)
                        col.enabled = true;

                    Debug.Log("Next dialogue unlocked!");
                }
            }
        }
    }
}