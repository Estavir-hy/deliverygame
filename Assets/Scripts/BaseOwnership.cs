using Unity.Netcode;
using UnityEngine;

public class BaseOwnership : NetworkBehaviour
{
    public NetworkVariable<ulong> OwnerPlayerId = new NetworkVariable<ulong>();

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            OwnerPlayerId.Value = 5;
        }
        Debug.Log($"基地 {gameObject.name} 初始 OwnerPlayerId = {OwnerPlayerId.Value}");
    }
}
