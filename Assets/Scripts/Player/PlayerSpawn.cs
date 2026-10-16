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
        if (IsServer)
        {
            foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                SpawnPlayer(client.ClientId);
            }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            SpawnPlayer(clientId);
        }
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

        if(playerBases != null && playerBases.Length > 0)
        {
            int index = (nextSpawnIndex - 1) % playerBases.Length;
            playerBases[index].OwnerPlayerId.Value = playerID;

            MaterialSystem mat = playerBases[index].GetComponent<MaterialSystem>() 
                ?? playerBases[index].GetComponentInParent<MaterialSystem>() 
                ?? playerBases[index].GetComponentInChildren<MaterialSystem>();

            if(mat != null)
            {
                mat.BaseOwnerId.Value = playerID;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -=
                OnClientConnected;
        }

        base.OnNetworkDespawn();
    }

}
