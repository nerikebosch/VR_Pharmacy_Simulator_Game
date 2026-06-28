using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections; // Needed for timers

public class OrderManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject orderItemPrefab;
    public Transform orderPanel;
    public GameObject orderCanvas;

    [Header("Pill Sprites")]
    public Sprite greenPill;
    public Sprite pinkPill;
    public Sprite bluePill;
    public Sprite redPill;

    [Header("System Links")]
    public DeliveryZone deliveryZone;
    public PatientAI activePatient;
    public GameManager gameManager;

    [Header("Patient Spawning (NEW)")]
    public GameObject patientPrefab;     // The character prefab to spawn
    public Transform patientSpawnPoint;  // Where they appear (the door)
    public Transform counterTarget;      // Where they walk to (the counter)
    public Transform exitDoorTarget;     // Where they walk to leave

    private Dictionary<PillColor, int> currentOrder = new Dictionary<PillColor, int>();
    private Dictionary<PillColor, TextMeshProUGUI> orderUIRows = new Dictionary<PillColor, TextMeshProUGUI>();

    void Start()
    {
        // CHANGED: Hide the panel instead of the whole canvas
        orderPanel.gameObject.SetActive(false);

        if (activePatient != null)
        {
            activePatient.SetupAndWalkToCounter(counterTarget, exitDoorTarget, this);
        }
    }

    public void GenerateRandomOrder()
    {
        currentOrder.Clear();
        orderUIRows.Clear();
        foreach (Transform child in orderPanel) Destroy(child.gameObject);

        int typesOfPills = Random.Range(1, 4);

        for (int i = 0; i < typesOfPills; i++)
        {
            PillColor randomColor = (PillColor)Random.Range(0, 4);
            if (currentOrder.ContainsKey(randomColor)) continue;

            int randomAmount = Random.Range(1, 4);
            currentOrder.Add(randomColor, randomAmount);

            GameObject newRow = Instantiate(orderItemPrefab, orderPanel);
            TextMeshProUGUI amountText = newRow.GetComponentInChildren<TextMeshProUGUI>();
            amountText.text = "x" + randomAmount.ToString();
            newRow.transform.Find("PillIcon").GetComponent<Image>().sprite = GetSpriteForColor(randomColor);

            orderUIRows.Add(randomColor, amountText);
        }

        //orderCanvas.SetActive(true);// CHANGED: Show the panel instead of the whole canvas
        orderPanel.gameObject.SetActive(true);
    }

    public void RefreshUI()
    {
        Dictionary<PillColor, int> submittedPills = new Dictionary<PillColor, int>();
        foreach (PillBottle bottle in deliveryZone.bottlesInZone)
        {
            foreach (PillColor color in bottle.pillsInside)
            {
                if (submittedPills.ContainsKey(color)) submittedPills[color]++;
                else submittedPills.Add(color, 1);
            }
        }

        foreach (var orderItem in currentOrder)
        {
            PillColor color = orderItem.Key;
            int totalNeeded = orderItem.Value;
            int currentlyDelivered = submittedPills.ContainsKey(color) ? submittedPills[color] : 0;

            int remaining = totalNeeded - currentlyDelivered;
            if (remaining < 0) remaining = 0;

            if (orderUIRows.ContainsKey(color))
            {
                orderUIRows[color].text = "x" + remaining.ToString();

                if (remaining == 0) orderUIRows[color].color = Color.green;
                else orderUIRows[color].color = Color.black;
            }
        }
    }

    public void CheckOrderAndSubmit()
    {
        Dictionary<PillColor, int> submittedPills = new Dictionary<PillColor, int>();
        foreach (PillBottle bottle in deliveryZone.bottlesInZone)
        {
            foreach (PillColor color in bottle.pillsInside)
            {
                if (submittedPills.ContainsKey(color)) submittedPills[color]++;
                else submittedPills.Add(color, 1);
            }
        }

        bool isOrderCorrect = true;
        foreach (var orderItem in currentOrder)
        {
            // CHANGED: Now uses '<' so it passes even if you put extra pills in!
            if (!submittedPills.ContainsKey(orderItem.Key) || submittedPills[orderItem.Key] < orderItem.Value)
            {
                isOrderCorrect = false;
                break;
            }
        }

        if (isOrderCorrect)
        {
            Debug.Log("Order Correct! Patient is leaving.");

            // --- NEW PAYOUT LOGIC ---
            int g = 0, p = 0, b = 0, r = 0;
            foreach (var item in currentOrder)
            {
                if (item.Key == PillColor.Green) g = item.Value;
                if (item.Key == PillColor.Pink) p = item.Value;
                if (item.Key == PillColor.Blue) b = item.Value;
                if (item.Key == PillColor.Red) r = item.Value;
            }
            if (gameManager != null) gameManager.PayForOrder(g, p, b, r);
            // ------------------------

            deliveryZone.ClearZone();
            orderPanel.gameObject.SetActive(false);
            activePatient.LeavePharmacy();
            StartCoroutine(SpawnNextPatientTimer(10f));
        }
        else
        {
            Debug.Log("Order Incorrect! Check the console to see what was missing.");
        }
    }

    IEnumerator SpawnNextPatientTimer(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Create a new patient at the door
        GameObject newPatientObj = Instantiate(patientPrefab, patientSpawnPoint.position, patientSpawnPoint.rotation);
        PatientAI newAI = newPatientObj.GetComponent<PatientAI>();

        // FIX: Use our new custom setup function so they have their targets BEFORE they try to walk!
        newAI.SetupAndWalkToCounter(counterTarget, exitDoorTarget, this);

        // Update the manager to track the new guy
        activePatient = newAI;
    }

    private Sprite GetSpriteForColor(PillColor color)
    {
        switch (color)
        {
            case PillColor.Green: return greenPill;
            case PillColor.Pink: return pinkPill;
            case PillColor.Blue: return bluePill;
            case PillColor.Red: return redPill;
            default: return greenPill;
        }
    }
}