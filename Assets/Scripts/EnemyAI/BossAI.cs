using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{

    // Assign player in inspector
    public Transform[] targets = new Transform[4];
    public TargetSwap[] targetSwaps = new TargetSwap[4];
    public TargetSwap targetCurrent;
    public TargetSwap targetNext;

    public Transform player;
    // Can change how fast enemy is
    public float chaseSpeed = 20f;
    // Speed when player is idle
    public float slowSpeed = 2f;
    // How close before it stops
    public float stopDistance = 1.5f;

    public float distance;

    public int numtargets = 0;

    public float suspicion;

    public float runningInc;

    // Reference to player's Movement script
    private Movement playerMovement;  // reference to player script

    private NavMeshAgent agent;


    void Start()
    {
        // Getting agent component
        agent = GetComponent<NavMeshAgent>();

        agent.acceleration = 30f;
        agent.angularSpeed = 720f;

        SwitchTarget();
    }

    void Update()
    {
        if (targets == null || targetSwaps == null) return;
        
        // calc distance between player and boss
        distance = Vector3.Distance(player.transform.position, transform.position);

        // Change amount of suspicion gained based on distance
        if (distance < 10.0f)
        {
            suspicion += 20f * Time.deltaTime;
        }
        else if (distance >= 10.0f && distance < 14.0f)
        {
            suspicion += 10f * Time.deltaTime;
        }
        else if(distance >= 14.0f && distance < 20.0f)
        {
            suspicion += 5f * Time.deltaTime;
        }
        else
        {
            suspicion -= 2.5f * Time.deltaTime;

            if (suspicion < 0)
            {
                suspicion = 0;
            }
        }

        // chase if suspicion reaches maximum
        if(suspicion >= 100)
        {
            agent.SetDestination(player.transform.position);
            suspicion = 100;
        }
        // else cycle through wander targets
        else
        {
            for (int i = 0; i < targetSwaps.Length; i++)
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
