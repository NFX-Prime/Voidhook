using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{

    // Assign player in inspector
    public Transform[] targets = new Transform[4];
    public TargetSwap[] targetSwaps = new TargetSwap[4];
    public TargetSwap targetCurrent;
    public TargetSwap targetNext;
    // Can change how fast enemy is
    public float chaseSpeed = 3f;
    // Speed when player is idle
    public float slowSpeed = 1.5f;
    // How close before it stops
    public float stopDistance = 1.5f;

    public int numtargets = 0;

    // Reference to player's Movement script
    private Movement playerMovement;  // reference to player script

    private NavMeshAgent agent;

    void Start()
    {
        // Getting agent component
        agent = GetComponent<NavMeshAgent>();

        SwitchTarget();
    }

    void Update()
    {
        if (targets == null || targetSwaps == null) return;

        // Always chase the player, but adjust speed based on walking/idle

        for(int i = 0; i < targetSwaps.Length; i++)
        {
            if (targetSwaps[i].targetted)
            {
                
                float distance = Vector3.Distance(transform.position, targets[i].position);
                if (distance > stopDistance)
                {
                    agent.SetDestination(targets[i].position);
                }
                else
                {
                    // stop when close enough (TEMPORARY)
                    agent.isStopped = true;
                }
            }

        }
    }

    public void SwitchTarget()
    {
        targetNext = targetCurrent;
        targetCurrent = targetSwaps[Random.Range(0, targets.Length)];
        targetCurrent.targetted = true;

        if(targetCurrent == targetNext)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targetSwaps[i].targetted = false;
            }
            SwitchTarget();

        }

    }
}
