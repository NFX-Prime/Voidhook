using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    // Assign player in inspector
    public Transform player;         
    // Can change how fast enemy is
    public float chaseSpeed = 3f;
    // Speed when player is idle
    public float slowSpeed = 1.5f;
    // How close before it stops
    public float stopDistance = 1.5f; 

    // Reference to player's Movement script
    private Movement playerMovement;  // reference to player script

    private NavMeshAgent agent;

    void Start()
    {
        // Getting agent component
        agent = GetComponent<NavMeshAgent>();

        // Checks to see if player is assigned
        if (player != null)
        {
            // Gets movement script from player
            playerMovement = player.GetComponent<Movement>();
        }
    }

    void Update()
    {
        if (player == null || playerMovement == null) return;

        // Always chase the player, but adjust speed based on walking/idle
        if (!playerMovement.isWalking)
        {
            agent.isStopped = false;

            // If player is idle (no movement input), enemy slows down
            if (playerMovement.moveAction.action.ReadValue<Vector2>() == Vector2.zero)
            {
                // Walk to player slowly
                agent.speed = slowSpeed;
            }
            else
            {
                // Chase after player
                agent.speed = chaseSpeed;
            }

            // Move toward player if not too close
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > stopDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                // stop when close enough (TEMPORARY)
                agent.isStopped = true; 
            }
        }
        else
        {
            // When player is walking, stop chasing (the "weakness" of this enemy)
            agent.isStopped = true; 
        }
    }
}