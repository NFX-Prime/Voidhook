using UnityEngine;
using System.Collections;

public class PondFishing : MonoBehaviour
{
    public FishingMiniGame miniGameUI;

    private void OnTriggerEnter(Collider other)
    {
        // Checking if bobber hit pond
        Debug.Log($"Something entered pond: {other.name}");

        Debug.Log($"Tag detected: {other.tag}");

        if (other.CompareTag("Bobber") || other.transform.root.CompareTag("Bobber"))
        {
            Bobber bobber = other.GetComponentInParent<Bobber>();
            if (bobber != null)
            {
                Debug.Log("Bobber successfully found and OnEnterPond called!");
                bobber.OnEnterPond(this);
            }
            else
            {
                Debug.LogWarning(" No Bobber component found in parent chain.");
            }
        }
    }

    public void StartFishing(Bobber bobber)
    {
        Debug.Log("StartFishing called for bobber: " + bobber.name);
        if (miniGameUI == null)
        {
            Debug.LogError("MiniGameUI is null!");
            return;
        }
        StartCoroutine(FishRoutine(bobber));
    }

    private IEnumerator FishRoutine(Bobber bobber)
    {
        float waitTime = Random.Range(1.5f, 4f);
        yield return new WaitForSeconds(waitTime);

        // Check if bobber still exists and is the same one
        if (bobber == null || !bobber.gameObject.activeInHierarchy)
        {
            yield break; // player cast away or bobber destroyed
        }

        // Start the minigame
        if (miniGameUI != null)
        {
            // Tell PlayerFishingController that a fishing session starts
            if (PlayerFishingController.Instance != null)
                PlayerFishingController.Instance.StartFishingSession();

            miniGameUI.StartMiniGame(
                () => { bobber.OnFishHooked(); PlayerFishingController.Instance?.EndFishingSession(); },
                () => { bobber.OnFishLost(); PlayerFishingController.Instance?.EndFishingSession(); }
            );
        }
    }

}