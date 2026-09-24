using Unity.Netcode;
using UnityEngine;

public class PlayerScore: NetworkBehaviour
{
    public NetworkVariable<int> Score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void AddScore(int amount)
    {
        if (!IsServer)
            return;
            
        Score.Value += amount;
        
        //Debug.Log($"Player{OwnerClientId},current score:{Score.Value}");
    }
}
