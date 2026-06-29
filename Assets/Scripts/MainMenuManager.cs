using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Save Slot UI")]
    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;

    void Start()
    {
        RefreshUI(); // Update the text when the game boots up
    }

    // NEW: We put this in a separate function so we can call it after resetting a save!
    public void RefreshUI()
    {
        int moneySlot1 = PlayerPrefs.GetInt("Money_Slot_1", 0);
        int moneySlot2 = PlayerPrefs.GetInt("Money_Slot_2", 0);

        slot1Text.text = "Save 1 - $" + moneySlot1;
        slot2Text.text = "Save 2 - $" + moneySlot2;
    }

    public void LoadSaveSlot(int slotNumber)
    {
        PlayerPrefs.SetInt("ActiveSlot", slotNumber);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    // --- NEW: RESET LOGIC ---
    public void ResetSaveSlot(int slotNumber)
    {
        // Delete the specific save data from the hard drive!
        PlayerPrefs.DeleteKey("Money_Slot_" + slotNumber);
        PlayerPrefs.Save();

        // Refresh the text to immediately show $0
        RefreshUI();
        Debug.Log("Reset Save Slot: " + slotNumber);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}