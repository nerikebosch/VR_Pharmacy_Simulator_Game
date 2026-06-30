using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class OrderManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject orderItemPrefab;
    public GameObject orderBackgroundPanel; // Turns the whole UI on and off
    public Transform orderListContainer;    // Where the pills spawn

    [Header("Pill Sprites")]
    public Sprite greenPill;
    public Sprite pinkPill;
    public Sprite bluePill;
    public Sprite redPill;

    [Header("System Links")]
    public DeliveryZone deliveryZone;
    public GameManager gameManager;

    [Header("Audio & Feedback")]
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip errorSound;
    public AudioClip doorBellSound;
    public AudioClip crowdLeaveSound;

    [Header("Patient Spawning (Queue System)")]
    public GameObject[] patientPrefabs;  // Array of 5 different character prefabs
    public Transform[] queueSpots;       // Array of the 3 spots (0=Counter, 1=Middle, 2=Door)
    public Transform patientSpawnPoint;
    public Transform exitDoorTarget;

    [Header("Queue Settings")]
    public int maxQueueSize = 3;
    public float minSpawnTime = 4f; // Minimum seconds between new customers
    public float maxSpawnTime = 8f; // Maximum seconds between new customers

    // A list to track who is currently in the store
    private List<PatientAI> activePatients = new List<PatientAI>();

    private Dictionary<PillColor, int> currentOrder = new Dictionary<PillColor, int>();
    private Dictionary<PillColor, TextMeshProUGUI> orderUIRows = new Dictionary<PillColor, TextMeshProUGUI>();

    void Start()
    {
        orderBackgroundPanel.SetActive(false);

        // Start the infinite spawning loop
        StartCoroutine(ContinuousSpawner());
    }

    public void GenerateRandomOrder()
    {
        currentOrder.Clear();
        orderUIRows.Clear();
        foreach (Transform child in orderListContainer) Destroy(child.gameObject);

        int typesOfPills = Random.Range(1, 4);

        for (int i = 0; i < typesOfPills; i++)
        {
            PillColor randomColor = (PillColor)Random.Range(0, 4);
            if (currentOrder.ContainsKey(randomColor)) continue;

            int randomAmount = Random.Range(1, 4);
            currentOrder.Add(randomColor, randomAmount);

            GameObject newRow = Instantiate(orderItemPrefab, orderListContainer);
            TextMeshProUGUI amountText = newRow.GetComponentInChildren<TextMeshProUGUI>();
            amountText.text = "x" + randomAmount.ToString();
            newRow.transform.Find("PillIcon").GetComponent<Image>().sprite = GetSpriteForColor(randomColor);

            orderUIRows.Add(randomColor, amountText);
        }

        //Show the panel instead of the whole canvas
        orderBackgroundPanel.SetActive(true);
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
        int totalPillsSubmitted = 0;

        // Stop the button from working if the shift is closed
        if (gameManager != null && !gameManager.isShiftActive) return;

        // count everything in deliveryzone
        foreach (PillBottle bottle in deliveryZone.bottlesInZone)
        {
            foreach (PillColor color in bottle.pillsInside)
            {
                if (submittedPills.ContainsKey(color)) submittedPills[color]++;
                else submittedPills.Add(color, 1);

                totalPillsSubmitted++;
            }
        }

        // Check if they met the minimum requirements
        bool isOrderCorrect = true;
        int totalPillsRequired = 0;

        foreach (var orderItem in currentOrder)
        {
            totalPillsRequired += orderItem.Value;

            if (!submittedPills.ContainsKey(orderItem.Key) || submittedPills[orderItem.Key] < orderItem.Value)
            {
                isOrderCorrect = false;
                break;
            }
        }

        // Resolve the Order
        if (isOrderCorrect)
        {
            Debug.Log("Order Correct! Patient is leaving.");
            if (audioSource && successSound) audioSource.PlayOneShot(successSound);
            if (gameManager != null) gameManager.PlayHaptics(0.5f, 0.2f);

            // Calculate the Malpractice Penalty
            int extraPills = totalPillsSubmitted - totalPillsRequired;
            int penalty = extraPills * 5; // $5 fine per wrong pill

            int g = 0, p = 0, b = 0, r = 0;
            foreach (var item in currentOrder)
            {
                if (item.Key == PillColor.Green) g = item.Value;
                if (item.Key == PillColor.Pink) p = item.Value;
                if (item.Key == PillColor.Blue) b = item.Value;
                if (item.Key == PillColor.Red) r = item.Value;
            }

            if (gameManager != null) gameManager.PayForOrder(g, p, b, r, penalty);

            deliveryZone.ClearZone();
            orderBackgroundPanel.SetActive(false);

            AdvanceQueue();
        }
        else
        {
            Debug.Log("Order Incorrect! Try again.");
            if (audioSource && errorSound) audioSource.PlayOneShot(errorSound);
            if (gameManager != null) gameManager.PlayHaptics(1.0f, 0.5f);
        }
    }

    // This tells the front person to leave, and everyone else to move up
    private void AdvanceQueue()
    {
        if (activePatients.Count == 0) return;

        // Tell the front person to leave
        PatientAI frontPatient = activePatients[0];
        frontPatient.LeavePharmacy();
        activePatients.RemoveAt(0); // Remove them from the list

        // Tell everyone else in line to move forward one spot
        for (int i = 0; i < activePatients.Count; i++)
        {
            bool isFrontOfLine = (i == 0); // If they moved to spot 0, it's their turn
            activePatients[i].MoveToSpot(queueSpots[i], isFrontOfLine);
        }
    }

    // A loop that constantly checks if there is room for more customers
    IEnumerator ContinuousSpawner()
    {
        while (true)
        {
            // If the shift is over, stop spawning
            if (gameManager != null && gameManager.shiftTimer <= 0) break;

            // If the store isn't full yet, spawn someone
            if (activePatients.Count < maxQueueSize)
            {
                // Pick a random character model
                GameObject randomPrefab = patientPrefabs[Random.Range(0, patientPrefabs.Length)];

                // Spawn them
                GameObject newObj = Instantiate(randomPrefab, patientSpawnPoint.position, patientSpawnPoint.rotation);
                PatientAI newAI = newObj.GetComponent<PatientAI>();

                newAI.orderManager = this;
                newAI.exitDoorTarget = exitDoorTarget;

                // Add to our list
                activePatients.Add(newAI);

                // Tell them which spot to walk to based on how many people are in the store
                int theirSpotIndex = activePatients.Count - 1;
                bool isFrontOfLine = (theirSpotIndex == 0);
                newAI.MoveToSpot(queueSpots[theirSpotIndex], isFrontOfLine);

                // Play the doorbell sound
                if (audioSource && doorBellSound) audioSource.PlayOneShot(doorBellSound);
            }

            // Wait a random amount of seconds before trying to spawn someone again
            float randomWaitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(randomWaitTime);
        }
    }
    public void ClearStore()
    {
        // Play the disappointed crowd sound only if there are people in the store
        if (activePatients.Count > 0 && audioSource != null && crowdLeaveSound != null)
        {
            audioSource.PlayOneShot(crowdLeaveSound);
        }

        // Hide the order UI so you can't read it anymore
        if (orderBackgroundPanel != null) orderBackgroundPanel.SetActive(false);

        // Loop through every patient currently waiting in line
        foreach (PatientAI patient in activePatients)
        {
            if (patient != null)
            {
                patient.LeavePharmacy(); // Triggers their walk-out animation
            }
        }

        // Empty the list since the store is now officially empty
        activePatients.Clear();
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