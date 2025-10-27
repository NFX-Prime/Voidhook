using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// NOTE: A PANEL THAT HOLDS THE TEXTMESH FOR THE SEQUENCE WILL BE ASSIGNED THIS SCRIPT!

public class FishingMiniGame : MonoBehaviour
{
    [Header("UI")]
    // UI panel background. Input the panel in the inspector! This panel simply holds the textmesh
    public GameObject panel;

    // The text field showing the sequence of keys to press. 
    public TextMeshProUGUI sequenceText;      

    [Header("Settings")]
    [Min(1)]
    // Number of keys to press in sequence. We can change this for each "special" pond"
    public int sequenceLength = 4;            
    

    // List to hold sequence of keys that will be updated to be clicked on
    private List<Key> sequence = new List<Key>();

    // Making keys list to hold which keys will be used for the fishing game. Change if want to use different keys.
    Key[] keys = { Key.UpArrow, Key.LeftArrow, Key.DownArrow, Key.RightArrow };

    private int index = 0;

    // These onSuccess and onFail actions will be given by the PondFishing class. The pond will essentially parse them in.
    // onSuccess is the PondFishing function onFishHooked
    public System.Action onSuccess;
    // onFail is the PondFishing function onFishLost
    private System.Action onFail;

    // The third function in pondfishing, OnFishCaught will be used in the reelinwheel script.

    /// <summary>
    /// This starts the fishing button sequence.
    /// </summary>
    public void StartMiniGame(System.Action success, System.Action fail)
    {
        onSuccess = success;
        onFail = fail;

        // Reset sequence length
        if (sequenceLength < 1) sequenceLength = 1;

        index = 0;
        sequence.Clear();

        // Generate new sequence (Arrow keys for now)
        for (int i = 0; i < sequenceLength; i++)
            sequence.Add(keys[Random.Range(0, keys.Length)]);

        UpdateUI();

        // Show the panel 
        panel.SetActive(true);
    }

    /// <summary>
    /// Function to update UI with a new sequence.
    /// </summary>
    void UpdateUI()
    {
        sequenceText.text = "";

        for (int i = 0; i < sequence.Count; i++)
        {
            // Highlight current key
            if (i == index)
                sequenceText.text += $"<color=yellow>{sequence[i]}</color> ";
            else
                sequenceText.text += sequence[i] + " ";
        }
    }

    void Update()
    {
        // Early outs:
        // 1. If panel isn't active do nothing
        // 2. If sequence is empty do nothing (safety)
        //if (!panel || !panel.activeSelf) return;
        if (sequence == null || sequence.Count == 0) return;
        if (index < 0 || index >= sequence.Count) return;

        // Check whether the correct key was pressed this frame
        bool correctPressed = false;
        Key currentKey = sequence[index];

        // If keyboard simply doesn't exist, then just stop it to prevent errors.
        if (Keyboard.current == null) return;

        // Use keyboard.current to get the key.
        var keyControl  = Keyboard.current[currentKey];
        if (keyControl != null && keyControl.wasPressedThisFrame)
            correctPressed = true;

        // If player hits correct key:
        if (correctPressed)
        {
            index++;

            // Completed sequence successfully
            if (index >= sequence.Count)
            {
                panel.SetActive(false);
                onSuccess?.Invoke();
            }
            else
            {
                UpdateUI();
            }
        }
        // If player presses any other key → fail (only if some key other than the correct one was pressed)
        // Should we chanGe this to make it not fail? 
        /*else if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            // ensure it's not the correct key (already checked above)
            panel.SetActive(false);
            onFail?.Invoke();
        }
        */
    }
}