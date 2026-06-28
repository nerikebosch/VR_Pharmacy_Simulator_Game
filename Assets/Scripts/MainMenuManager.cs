using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes!

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Starting Shift...");
        // This loads the scene at Index 1 (we will set this up in Step 4)
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        // Quits the application (Note: This only works in a built .apk or .exe, not in the Unity Editor)
        Application.Quit();
    }
}