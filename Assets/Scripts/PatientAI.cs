using UnityEngine;
using UnityEngine.AI;

public class PatientAI : MonoBehaviour
{
    public Transform counterTarget; // Where the patient needs to go

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Tell the agent to start walking to the counter
        if (counterTarget != null)
        {
            agent.SetDestination(counterTarget.position);
            animator.SetBool("isWalking", true);
        }
    }

    // Add this reference at the top of your PatientAI script:
    public OrderManager orderManager;
    private bool hasOrdered = false; // Prevents spamming the order

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetBool("isWalking", false);

            // NEW CODE: Trigger the order once!
            if (!hasOrdered)
            {
                orderManager.GenerateRandomOrder();
                hasOrdered = true;
            }

            Vector3 direction = (counterTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}