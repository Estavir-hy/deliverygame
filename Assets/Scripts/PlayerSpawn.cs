using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawn : NetworkBehaviour
{
    [SerializeField]
    public Transform[] SpawnPoints;
    private int nextSpawnIndex = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        
        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        if (SpawnPoints == null || SpawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnPlayer(client.PlayerObject);
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"CLIENT CONNECTED: {clientId}");

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(
            clientId,
            out NetworkClient client))
        {
            Debug.LogError($"Could not find client {clientId}");
            return;
        }

        Debug.Log($"Client found: {clientId}");
        Debug.Log($"PlayerObject: {client.PlayerObject}");

        if (client.PlayerObject == null)
        {
            Debug.LogError(
                $"CLIENT {clientId} HAS NO PLAYER OBJECT!"
            );

            return;
        }

        SpawnPlayer(client.PlayerObject);
    }

    private void SpawnPlayer(NetworkObject player)
    {
        if (player == null)
        {
            Debug.LogError("Player object doesn't exist!");
            return;
        }

        if (SpawnPoints == null || SpawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        Transform spawnPoint = SpawnPoints[nextSpawnIndex];
        player.transform.SetPositionAndRotation(spawnPoint.position,spawnPoint.rotation);

        Debug.Log($"Player {player.OwnerClientId} spawned at {spawnPoint.position}");

        nextSpawnIndex++;

        if (nextSpawnIndex >= SpawnPoints.Length)
        {
            nextSpawnIndex = 0;
        }
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -=
                OnClientConnected;
        }

        base.OnDestroy();
    }
}
