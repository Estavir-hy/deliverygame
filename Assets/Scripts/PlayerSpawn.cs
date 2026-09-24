using Unity.Netcode;
using UnityEngine;

public class PlayerSpawn : NetworkBehaviour
{
    [SerializeField] public Transform[] SpawnPoints;
    [SerializeField] private MaterialSystem[] PlayerBases;
    [SerializeField] private GameObject playerPrefab;
    private int nextSpawnIndex = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnPlayer(client.ClientId);
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer)
            return;
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong playerID)
    {
       if(SpawnPoints.Length == 0)
       {
            Debug.LogError("No spawn points assigned!");
            return;
       }

       if (PlayerBases.Length == 0)
        {
            Debug.LogError("No player bases assigned!");
            return;
        }

        int spawnIndex = nextSpawnIndex % SpawnPoints.Length;

        Transform spawnPoint = SpawnPoints[spawnIndex];
        GameObject car = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        car.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);

        // Assign this base to this player
        if (spawnIndex < PlayerBases.Length)
        {
            PlayerBases[spawnIndex].BaseOwnerId.Value = playerID;
            Debug.Log($"Player {playerID} assigned to Base {spawnIndex}");
        }

        nextSpawnIndex++;
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
