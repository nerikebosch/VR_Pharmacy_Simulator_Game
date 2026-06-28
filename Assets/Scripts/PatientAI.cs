using UnityEngine;
using UnityEngine.AI;

public class PatientAI : MonoBehaviour
{
    public Transform counterTarget;
    public Transform exitDoorTarget;
    public OrderManager orderManager;

    private NavMeshAgent agent;
    private Animator animator;

    // These act as the AI's "Brain" so it knows what it is currently doing
    private bool isLeaving = false;
    private bool isWaitingAtCounter = false;

    void Awake()
    {
        // Awake runs before ANYTHING else, guaranteeing the components are ready
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // A custom setup function we can call safely from the Spawner
    public void SetupAndWalkToCounter(Transform counter, Transform exit, OrderManager manager)
    {
        counterTarget = counter;
        exitDoorTarget = exit;
        orderManager = manager;

        isLeaving = false;
        isWaitingAtCounter = false;

        agent.SetDestination(counterTarget.position);
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
        // 1. If walking IN and they reach the counter
        if (!isLeaving && !isWaitingAtCounter && !agent.pathPending && agent.remainingDistance <= 0.1f)
        {
            isWaitingAtCounter = true;
            animator.SetBool("isWalking", false);
            orderManager.GenerateRandomOrder();
        }

        // 2. If waiting at the counter, look at it
        if (isWaitingAtCounter)
        {
            // Simply smoothly rotate to match the Counter Target's exact rotation!
            transform.rotation = Quaternion.Slerp(transform.rotation, counterTarget.rotation, Time.deltaTime * 5f);
        }

        // 3. If walking OUT and they reach the door
        if (isLeaving && !agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            // They successfully left the building. Delete them!
            Destroy(gameObject);
        }
    }
}