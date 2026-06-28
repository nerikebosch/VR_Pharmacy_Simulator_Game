using UnityEngine;
using System.Collections.Generic;

public class PillBottle : MonoBehaviour
{
    [Header("Bottle Inventory")]
    // This list keeps track of every pill color dropped inside
    public List<PillColor> pillsInside = new List<PillColor>();

    // This runs when something touches the invisible trigger at the opening
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the thing that hit us has a "Pill" script on it
        Pill droppedPill = other.GetComponent<Pill>();

        if (droppedPill != null)
        {
            // 2. Add that pill's color to our bottle's memory
            pillsInside.Add(droppedPill.myColor);

            // 3. Destroy the physical pill object so it "disappears"
            Destroy(other.gameObject);

            // Print to the console so you can test if it's working
            Debug.Log("Sucked up a " + droppedPill.myColor + " pill! Total inside: " + pillsInside.Count);
        }
    }
}