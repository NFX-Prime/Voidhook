using UnityEngine;

public class BaseZone : MonoBehaviour
{
    // Total Fish stored
    public int totalFishStored = 0;

    // How many fish must be dropped off before unlocking dialogue
    public int fishThresholdForDialogue = 3;

    public FishingSystem playerFishing;
    public GameManager gameManager;

    // The dialogue trigger to unlock
    public GameObject nextDialogueTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int fishToDeposit = gameManager.fishCount;

            if (fishToDeposit > 0)
            {
                // Add to total BEFORE resetting count
                totalFishStored += fishToDeposit;

                Debug.Log($"Deposited {fishToDeposit} fish! Total stored: {totalFishStored}");

                // Now call deposit (which resets count)
                gameManager.DepositFish();

                // Unlock dialogue if threshold met
                if (totalFishStored >= fishThresholdForDialogue && nextDialogueTrigger != null)
                {
                    Collider col = nextDialogueTrigger.GetComponent<Collider>();
                    if (col != null && !col.enabled)
                    {
                        col.enabled = true;
                        Debug.Log("Next dialogue unlocked!");
                    }
                }
            }
            else
            {
                Debug.Log("No fish to deposit!");
            }
        }
    }
}