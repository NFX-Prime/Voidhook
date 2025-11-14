using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform targetDest;
    public GameManager manager;
    public BossAI areaBoss;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && targetDest != null)
        {
            // switch boss targeted to avoid suspicion bug
            manager.boss = areaBoss;

            // Debug to see if it's actually going
            Debug.Log("Going through door");
            // Might be character controller? Getting it
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                // Temporarily disable CharacterController to move player
                cc.enabled = false;
                other.transform.position = targetDest.position;
                // Then going to re-enable it
                cc.enabled = true;
            }

        }

    }
}
