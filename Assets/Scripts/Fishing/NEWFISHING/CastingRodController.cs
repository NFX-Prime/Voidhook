using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerFishingController : MonoBehaviour
{
    public GameObject bobberPrefab;
    public float castHeight = 0.5f;
    public float minCastDistance = 2f;
    public float maxCastDistance = 10f;
    public float castChargeSpeed = 5f;
    // PUT THE INTERACTION ACTION IN UNITY INSPECTOR!
    public InputActionReference interactAction;


    private bool isCharging = false;
    private float castCharge = 0f;
    private GameObject currentBobber;

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
        }
    }

    /// <summary>
    /// Function that starts cast
    /// </summary>
    /// <param name="context"></param>
    void OnCastStarted(InputAction.CallbackContext context)
    {
        isCharging = true;
        castCharge = 0f;
    }

    /// <summary>
    /// Function that handles when cast is released.
    /// </summary>
    /// <param name="context"></param>
    void OnCastReleased(InputAction.CallbackContext context)
    {
        if (!isCharging) return;
        isCharging = false;

        float castDistance = Mathf.Lerp(minCastDistance, maxCastDistance, castCharge);
        Vector3 castPoint = transform.position + transform.forward * castDistance + Vector3.up * 2f;

        if (currentBobber != null) Destroy(currentBobber);

        currentBobber = Instantiate(bobberPrefab, castPoint, Quaternion.identity);
        castCharge = 0f;
    }
}