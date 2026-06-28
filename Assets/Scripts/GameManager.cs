using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Required to change levels!

public class GameManager : MonoBehaviour
{
    [Header("Shift Settings")]
    public float shiftTimer = 60f; // 1 minute shift
    private bool isShiftActive = true;

    [Header("Economy")]
    public int greenPillPrice = 10;
    public int pinkPillPrice = 15;
    public int bluePillPrice = 20;
    public int redPillPrice = 25;

    private int sessionMoney = 0;
    private int totalSavedMoney = 0;
    private int activeSlot;

    [Header("UI Links")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI endOfShiftText;
    public GameObject endOfShiftButtons; // We will put the Next Day/Main Menu buttons inside this!

    void Start()
    {
        activeSlot = PlayerPrefs.GetInt("ActiveSlot", 1);
        totalSavedMoney = PlayerPrefs.GetInt("Money_Slot_" + activeSlot, 0);

        // Clear the text at the start of the day
        if (endOfShiftText != null) endOfShiftText.text = "";

        // Hide the buttons while we are playing
        if (endOfShiftButtons != null) endOfShiftButtons.SetActive(false);

        // Ensure the timer is visible when the day starts!
        if (timerText != null) timerText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (isShiftActive)
        {
            shiftTimer -= Time.deltaTime;

            // Prevent timer from going negative
            if (shiftTimer <= 0)
            {
                shiftTimer = 0;
                EndShift();
            }

            // FORMAT TIMER AS 00:00
            int minutes = Mathf.FloorToInt(shiftTimer / 60F);
            int seconds = Mathf.FloorToInt(shiftTimer % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void PayForOrder(int greens, int pinks, int blues, int reds)
    {
        if (!isShiftActive) return;

        int payout = (greens * greenPillPrice) +
                     (pinks * pinkPillPrice) +
                     (blues * bluePillPrice) +
                     (reds * redPillPrice);

        sessionMoney += payout;
        Debug.Log("Paid $" + payout + " for order! Total shift money: $" + sessionMoney);
    }

    private void EndShift()
    {
        isShiftActive = false;

        // Save the new total back to the hard drive
        int newTotal = totalSavedMoney + sessionMoney;
        PlayerPrefs.SetInt("Money_Slot_" + activeSlot, newTotal);
        PlayerPrefs.Save();

        // Show the text and the buttons!
        endOfShiftText.text = "SHIFT COMPLETE\nEarned Today: $" + sessionMoney + "\nTotal Bank: $" + newTotal;

        if (endOfShiftButtons != null) endOfShiftButtons.SetActive(true);

        // NEW: Hide the timer text so it gets out of the way!
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    // --- SCENE NAVIGATION METHODS ---

    public void StartNextDay()
    {
        // This instantly reloads the current Pharmacy scene, acting as a fresh day!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        // Loads Scene 0 (The Main Menu)
        SceneManager.LoadScene(0);
    }
}