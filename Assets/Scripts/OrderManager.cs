using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject orderItemPrefab; // Drag your OrderItemRow prefab here
    public Transform orderPanel;       // Drag your BackgroundPanel here
    public GameObject orderCanvas;     // The whole canvas to hide/show

    [Header("Pill Sprites")]
    public Sprite greenPill;
    public Sprite pinkPill;
    public Sprite bluePill;
    public Sprite redPill;

    // A simple enum to track colors
    private enum PillColor { Green, Pink, Blue, Red }

    void Start()
    {
        // Hide the UI when the game starts
        orderCanvas.SetActive(false);
    }

    public void GenerateRandomOrder()
    {
        // 1. Clear old orders
        foreach (Transform child in orderPanel)
        {
            Destroy(child.gameObject);
        }

        // 2. Decide how many different colors they want (e.g., 1 or 2 types)
        int typesOfPills = Random.Range(1, 4);
        List<PillColor> chosenColors = new List<PillColor>();

        // 3. Generate the items
        for (int i = 0; i < typesOfPills; i++)
        {
            // Pick a random color
            PillColor randomColor = (PillColor)Random.Range(0, 4);

            // Prevent duplicate colors in the same order
            if (chosenColors.Contains(randomColor)) continue;
            chosenColors.Add(randomColor);

            // Pick a random amount (1 to 3 pills)
            int randomAmount = Random.Range(1, 4);

            // 4. Spawn the UI Row
            GameObject newRow = Instantiate(orderItemPrefab, orderPanel);

            // 5. Update the Text
            TextMeshProUGUI amountText = newRow.GetComponentInChildren<TextMeshProUGUI>();
            amountText.text = "x" + randomAmount.ToString();

            // 6. Update the Icon Image
            Image pillIcon = newRow.transform.Find("PillIcon").GetComponent<Image>();
            pillIcon.sprite = GetSpriteForColor(randomColor);
        }

        // Show the UI!
        orderCanvas.SetActive(true);
    }

    // Helper function to match the enum to the correct image
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