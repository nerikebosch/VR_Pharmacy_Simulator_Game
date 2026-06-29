using UnityEngine;
using UnityEngine.XR;

public class TrashCan : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip trashSound;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object thrown in is a Pill Bottle
        PillBottle ruinedBottle = other.GetComponent<PillBottle>();

        if (ruinedBottle != null)
        {
            // Play a sound and give a haptic buzz
            if (audioSource != null && trashSound != null)
            {
                audioSource.PlayOneShot(trashSound);
            }

            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0, 0.5f, 0.2f);
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(0, 0.5f, 0.2f);

            // Destroy the bottle forever
            Destroy(ruinedBottle.gameObject);

            Debug.Log("Ruined bottle thrown away!");

            // Tell the UI to refresh in case the bottle was somehow in the delivery zone
            OrderManager om = FindObjectOfType<OrderManager>();
            if (om != null) om.RefreshUI();
        }
    }
}