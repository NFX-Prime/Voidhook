using UnityEngine;
using UnityEngine.SceneManagement;  // Needed for reloading scenes

public class EnemyCollision : MonoBehaviour
{
    /// <summary>
    /// Function that sees if player collides with enemy
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    // If you’re using CharacterController on the player,
    // replace OnCollisionEnter with OnTriggerEnter and make
    // the enemy’s collider set to "Is Trigger"
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    /// <summary>
    /// Function that runs if player touches enemy
    /// </summary>
    void KillPlayer()
    {
        // Simply reload the scene
        Debug.Log("💀 Player was caught by enemy!");
        // Initiate DeathManager kill player method
        DeathManager.Instance.KillPlayer();
    }
}
