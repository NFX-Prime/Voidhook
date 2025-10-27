using UnityEngine;

public class BaseZone : MonoBehaviour
{
    // Total Fish stored
    public int totalFishStored = 0;

    // Set up playerFishing script
    public FishingSystem playerFishing;
    public GameManager gameManager;

    private void Awake()
    {

    }


    // Function that checks if zone collides with another object.
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("BaseZone triggered");
        if (gameManager.fishCount > 0)
        {
            Debug.Log("Fishcount analyzed");
            gameManager.DepositFish();
        }

        
    }
}