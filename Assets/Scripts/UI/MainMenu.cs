
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private InputField ipField;
    [SerializeField] private LevelManager levelManager;

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = false;
    }

    public void HostGame()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;

        if (NetworkManager.Singleton.StartHost())
        {
            levelManager.LoadLevel("Level01");
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }

    public void JoinGame()
    {
        var transport =NetworkManager.Singleton.GetComponent<UnityTransport>();
        string targetIP = string.IsNullOrEmpty(ipField.text) ? "127.0.0.1" : ipField.text;

        transport.SetConnectionData(targetIP, 7777);
        NetworkManager.Singleton.StartClient();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
