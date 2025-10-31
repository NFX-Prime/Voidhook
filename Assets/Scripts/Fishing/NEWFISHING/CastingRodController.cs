using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFishingController : MonoBehaviour
{
    public static PlayerFishingController Instance; // Singleton for easy access

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
    // List that holds the castarrows
    private List<GameObject> castArrows = new List<GameObject>();

    // Container to hold all preview arrows so they don't affect the player
    private GameObject castPreviewContainer;

    // Fishing line
    private LineRenderer lineRenderer;

    private bool isCharging = false;
    private float castCharge = 0f;
    private GameObject currentBobber;

    // Whether player can cast (disabled while in minigame)
    private bool canCast = true;

    void Awake()
    {
        // Setup singleton
        Instance = this;

        // Setup the LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        // Simple unlit material
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.05f;
        // Only show when bobber exists
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
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position + Vector3.up * castHeight); // Player position (adjust height)
            lineRenderer.SetPosition(1, currentBobber.transform.position); // Bobber position
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Function that starts cast
    /// </summary>
    /// <param name="context"></param>
    void OnCastStarted(InputAction.CallbackContext context)
    {
        // Prevent casting while in reeling minigame
        if (!canCast) return;

        isCharging = true;
        castCharge = 0f;

        // Create the container if it doesn't exist
        if (castPreviewContainer == null)
        {
            castPreviewContainer = new GameObject("CastPreviewContainer");
            // Making it world root
            castPreviewContainer.transform.parent = null;
            castPreviewContainer.transform.position = Vector3.zero;
        }
    }

    /// <summary>
    /// Function that handles when cast is released.
    /// </summary>
    /// <param name="context"></param>
    void OnCastReleased(InputAction.CallbackContext context)
    {
        // Prevent casting while in reeling minigame
        if (!isCharging || !canCast) return;

        isCharging = false;

        float castDistance = Mathf.Lerp(minCastDistance, maxCastDistance, castCharge);
        Vector3 castPoint = transform.position + transform.forward * castDistance + Vector3.up * 2f;

        // Incase something null-ey happens idk this fixed something so pls dont delete
        if (currentBobber != null)
        {
            Destroy(currentBobber);
        }

        currentBobber = Instantiate(bobberPrefab, castPoint, Quaternion.identity);

        // Delete the cast preview
        ClearCastPreview();

        castCharge = 0f;
    }

    /// <summary>
    /// Function that does all the funky math for the casting preview.
    /// </summary>
    void UpdateCastPreview()
    {
        float castDistance = Mathf.Lerp(minCastDistance, maxCastDistance, castCharge);

        // Update origin and direction every frame based on player position and rotation
        Vector3 origin = transform.position + Vector3.up * castHeight;
        Vector3 dir = transform.forward.normalized;

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
            Vector3 point = origin + dir * (castDistance * t);
            point.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

            castArrows[i].transform.position = point;

            // Calculate rotation
            Vector3 nextPoint = (i < arcSegments - 1)
                ? origin + dir * (castDistance * ((float)(i + 1) / (arcSegments - 1)))
                : point + dir;

            nextPoint.y += (i < arcSegments - 1) ? Mathf.Sin(((float)(i + 1) / (arcSegments - 1)) * Mathf.PI) * arcHeight : 0;

            Vector3 direction = (nextPoint - point).normalized;
            if (direction != Vector3.zero)
                castArrows[i].transform.rotation = Quaternion.LookRotation(direction);

            castArrows[i].SetActive(true);
        }
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
    /// <param name="value"></param>
    public void SetCanCast(bool value)
    {
        canCast = value;
    }
}