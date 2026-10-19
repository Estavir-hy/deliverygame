
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadLevel(string levelName)
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found.");
            return;
        }

        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("Only the host/server can load the level.");
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public void ReturnToMainMenu()
    {
        if (NetworkManager.Singleton == null) return;

        if (NetworkManager.Singleton.IsServer)
        {
            // Host pulls EVERY connected player into MainMenu
            NetworkManager.Singleton.SceneManager.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            // If a client presses it, they disconnect locally and leave
            NetworkManager.Singleton.Shutdown();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
