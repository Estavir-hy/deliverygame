using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField]
    public Transform[] SpawnPoints;
    private int nextSpawnIndex = 0;

    void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // Approve all connections for this example
        response.Approved = true;
        response.CreatePlayerObject = true;

        if (SpawnPoints != null && SpawnPoints.Length > 0)
        {
            // Assign the spawn position and rotation based on the next spawn point
            response.Position = SpawnPoints[nextSpawnIndex].position;
            response.Rotation = SpawnPoints[nextSpawnIndex].rotation;
            // Update the next spawn index for the next player
            nextSpawnIndex = (nextSpawnIndex + 1) % SpawnPoints.Length;
        }
        else
        {
            // If no spawn points are defined, use default values
            response.Position = Vector3.zero;
            response.Rotation = Quaternion.identity;
        }

    }
}
