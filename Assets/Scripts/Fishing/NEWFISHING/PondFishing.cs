using UnityEngine;
using UnityEngine.InputSystem;

// NOTE: EVERY POND WILL BE ASSIGNED THIS SCRIPT!


public class PondFishing : MonoBehaviour
{

    // Getting bober prefab. We oughta make a model for the bobber itself
    public GameObject bobberPrefab;
    public float castHeight = 0.5f;
    public float biteWaitMin = 1.5f;
    public float biteWaitMax = 4f;

    // The actual bobber (so we can delete it later and re-initiailize it)
    private GameObject bobber;
    private bool isFishing = false;

    // Assign minigameUI in inspector. This is the panel with the FishingMiniGame.cs script!
    public FishingMiniGame miniGameUI;

    // Other variables
    Ray ray;


    void Update()
    {
        // CAST ON CLICK
        if (Mouse.current.leftButton.wasPressedThisFrame && !isFishing)
        {
            ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Only cast if clicked on THIS pond collider
                if (hit.collider == GetComponent<Collider>())
                {
                    StartCoroutine(CastBobber(hit.point));
                }
            }
        }
    }

    System.Collections.IEnumerator CastBobber(Vector3 castPoint)
    {
        isFishing = true;

        // Spawn bobber
        bobber = Instantiate(bobberPrefab, castPoint + Vector3.up * castHeight, Quaternion.identity);

        // Random time before bite
        yield return new WaitForSeconds(Random.Range(biteWaitMin, biteWaitMax));

        // Bobber wiggles (visual feedback)
        StartCoroutine(BobberWiggle());

        // Show mini-game UI
        miniGameUI.StartMiniGame(OnFishHooked, OnFishLost);
    }

    void OnFishHooked()
    {
        // Remove bobber
        Destroy(bobber);

        // Start reel phase UI (IN PROGRESS)
        ReelInWheel.Instance.StartReeling(OnFishCaught);
    }

    void OnFishLost()
    {
        Destroy(bobber);
        isFishing = false;
        Debug.Log("Fish got away...");
    }

    void OnFishCaught()
    {
        Debug.Log("?? Fish caught!");
        GameManager.Instance.AddFish();
        isFishing = false;
    }

    /// <summary>
    /// Coroutine bobberwiggle used to make a simple wiggle
    /// </summary>
    /// <returns></returns>
    System.Collections.IEnumerator BobberWiggle()
    {
        // Sets position based on where the bobber actually is
        Vector3 startPos = bobber.transform.position;

        // Essentially just goes up and down to go crazy bobbing.
        for (int i = 0; i < 4; i++)
        {
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * 4f;
                bobber.transform.position = startPos + Vector3.up * Mathf.Sin(t * Mathf.PI) * 0.2f;
                yield return null;
            }
        }

        // Resets bobber to go back to start position.
        bobber.transform.position = startPos;
    }
}