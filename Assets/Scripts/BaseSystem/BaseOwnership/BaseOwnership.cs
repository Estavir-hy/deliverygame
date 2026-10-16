using Unity.Netcode;
using UnityEngine;

public class BaseOwnership : NetworkBehaviour
{
    public NetworkVariable<ulong> OwnerPlayerId = new NetworkVariable<ulong>();
}
