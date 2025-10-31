using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    // Assign the dialogue script here
    public TutorialDialogue dialogue;

    // Flag to make sure this trigger only fires once
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if player enters trigger and it hasn't already triggered
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            // Trigger the dialogue
            if (dialogue != null)
            {
                dialogue.TriggerDialogue();
            }

            // Optionally disable the collider so it can't be triggered again
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;
        }
    }
}