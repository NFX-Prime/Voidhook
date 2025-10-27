using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class FishingSystemPond : MonoBehaviour
{
    public Transform pondCenter;
    public float castRange = 10f;
    public bool isFishing = false;

    // Getting player character
    public Transform player;

    private Transform fishTarget;
    public float catchProgress = 0f;
    private float catchThreshold = 5f;

    // PHYSICS FOR THE FISHING LINE ITSELF. CHANGE VALUES WHEN NECESSARY!
    [Header("Line Physics")]
    // Length of line
    public float lineLength = 15f;        
    // Too close = slack
    public float minTension = 2f;        
    // Too far = break
    public float maxTension = 15f;        
    // How much pulling helps progress
    public float reelForce = 2f;         
    // How strongly fish moves
    public float fishPullStrength = 3f;  
    // How often fish changes direction
    public float fishChangeDirTime = 2f; 
    private float fishTimer;
    private Vector3 fishDirection;
    // Float to hold distance of fish to player.
    private float dist;

    /// <summary>
    /// "Tugging" is essentially the feedback for the fish moving so that the player can feel the fish being alive.
    /// </summary>
    [Header("Tugging")]
    // Current fish velocity
    private Vector3 fishVelocity;
    // Temporary tug impulse 
    private Vector3 tugImpulse;
    // How strong the tug will feel
    public float tugStrength = 3f;
    // How fast it fades away
    public float tugDamping = 5f;

    // Assign pond gameobject collider in inspector
    [Header("Pond Settings")]
    public Collider pondCollider;

    [Header("Fish Settings")]
    // Assign the fish object in the inspector
    public GameObject fishPrefab;
    private GameObject fishObject;

    [Header("UI")]
    // To hold the progressbar
    public Slider catchProgressBar;

    // Further fish physics
    [SerializeField]
    private float alignment;
    [SerializeField]
    private float currentSpeed;
    private Vector3 targetVelocity;

    // Line Renderer - This is our reel
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Setting up line in awake
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.02f;
        // Just getting a new material. Can set it to a different acutal asset material later.
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.gray;
        // Hiding until fishing starts
        lineRenderer.enabled = false; 

        // Getting pond center
        pondCenter = this.transform;
        
        // Getting collider
        pondCollider = this.GetComponent<Collider>();


    }

    void Update()
    {
        // Casting using the left mouse button.
        if (Mouse.current.leftButton.wasPressedThisFrame && !isFishing)
        {
            // Only goes if near a pond 
            if (Vector3.Distance(transform.position, player.position) < castRange)
            {
                StartFishing();
            }
        }
        
        if (isFishing == true)
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
                Debug.Log("You let go early...");
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

        // Destroy previous fish object if it exists
        if (fishObject != null)
        {
            Destroy(fishObject);
        }

        // Instantiate new fish at the target position
        if (fishPrefab != null)
        {
            fishObject = Instantiate(fishPrefab, fishTarget.position, Quaternion.identity);
        }

        // Start catch progress bar
        if (catchProgressBar != null)
        {
            catchProgressBar.value = 0f;
        }

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
            // Get random direction
            fishDirection = Random.insideUnitSphere;
            // Make sure the Y value isn't changed.
            fishDirection.y = 0;
            fishDirection.Normalize();
            fishTimer = fishChangeDirTime;
        }

        // NEW FISHING SYSTEM
        // This one is more in line with Taryn's fishing concept. You Essentially use the mouse to tug the fish in the opposite direction to catch it. Must still be in a decent amount of length away from the fish though (taken from old system). So its both WASD movement and mouse movement. Maybe too much?

        // Get mouse direction from screen center
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 10f)
        );


        // Determining fish pull speed based on pull strength
        currentSpeed = fishPullStrength;
        Vector3 baseVelocity = fishDirection * currentSpeed;

        // Tug for the slowed down of the fish.
        Vector3 appliedTug = Vector3.zero;

        // Dampening tug impulse
        tugImpulse = Vector3.Lerp(tugImpulse, Vector3.zero, Time.deltaTime * tugDamping);

        // Applying it to velocity
        targetVelocity = baseVelocity + tugImpulse;

        // Putting it into a new position to temporarily hold
        Vector3 newPosition = fishTarget.position + targetVelocity * Time.deltaTime;

        // Taking pond and using its collider coordinates to make sure the fish won't go out of the bounds of the pond.
        if (pondCollider != null)
        {
            Bounds bounds = pondCollider.bounds;
            float clampedX = Mathf.Clamp(newPosition.x, bounds.min.x, bounds.max.x);
            float clampedZ = Mathf.Clamp(newPosition.z, bounds.min.z, bounds.max.z);
            float clampedY = pondCenter.position.y; // Keep fish at pond's Y level
            newPosition = new Vector3(clampedX, clampedY, clampedZ);
        }

        // Move fish
        fishTarget.position = newPosition;

        // Move the visible fish object to match fishTarget
        if (fishObject != null)
        {
            fishObject.transform.position = fishTarget.position;
        }

        // Making the fish rotate to face the direction it's going.
        if (fishObject != null && targetVelocity != Vector3.zero)
        {
            fishObject.transform.rotation = Quaternion.LookRotation(targetVelocity);
        }

        // Update reel line positions. This dot sets the line from the player.
        lineRenderer.SetPosition(0, player.position + Vector3.up * 1f);
        // This dot sets the other end of the line that is on the fish.
        lineRenderer.SetPosition(1, fishTarget.position);

        // Get distance from player to fish.
        dist = Vector3.Distance(transform.position, fishTarget.position);

        // Combining alignment with position of player and tug direction.
        Vector3 fishToPlayer = (transform.position - fishTarget.position).normalized;
        Vector3 fishDir = fishDirection.normalized;

        // Find how fish is moving relative to player
        float towardFactor = Vector3.Dot(fishDir, fishToPlayer);

        // Convert to world-space direction on XZ plane
        Vector3 tugDir = (Camera.main.ScreenToWorldPoint(
                new Vector3(Mouse.current.position.x.ReadValue(),
                            Mouse.current.position.y.ReadValue(), 10f))
            - transform.position).normalized;
        tugDir.y = 0;
        tugDir.Normalize();

        // Compare how opposite tug direction is to fish direction
        float tugSameAsFish = Vector3.Dot(fishDir, tugDir);
        // If opposite, then just inverse it.
        float tugOppositeFish = -tugSameAsFish;

        // Compare how fish moves relative to player position
        float positionalFactor = Vector3.Dot(fishDir, fishToPlayer);

        // Combine based on fish movement using below if statement
        float alignment;

        if (towardFactor > 0f)
        {
            // Fish moving toward player → tug same direction helps
            alignment = tugOppositeFish;
        }
        else
        {
            // Fish moving away from player → tug opposite direction helps
            alignment = tugSameAsFish;
        }


        // If line is too far away, it breaks (fish gets away)
        if (dist > lineLength) 
        {
            StopFishing(false);
            // End fishing program.
            return;
        }

        // Catch progress logic
        if (alignment < -0.3f && dist >= minTension && dist <= maxTension)
        {
            // Only increase if tugging mostly opposite and in the valid range
            float mid = (minTension + maxTension) / 2f;
            // still encourages keeping a good distance
            float distanceFactor = 1f - Mathf.Abs(dist - mid) / (maxTension - minTension);
            catchProgress += distanceFactor * reelForce * Time.deltaTime;
        }
        else
        {
            // If not tugging correctly, lose a little progress over time
            catchProgress = Mathf.Max(0f, catchProgress - Time.deltaTime / 4f);
        }

        // Update catch progress bar
        if (catchProgressBar != null)
        {
            catchProgressBar.value = catchProgress / catchThreshold;
        }

        // If succesful catch
        if (catchProgress > catchThreshold)
        {
            // Signify that the fish was caught.
            StopFishing(true);
        }

        // DEBUGGING STUFF
        Debug.DrawLine(fishTarget.position, fishTarget.position + fishDir * 2f, Color.cyan);
        Debug.DrawLine(fishTarget.position, fishTarget.position + fishToPlayer * 2f, Color.yellow);
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

        // Reset catch progress bar
        if (catchProgressBar != null)
        {
            catchProgressBar.value = 0f;
        }


        // If succesfully caught a fish
        if (caught)
        {
            Destroy(fishObject);
            Debug.Log("🎣 You caught a fish!");
            GameManager.Instance.AddFish(); 
        }
        // If not succesfully caught a fish
        else
        {
            Destroy(fishObject);
            // Print out last location of fish
            Debug.Log("Last location: x:" + fishTarget.position.x + " y: " + fishTarget.position.y + " z: " + fishTarget.position.z + " Distance: " + dist);
            Debug.Log("❌ The fish escaped...");
        }
    }


}
