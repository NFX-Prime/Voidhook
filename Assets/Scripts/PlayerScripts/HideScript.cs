using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class HideSpot : MonoBehaviour
{
    [Header("Input Action")]
    // Put interact button here (should just be E)
    public InputActionReference interactAction;

    [Header("UI Prompt")]
    // For future E prompt
    public GameObject pressEIcon;

    [Header("Hiding Settings")]
    // IF WE EVER WANT TO HAVE THE POSTION OF THE EXIT BE DIFFERENT FROM THE ENTRANCE (like venting?)
    public Transform hidePosition; 

    private bool playerInside = false;
    private bool isHidingHere = false;
    private Transform player;
    public Movement playerMovement;

    void OnEnable()
    {
        interactAction.action.performed += OnInteract;
    }

    void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
    }

    // Essentially, the player can hide if it collides with the collider box of any sort in the object. So if we want an object to both be able to be stood on and hide inside, it'll need 2 colliders. One for the physical, which wont allow the player to pass through, and one for the hiding that allows the player to pass through.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !PlayerState.Instance.isHidden)
        {
            playerInside = true;
            player = other.transform;

            if (pressEIcon != null)
                pressEIcon.SetActive(true);
        }
    }

    /// <summary>
    /// Function that resets bools when player leaves
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isHidingHere)
        {
            playerInside = false;

            if (pressEIcon != null)
                pressEIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Function that hides the player, locking them in the object to be unable to move.
    /// </summary>
    /// <param name="context"></param>
    private void OnInteract(InputAction.CallbackContext context)
    {
        // Essentially only goes if the player is inide hte box.
        if (!playerInside) return;

        // Enter hiding (only if not already hidden)
        if (!PlayerState.Instance.isHidden)
        {
            EnterHiding();
        }
        else if (isHidingHere)
        {
            // Only exit if the player is hiding in THIS spot
            ExitHiding();
        }
    }

    private void EnterHiding()
    {
        isHidingHere = true;

        playerMovement.isHiding = true;

        // Move player to hide location
        player.position = hidePosition.position;

        // Hide player model (Maybe set it as true still? This was just an easy solution)
        player.gameObject.SetActive(false);

        // Mark player globally hidden
        PlayerState.Instance.isHidden = true;
        
        // debug helper
        Debug.Log($"Player is now hiding at: {gameObject.name}");
    }

    private void ExitHiding()
    {
        playerMovement.isHiding = false;
        isHidingHere = false;

        // Reveal player visually
        player.gameObject.SetActive(true);

        // Mark no longer hidden
        PlayerState.Instance.isHidden = false;

        // More debug helping
        Debug.Log("Player exited hiding spot");

        // Allow prompt to appear again 
        playerInside = false;

        if (pressEIcon != null)
        {
            pressEIcon.SetActive(false);
        }
           
    }
}