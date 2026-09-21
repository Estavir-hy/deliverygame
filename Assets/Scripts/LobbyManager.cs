using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Text playerCountText;
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private GameObject isStartedButton; 

    // NetworkVariable to track the number of connected players
    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // NetworkVariable to track if the game has started
    public NetworkVariable<bool> isStarted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public static LobbyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        // Update the player count UI when the value changes
        playerCount.OnValueChanged += UpdatePlayerCountUI;
        isStarted.OnValueChanged += OnGameStarted;

        if(IsServer)
        {
            UpdateCount();
            NetworkManager.Singleton.OnClientConnectedCallback += (id) => UpdateCount();
            NetworkManager.Singleton.OnClientDisconnectCallback += (id) => UpdateCount();
        }
        // Only show the start button for the host
        if (isStartedButton != null)
        {
            isStartedButton.SetActive(IsServer);
        }
    }

    private void UpdateCount()
    {
        playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    private void UpdatePlayerCountUI(int oldCount, int newCount)
    {
        playerCountText.text = $"{newCount} / 4 Players Connected";
    }

    // Called by the UI button to start the game, only the host can trigger this
    public void StartGameHostButton()
    {
        if(IsServer)
        {
            isStarted.Value = true;
        }
    }

    private void OnGameStarted(bool wasStarted, bool isNowStarted)
    {
        if (isNowStarted)
        {
            // Hide the UI panel when the game starts
            UIPanel.SetActive(false);

            CameraFollow cameraFollow = FindAnyObjectByType<CameraFollow>();
            if(cameraFollow != null && NetworkManager.Singleton.LocalClient.PlayerObject != null)
            {
                cameraFollow.TargetCar = NetworkManager.Singleton.LocalClient.PlayerObject.transform;
            }
        }
    }


}
