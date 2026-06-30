using UnityEngine;
using UnityEngine.AI;

public class PatientAI : MonoBehaviour
{
    public Transform currentTargetSpot; // Where they are currently heading
    public Transform exitDoorTarget;
    public OrderManager orderManager;

    private NavMeshAgent agent;
    private Animator animator;

    private bool isLeaving = false;
    private bool isWaitingAtCounter = false;
    public bool isFrontOfLine = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void MoveToSpot(Transform newSpot, bool frontOfLine)
    {
        currentTargetSpot = newSpot;
        isFrontOfLine = frontOfLine;

        isLeaving = false;
        isWaitingAtCounter = false;

        agent.SetDestination(currentTargetSpot.position);
        animator.SetBool("isWalking", true);
    }

    public void LeavePharmacy()
    {
        isLeaving = true;
        isWaitingAtCounter = false;
        animator.SetBool("isWalking", true);
        agent.SetDestination(exitDoorTarget.position);
    }

    void Update()
    {
        if (!isLeaving && !isWaitingAtCounter && !agent.pathPending && agent.remainingDistance <= 0.1f)
        {
            isWaitingAtCounter = true;
            animator.SetBool("isWalking", false);

            // only place the order if they are at the actual counter
            if (isFrontOfLine)
            {
                orderManager.GenerateRandomOrder();
            }
        }

        if (isWaitingAtCounter)
        {
            // Always face forward toward the counter while waiting in line
            transform.rotation = Quaternion.Slerp(transform.rotation, currentTargetSpot.rotation, Time.deltaTime * 5f);
        }

        if (isLeaving && !agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            Destroy(gameObject);
        }
    }
}