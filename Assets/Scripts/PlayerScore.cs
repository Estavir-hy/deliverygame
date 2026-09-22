using Unity.Netcode;
using UnityEngine;

public class PlayerScore: NetworkBehaviour
{
    public NetworkVariable<int> Score = new NetworkVariable<int>(0);

    public void AddScore(int amount)
    {
        Score.Value += amount;
        Debug.Log($"Player{OwnerClientId},current score:{Score.Value}");
    }
}
