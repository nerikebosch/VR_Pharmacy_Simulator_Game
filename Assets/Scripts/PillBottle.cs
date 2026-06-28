using UnityEngine;
using System.Collections.Generic;

public class PillBottle : MonoBehaviour
{
    [Header("Bottle Inventory")]
    public List<PillColor> pillsInside = new List<PillColor>();

    private void OnTriggerEnter(Collider other)
    {
        Pill droppedPill = other.GetComponent<Pill>();

        if (droppedPill != null)
        {
            pillsInside.Add(droppedPill.myColor);
            Destroy(other.gameObject);
            Debug.Log("Sucked up a " + droppedPill.myColor + " pill!");

            // Tell the UI to check if numbers need to go down!
            OrderManager om = FindObjectOfType<OrderManager>();
            if (om != null) om.RefreshUI();
        }
    }
}