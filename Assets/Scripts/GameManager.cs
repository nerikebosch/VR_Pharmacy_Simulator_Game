using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Required to change levels!
using UnityEngine.XR; // NEW: Required for Haptics

public class GameManager : MonoBehaviour
{
    [Header("Shift Settings")]
    public float shiftTimer = 60f;
    private bool isShiftActive = true;
    private bool isPaused = false; // NEW: Tracks if the game is paused

    [Header("Economy")]
    public int greenPillPrice = 10;
    public int pinkPillPrice = 15;
    public int bluePillPrice = 20;
    public int redPillPrice = 25;

    private int sessionMoney = 0;
    private int sessionPenalties = 0;
    private int totalSavedMoney = 0;
    private int activeSlot;

    [Header("Pause Overlay")]
    public GameObject dimOverlay;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip endOfShiftSound;

    [Header("UI Links")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI endOfShiftText;
    public GameObject endOfShiftButtons; // We will put the Next Day/Main Menu buttons inside this!

    // --- NEW UI LINKS ---
    public GameObject activeShiftButtons; // The folder holding Pause & Quit
    public TextMeshProUGUI pauseButtonText; // To swap between "Pause" and "Resume"

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        activeSlot = PlayerPrefs.GetInt("ActiveSlot", 1);
        sessionPenalties = 0;
        totalSavedMoney = PlayerPrefs.GetInt("Money_Slot_" + activeSlot, 0);

        // Clear the text at the start of the day
        if (endOfShiftText != null) endOfShiftText.text = "";

        // Hide the buttons while we are playing
        if (endOfShiftButtons != null) endOfShiftButtons.SetActive(false);

        // Ensure the timer is visible when the day starts!
        if (timerText != null) timerText.gameObject.SetActive(true);

        if (activeShiftButtons != null) activeShiftButtons.SetActive(true);

        if (dimOverlay != null) dimOverlay.SetActive(false);
    }

    void Update()
    {
        if (isShiftActive && !isPaused)
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

    public void PayForOrder(int greens, int pinks, int blues, int reds, int penaltyAmount)
    {
        if (!isShiftActive) return;

        int payout = (greens * greenPillPrice) +
                     (pinks * pinkPillPrice) +
                     (blues * bluePillPrice) +
                     (reds * redPillPrice);

        // Deduct the penalty for any wrong/extra pills!
        payout -= penaltyAmount;

        sessionMoney += payout;
        sessionPenalties += penaltyAmount;
        Debug.Log("Paid $" + payout + " (included penalty of $" + penaltyAmount + "). Total shift money: $" + sessionMoney);
    }

    private void EndShift()
    {
        isShiftActive = false;

        // NEW: Play the End of Shift sound!
        if (audioSource != null && endOfShiftSound != null)
        {
            audioSource.PlayOneShot(endOfShiftSound);
        }

        // Save the new total back to the hard drive
        int newTotal = totalSavedMoney + sessionMoney;
        PlayerPrefs.SetInt("Money_Slot_" + activeSlot, newTotal);
        PlayerPrefs.Save();

        int grossEarnings = sessionMoney + sessionPenalties;

        // Show the text and the buttons!
        endOfShiftText.text = "SHIFT COMPLETE\n" +
                              "Gross Earnings: $" + grossEarnings + "\n" +
                              "Malpractice Penalties: -$" + sessionPenalties +
                              "Net Profit: $" + sessionMoney + "\n" +
                              "Total Bank: $" + newTotal;

        if (endOfShiftButtons != null) endOfShiftButtons.SetActive(true);

        // Hide the timer text so it gets out of the way!
        if (timerText != null) timerText.gameObject.SetActive(false);

        if (activeShiftButtons != null) activeShiftButtons.SetActive(false);
    }

    // --- SCENE NAVIGATION METHODS ---

    public void TogglePause()
    {
        if (!isShiftActive) return; // Don't allow pausing if the shift is already over

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Freezes physics, AI, and movement instantly
            if (pauseButtonText != null) pauseButtonText.text = "Resume";
        }
        else
        {
            Time.timeScale = 1f; // Unfreezes the world
            if (pauseButtonText != null) pauseButtonText.text = "Pause";
        }

        if (dimOverlay != null) dimOverlay.SetActive(isPaused);
    }

    public void StartNextDay()
    {
        // This instantly reloads the current Pharmacy scene, acting as a fresh day!
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        // Loads Scene 0 (The Main Menu)
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void PlayHaptics(float intensity, float duration)
    {
        // Sends a vibration to both hands
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        rightHand.SendHapticImpulse(0, intensity, duration);
        leftHand.SendHapticImpulse(0, intensity, duration);
    }
}