using UnityEngine;
using System.Collections.Generic;

public class DeliveryZone : MonoBehaviour
{
    public List<PillBottle> bottlesInZone = new List<PillBottle>();
    public OrderManager orderManager;

    private void OnTriggerEnter(Collider other)
    {
        PillBottle bottle = other.GetComponent<PillBottle>();
        if (bottle != null && !bottlesInZone.Contains(bottle))
        {
            bottlesInZone.Add(bottle);
            if (orderManager != null) orderManager.RefreshUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PillBottle bottle = other.GetComponent<PillBottle>();
        if (bottle != null && bottlesInZone.Contains(bottle))
        {
            bottlesInZone.Remove(bottle);
            if (orderManager != null) orderManager.RefreshUI();
        }
    }

    public void ClearZone()
    {
        foreach (PillBottle bottle in bottlesInZone)
        {
            if (bottle != null) Destroy(bottle.gameObject);
        }
        bottlesInZone.Clear();
    }
}