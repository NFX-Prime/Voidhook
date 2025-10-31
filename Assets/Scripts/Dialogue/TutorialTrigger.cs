using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialDialogue dialogue;

    /// <summary>
    /// Essentially, will check if the player is standing on something to activate the dialogue!
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogue.TriggerDialogue();
            // Optionally disable the trigger so it can't be reactivated
            GetComponent<Collider>().enabled = false;
        }
    }
}