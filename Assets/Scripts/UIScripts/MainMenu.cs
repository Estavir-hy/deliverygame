using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MainMenu : MonoBehaviour
{
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // Approve the player and tell NGO to notspawn their Player Prefab
        response.Approved = true;
        response.CreatePlayerObject = false;

    }
    public void HostGame()
    {
        // Tell NetworkManager to use our custom approval check
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;

        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Level01", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }

    }
    public void JoinGame()
    {
        NetworkManager.Singleton.StartClient();
    }

     public void QuitGame()
    {
        Debug.Log("Quiting...");
        Application.Quit();
    }
}
