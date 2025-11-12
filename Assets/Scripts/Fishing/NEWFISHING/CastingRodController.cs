using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFishingController : MonoBehaviour
{
    // Instance for using in other scripts
    public static PlayerFishingController Instance;

    public GameObject bobberPrefab;
    public float castHeight = 0.5f;
    public float minCastDistance = 0f;
    public float maxCastDistance = 10f;
    public float castChargeSpeed = 1f;
    // PUT THE INTERACTION ACTION IN UNITY INSPECTOR!
    public InputActionReference interactAction;

    [Header("Cast Preview")]
    // Small arrow/marker prefab
    public GameObject castArrowPrefab;
    // Number of arrows in the arc
    public int arcSegments = 10;
    // Maximum height of the arc
    public float arcHeight = 2f;
    // List that holds the cast arrows
    private List<GameObject> castArrows = new List<GameObject>();

    // Container to hold all preview arrows so they don't affect the player
    private GameObject castPreviewContainer;

    // Fishing line
    private LineRenderer lineRenderer;

    [Header("Line Settings")]
    // Max distance before line breaks 
    public float maxLineDistance = 15f;

    private bool isCharging = false;
    private float castCharge = 0f;
    private GameObject currentBobber;
    private bool blockCastUntilRelease = false;

    // This event is called when the line breaks
    public System.Action onLineBreak;

    // Setting up property to see if fishing is activated.
    public bool IsFishingSessionActive { get; private set; } = false;

    // Whether player can cast (disabled while in minigame)
    private bool canCast = true;

    // Cached mouse target for preview and cast
    private Vector3 mouseWorldTarget;

    void Awake()
    {
        // Setup singleton
        Instance = this;

        // Setup the LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.enabled = false;
        lineRenderer.numCapVertices = 2;
    }

    void OnEnable()
    {
        interactAction.action.started += OnCastStarted;
        interactAction.action.canceled += OnCastReleased;
    }

    void OnDisable()
    {
        interactAction.action.started -= OnCastStarted;
        interactAction.action.canceled -= OnCastReleased;
    }

    void Update()
    {
        // If charging,
        if (isCharging)
        {
            // Then increase the length the cast will go
            castCharge += Time.deltaTime * castChargeSpeed;
            castCharge = Mathf.Clamp01(castCharge);

            // Also update the casting arrows to give a visual indicator of where the bobber will go
            UpdateCastPreview();
        }
        else
        {
            // Delete the casting preview
            ClearCastPreview();
        }

        // Update the fishing line if bobber exists
        if (currentBobber != null)
        {
            // Getting distance from bobber to player position
            float distance = Vector3.Distance(transform.position, currentBobber.transform.position);

            // Break the line if too far
            if (distance > maxLineDistance)
            {
                Destroy(currentBobber);
                currentBobber = null;
                lineRenderer.enabled = false;
                onLineBreak?.Invoke();
            }
            else
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.position + Vector3.up * castHeight);
                lineRenderer.SetPosition(1, currentBobber.transform.position);
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Function that starts cast
    /// </summary>
    void OnCastStarted(InputAction.CallbackContext context)
    {
        // Prevent casting while in reeling minigame
        if (!canCast || IsFishingSessionActive || blockCastUntilRelease) return;

        isCharging = true;
        castCharge = 0f;

        // Create the container if it doesn't exist
        if (castPreviewContainer == null)
        {
            castPreviewContainer = new GameObject("CastPreviewContainer");
            castPreviewContainer.transform.parent = null;
            castPreviewContainer.transform.position = Vector3.zero;
        }
    }

    /// <summary>
    /// Function that handles when cast is released.
    /// </summary>
    void OnCastReleased(InputAction.CallbackContext context)
    {
        blockCastUntilRelease = false;

        if (!isCharging || !canCast) return;

        isCharging = false;

        // Final world target from mouse position
        Vector3 castTarget = GetMouseWorldPoint();

        // Clamp to max range from player
        Vector3 direction = (castTarget - transform.position);
        direction.y = 0f; // ignore vertical difference
        float distance = Mathf.Min(direction.magnitude, maxCastDistance);
        castTarget = transform.position + direction.normalized * distance;

        // Spawn the bobber at target + small offset
        if (currentBobber != null)
            Destroy(currentBobber);

        currentBobber = Instantiate(bobberPrefab, castTarget + Vector3.up * 0.1f, Quaternion.identity);

        // Delete the cast preview
        ClearCastPreview();

        castCharge = 0f;
    }

    /// <summary>
    /// Function that does all the funky math for the casting preview.
    /// </summary>
    void UpdateCastPreview()
    {
        // Get mouse target each frame
        mouseWorldTarget = GetMouseWorldPoint();

        // Compute direction from player to mouse target
        Vector3 direction = (mouseWorldTarget - transform.position);
        direction.y = 0f;
        float distance = Mathf.Clamp(direction.magnitude, minCastDistance, maxCastDistance);
        Vector3 dir = direction.normalized;

        Vector3 origin = transform.position + Vector3.up * castHeight;

        // Instantiate arrows as children of container if needed
        while (castArrows.Count < arcSegments)
        {
            GameObject arrow = Instantiate(castArrowPrefab, origin, Quaternion.identity, castPreviewContainer.transform);
            castArrows.Add(arrow);
        }

        // Set arrow positions in world-space
        for (int i = 0; i < arcSegments; i++)
        {
            float t = (float)i / (arcSegments - 1);
            Vector3 point = origin + dir * (distance * t);
            point.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
            castArrows[i].transform.position = point;

            Vector3 nextPoint = (i < arcSegments - 1)
                ? origin + dir * (distance * ((float)(i + 1) / (arcSegments - 1)))
                : point + dir;

            nextPoint.y += (i < arcSegments - 1)
                ? Mathf.Sin(((float)(i + 1) / (arcSegments - 1)) * Mathf.PI) * arcHeight
                : 0;

            Vector3 arrowDir = (nextPoint - point).normalized;
            if (arrowDir != Vector3.zero)
                castArrows[i].transform.rotation = Quaternion.LookRotation(arrowDir);

            castArrows[i].SetActive(true);
        }
    }

    /// <summary>
    /// Get mouse position projected onto ground plane
    /// </summary>
    Vector3 GetMouseWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        // Raycast against anything on the ground layer
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Default", "Ground", "Terrain")))
        {
            return hit.point;
        }

        // Fallback: if nothing hit, use flat plane at player's height
        Plane playerPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        if (playerPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        // As a last fallback, cast a few meters forward
        return transform.position + transform.forward * 2f;
    }

    /// <summary>
    /// Function that simply goes through each arrow preview and deactivates them in the list.
    /// </summary>
    void ClearCastPreview()
    {
        foreach (var arrow in castArrows)
        {
            if (arrow != null) arrow.SetActive(false);
        }
    }

    /// <summary>
    /// Function to enable or disable casting (used by ReelInWheel minigame)
    /// </summary>
    public void SetCanCast(bool value)
    {
        canCast = value;
    }

    /// <summary>
    /// Function that starts fishing
    /// </summary>
    public void StartFishingSession()
    {
        IsFishingSessionActive = true;
        SetCanCast(false);

        isCharging = false;
        ClearCastPreview();

        blockCastUntilRelease = true;
    }

    public void EndFishingSession()
    {
        IsFishingSessionActive = false;
        SetCanCast(true);
    }
}