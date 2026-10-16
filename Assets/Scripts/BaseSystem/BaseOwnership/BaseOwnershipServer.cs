using UnityEngine;

public class BaseOwnershipServer : BaseOwnership
{
    public override void OnNetworkSpawn()
    {
            OwnerPlayerId.Value = 5;
    }
}
