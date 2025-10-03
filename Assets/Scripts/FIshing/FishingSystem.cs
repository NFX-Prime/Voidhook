using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class FishingSystem : MonoBehaviour
{
    public Transform pondCenter;
    public float castRange = 10f;
    public bool isFishing = false;

    private Transform fishTarget;
    public float catchProgress = 0f;
    private float catchThreshold = 5f;

    // PHYSICS FOR THE FISHING LINE ITSELF. CHANGE VALUES WHEN NECESSARY!
    [Header("Line Physics")]
    // Max length before breaking
    public float lineLength = 15f;        
    // Too close = slack
    public float minTension = 2f;        
    // Too far = break
    public float maxTension = 5f;        
    // How much pulling helps progress
    public float reelForce = 2f;         
    // How strongly fish moves
    public float fishPullStrength = 3f;  
    // How often fish changes direction
    public float fishChangeDirTime = 2f; 
    private float fishTimer;
    private Vector3 fishDirection;

    // Line Renderer - This is our reel
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Setting up line in awake
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.02f;
        // Just getting a new material. Can set it to a different acutaly asset material later.
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.gray;
        // Hiding until fishing starts
        lineRenderer.enabled = false; 
    }

    void Update()
    {
        // Casting using the left mouse button.
        if (Mouse.current.leftButton.wasPressedThisFrame && !isFishing)
        {
            // Only goes if near a pond 
            if (Vector3.Distance(transform.position, pondCenter.position) < castRange)
            {
                StartFishing();
            }
        }
        
        if (isFishing)
        {
            // Must be holding the left button to fish
            if (Mouse.current.leftButton.isPressed)
            {
                HandleFishing();
            }
            // Stops when released early
            else
            {
                StopFishing(false);
            }
        }
    }

    /// <summary>
    /// Function for starting the fishing minigame itself. Decides the direction and position of the fish in the pond.
    /// </summary>
    void StartFishing()
    {
        isFishing = true;
        catchProgress = 0f;

        // Create fish target. Will be invisible for now. 
        if (fishTarget == null)
        {
            fishTarget = new GameObject("FishTarget").transform;
        }

        // Fish target position will transform randomly. Trying to make it inside the pond object itself.
        fishTarget.position = pondCenter.position + Random.insideUnitSphere * 2f;
        fishTarget.position = new Vector3(fishTarget.position.x, pondCenter.position.y, fishTarget.position.z);

        // For now starts in random direction. Will try to see if we can make it stop once in a while.
        fishDirection = Random.insideUnitSphere;
        fishDirection.y = 0;
        fishTimer = fishChangeDirTime;

        // Show line
        lineRenderer.enabled = true;
    }
    
    /// <summary>
    /// Function that calculates all the fishing movement and the line reel movement.
    /// </summary>
    void HandleFishing()
    {
        // Fish movement based on deltatime. Everytime it goes below 0, the direction changes for now.
        fishTimer -= Time.deltaTime;
        if (fishTimer <= 0)
        {
            fishDirection = Random.insideUnitSphere;
            // Always have the y direction be 0 (I think we'll keep the fishing to a flat plane for now.
            // Later on we can do weird waterfall fishing if we're higher IQ.
            fishDirection.y = 0;
            fishTimer = fishChangeDirTime;
        }
        // Constantly updates direction.
        fishTarget.position += fishDirection.normalized * fishPullStrength * Time.deltaTime;

        // Update line positions. This dot sets the line from the player.
        lineRenderer.SetPosition(0, transform.position + Vector3.up * 1f);
        // This dot sets the other end of the line that is on the fish.
        lineRenderer.SetPosition(1, fishTarget.position);

        // Check distance
        float dist = Vector3.Distance(transform.position, fishTarget.position);

        // If line is too far away, it breaks (fish gets away)
        if (dist > lineLength) 
        {
            StopFishing(false);
            return;
        }

        // If the player is too close to the fishing line, it'll lose progress.
        if (dist < minTension) 
        {
            catchProgress = Mathf.Max(0, catchProgress - Time.deltaTime);
        }

        /*
        OLD FISHING SYSTEM THAT WAS TOO SIMPLE
        If player is in the correct medium spot (not too close and not too far to snap) it'll increase progression

        else if (dist < maxTension) 
        {
            float mid = (minTension + maxTension) / 2f;
            float alignment = 1f - Mathf.Abs(dist - mid) / (maxTension - minTension);
            catchProgress += alignment * reelForce * Time.deltaTime;
        }
        */

        // NEW FISHING SYSTEM
        // This one is more in line with Taryn's fishing concept. You Essentially use the mouse to tug the fish in the opposite direction to catch it. Must still be in a decent amount of length away from the fish though (taken from old system). So its both WASD movement and mouse movement. Maybe too much?

        // 1. Get mouse direction from screen center
        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mouseDir = (mouse - screenCenter).normalized;

        // 2. Convert to world-space direction on XZ plane
        Vector3 tugDir = new Vector3(mouseDir.x, 0, mouseDir.y);

        // 3. Compare tug with fish direction (opposite)
        float alignment = Vector3.Dot(tugDir, -fishDirection.normalized);
        // +1 = perfectly opposite to fish pull, 0 = perpendicular, -1 = same direction

        if (alignment > 0.2f) // must be somewhat opposite
        {
            catchProgress += alignment * reelForce * Time.deltaTime;
        }
        else
        {
            catchProgress = Mathf.Max(0, catchProgress - Time.deltaTime); // lose progress if not tugging correctly
        }



        // If succesful catch
        if (catchProgress >= catchThreshold)
        {
            // Signify that the fish was caught.
            StopFishing(true);
        }
    }

    /// <summary>
    /// Function to stop fishing. Here we can put the code or anything to showcase what happens when you get a fish or don't.
    /// </summary>
    /// <param name="caught"></param>
    void StopFishing(bool caught)
    {
        isFishing = false;
        // Hide line. No longer needed
        lineRenderer.enabled = false; 

        // If succesfully caught a fish
        if (caught)
        {
            Debug.Log("🎣 You caught a fish!");
        }
        // If not succesfully caught a fish
        else
        {
            Debug.Log("❌ The fish escaped...");
        }
    }
}
