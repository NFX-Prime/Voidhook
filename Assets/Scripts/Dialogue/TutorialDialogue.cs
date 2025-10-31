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
            nextTextAction.action.performed += OnNextText;
    }

    void OnDisable()
    {
        if (nextTextAction != null)
            nextTextAction.action.performed -= OnNextText;
    }

    private void OnNextText(InputAction.CallbackContext context)
    {
        if (!active) return;

        currentIndex++;
        if (currentIndex >= messages.Length)
        {
            EndDialogue();
        }
        else
        {
            dialogueText.text = messages[currentIndex];
        }
    }

    public void TriggerDialogue()
    {
        if (hasActivated) return;

        hasActivated = true;
        active = true;
        currentIndex = 0;

        dialoguePanel.SetActive(true);
        dialogueText.text = messages[currentIndex];
    }

    private void EndDialogue()
    {
        active = false;
        dialoguePanel.SetActive(false);
    }
}