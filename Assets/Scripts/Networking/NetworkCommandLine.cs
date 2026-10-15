using UnityEngine;
using Unity.Netcode;

public class NetworkCommandLine : MonoBehaviour
{
    public static NetworkCommandLine instance;
    public bool IsHosting = false;

    public void Start()
    {
        if (instance == null) instance = this;
    }

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {

            if(IsHosting)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }
            
        }
        else
        {
          //GUILayout.Label($"Mode: {(NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client")}");
        }
    }
    
    
}
