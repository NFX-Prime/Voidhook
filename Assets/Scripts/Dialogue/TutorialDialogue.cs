using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TutorialDialogue : MonoBehaviour
{
    [Header("UI Elements")]
    // Panel containing the text
    public GameObject dialoguePanel;
    // The actual UI text element
    public Text dialogueText;
    // Small icon or text that says "Press E"
    public GameObject pressEIcon;

    [Header("Dialogue Content")]
    [TextArea]
    // Array of messages to cycle through
    public string[] messages;

    [Header("Player Input")]
    // Input the E key/Interact key in inspector pls!
    public InputActionReference nextTextAction;

    private int currentIndex = 0;
    private bool active = false;
    // Only triggers once
    private bool hasActivated = false;

    void OnEnable()
    {
        if (nextTextAction != null)
        {
            // Enable the Input Action so it can respond to E presses
            nextTextAction.action.Enable();
            // Subscribe to the performed callback
            nextTextAction.action.performed += OnNextText;
        }
    }

    void OnDisable()
    {
        if (nextTextAction != null)
        {
            // Unsubscribe from the callback
            nextTextAction.action.performed -= OnNextText;
            // Disable the action to clean up
            nextTextAction.action.Disable();
        }
    }

    private void OnNextText(InputAction.CallbackContext context)
    {
        if (!active) return;

        // Go to next message
        currentIndex++;

        if (currentIndex >= messages.Length)
        {
            // If finished, hide dialogue
            EndDialogue();
        }
        else
        {
            // Update the text in the panel
            dialogueText.text = messages[currentIndex];
        }
    }

    public void TriggerDialogue()
    {
        if (hasActivated) return;

        hasActivated = true;
        active = true;
        currentIndex = 0;

        // Show the dialogue panel and first message
        dialoguePanel.SetActive(true);
        dialogueText.text = messages[currentIndex];

        // Show the "Press E" icon
        if (pressEIcon != null)
            pressEIcon.SetActive(true);
    }

    private void EndDialogue()
    {
        active = false;

        // Hide dialogue panel
        dialoguePanel.SetActive(false);

        // Hide the "Press E" icon
        if (pressEIcon != null)
            pressEIcon.SetActive(false);
    }
}