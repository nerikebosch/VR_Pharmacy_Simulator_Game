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
        // When the menu loads, check the saved money for each slot!
        // The "0" means if no save exists, default to $0.
        int moneySlot1 = PlayerPrefs.GetInt("Money_Slot_1", 0);
        int moneySlot2 = PlayerPrefs.GetInt("Money_Slot_2", 0);

        slot1Text.text = "Save 1\n$" + moneySlot1;
        slot2Text.text = "Save 2\n$" + moneySlot2;
    }

    // Call this from your UI Buttons! Pass in 1, 2, or 3.
    public void LoadSaveSlot(int slotNumber)
    {
        // Tell the game which slot we are actively playing on
        PlayerPrefs.SetInt("ActiveSlot", slotNumber);
        PlayerPrefs.Save(); // Lock it in

        Debug.Log("Starting Shift on Save Slot: " + slotNumber);
        SceneManager.LoadScene(1); // Load the Pharmacy!
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}