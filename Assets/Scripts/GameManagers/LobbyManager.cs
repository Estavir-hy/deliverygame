using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Text playerCountText;
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private GameObject isStartedButton;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> isStarted = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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
        playerCount.OnValueChanged += UpdatePlayerCountUI;
        isStarted.OnValueChanged += OnGameStarted;

        if (IsServer)
        {
            UpdateCount();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientChanged;
        }

        UpdatePlayerCountUI(0, playerCount.Value);

        if (isStartedButton != null)
        {
            isStartedButton.SetActive(IsServer);
        }
    }

    public override void OnNetworkDespawn()
    {
        playerCount.OnValueChanged -= UpdatePlayerCountUI;
        isStarted.OnValueChanged -= OnGameStarted;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientChanged;
        }

        base.OnNetworkDespawn();
    }

    public override void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        base.OnDestroy();
    }

    private void OnClientChanged(ulong clientId)
    {
        if (!IsServer || !IsSpawned) return;
        UpdateCount();
    }

    private void UpdateCount()
    {
        if (NetworkManager.Singleton == null) return;
        playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    private void UpdatePlayerCountUI(int oldCount, int newCount)
    {
        if (playerCountText != null)
        {
            playerCountText.text = $"{newCount} / 4 Players Connected";
        }
    }

    public void StartGameHostButton()
    {
        if (IsServer)
        {
            isStarted.Value = true;
        }
    }

    private void OnGameStarted(bool wasStarted, bool isNowStarted)
    {
        if (isNowStarted)
        {
            if (UIPanel != null)
            {
                UIPanel.SetActive(false);
            }

            CameraFollow cameraFollow = FindAnyObjectByType<CameraFollow>();
            if (cameraFollow != null && NetworkManager.Singleton.LocalClient.PlayerObject != null)
            {
                cameraFollow.TargetCar = NetworkManager.Singleton.LocalClient.PlayerObject.transform;
            }
        }
    }
}