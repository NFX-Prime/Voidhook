using UnityEngine;

public class BaseZone : MonoBehaviour
{
    // Total Fish stored
    public int totalFishStored = 0;

    // How many fish must be dropped off before unlocking dialogue
    public int fishThresholdForDialogue = 3;

    // Set up playerFishing script
    public FishingSystem playerFishing;
    public GameManager gameManager;

    // The dialogue trigger to unlock (either disabled GameObject or disabled Collider)
    public GameObject nextDialogueTrigger;

    private void OnTriggerEnter(Collider other)
    {
        // Only allow player to deposit
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

                // Check if threshold reached and unlock dialogue
                if (totalFishStored >= fishThresholdForDialogue && nextDialogueTrigger != null)
                {
                    // Check if the GameObject is inactive in hierarchy
                    if (!nextDialogueTrigger.activeSelf)
                    {
                        nextDialogueTrigger.SetActive(true);
                        Debug.Log("Next dialogue trigger GameObject activated!");
                    }
                    else
                    {
                        // If the GameObject is already active, try enabling its collider instead
                        Collider col = nextDialogueTrigger.GetComponent<Collider>();
                        if (col != null && !col.enabled)
                        {
                            col.enabled = true;
                            Debug.Log("Next dialogue trigger collider re-enabled!");
                        }
                        else
                        {
                            Debug.Log("Next dialogue trigger was already active and collider enabled.");
                        }
                    }
                }
                else
                {
                    Debug.Log($" Not enough fish yet: {totalFishStored}/{fishThresholdForDialogue}");
                }
            }
            else
            {
                Debug.Log("No fish to deposit!");
            }
        }
    }
}