using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawn : NetworkBehaviour
{
    [SerializeField]
    public Transform[] SpawnPoints;
    [SerializeField] private GameObject playerPrefab;
    private int nextSpawnIndex = 0;

    [Header("Player's Bases")]
    public BaseOwnership[] playerBases;

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

        Transform spawnPoint = SpawnPoints[nextSpawnIndex % SpawnPoints.Length];
        nextSpawnIndex++;

        GameObject car = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        car.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);


        // give belongs to base(pair base with players)
        int index = (nextSpawnIndex - 1) % playerBases.Length;
        if (playerBases != null && playerBases.Length > index)
        {
            playerBases[index].OwnerPlayerId.Value = playerID;
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
