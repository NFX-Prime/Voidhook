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
        Debug.Log($"Waiting {waitTime} seconds for bite...");
        yield return new WaitForSeconds(waitTime);

        if (miniGameUI == null)
        {
            Debug.LogError("MiniGameUI is null during FishRoutine!");
            yield break;
        }

        Debug.Log("Triggering MiniGame now!");
        miniGameUI.StartMiniGame(
            () => bobber.OnFishHooked(),
            () => bobber.OnFishLost()
        );
    }

}