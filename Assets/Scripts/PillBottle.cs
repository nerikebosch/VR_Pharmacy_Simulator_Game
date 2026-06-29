using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR; // Required for haptics

public class PillBottle : MonoBehaviour
{
    [Header("Bottle Inventory")]
    public List<PillColor> pillsInside = new List<PillColor>();

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dropSound;

    private void OnTriggerEnter(Collider other)
    {
        Pill droppedPill = other.GetComponent<Pill>();

        if (droppedPill != null)
        {
            pillsInside.Add(droppedPill.myColor);
            Destroy(other.gameObject);

            // NEW: Play sound and send a tiny haptic "tick" to the hands
            if (audioSource && dropSound) audioSource.PlayOneShot(dropSound);
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0, 0.2f, 0.1f);
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(0, 0.2f, 0.1f);

            OrderManager om = FindObjectOfType<OrderManager>();
            if (om != null) om.RefreshUI();
        }
    }
}