using UnityEngine;

public class BaseOwnershipClient : BaseOwnership
{
    public override void OnNetworkSpawn()
    {
        OwnerPlayerId.OnValueChanged += OnOwnerChanged;
    }

    private void OnOwnerChanged(ulong oldId, ulong newId)
    {
    }

    public override void OnNetworkDespawn()
    {
        OwnerPlayerId.OnValueChanged -= OnOwnerChanged;
    }
}
