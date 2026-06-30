using UnityEngine;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour
{
    [Header("Button Settings")]
    public float pushDistance = 0.0084f;
    public float springSpeed = 5f;

    [Header("Event to Trigger")]
    public UnityEvent onPressed;

    private Vector3 startPos;
    private bool isPressed = false;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Calculate how far down the button is currently pushed
        float currentDistance = startPos.y - transform.localPosition.y;

        if (currentDistance >= pushDistance * 0.9f && !isPressed)
        {
            isPressed = true;
            onPressed.Invoke();
            Debug.Log("BUTTON SMASHED!");
        }
        else if (currentDistance < pushDistance * 0.5f)
        {
            isPressed = false;
        }

        // The "Spring" effect
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * springSpeed);
    }

    private void OnCollisionStay(Collision collision)
    {
        // If a physical object (like the bottle) is resting on the button, force it down
        transform.localPosition = new Vector3(startPos.x, startPos.y - pushDistance, startPos.z);
    }
}