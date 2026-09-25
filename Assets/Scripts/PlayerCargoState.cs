using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCargoState :NetworkBehaviour
{
    public NetworkVariable<PlayerTruckState> CurrentState = new NetworkVariable<PlayerTruckState>(PlayerTruckState.Empty);

    [Header("MeshRenderer")]
    public MeshRenderer CargoIndecator;

    public void FixedUpdate()
    {
        bool Isloaded = CurrentState.Value == PlayerTruckState.loaded;
        CargoIndecator.enabled = Isloaded;
    }

    [ServerRpc]
    public void PickupCargoServerRpc()
    {
        if (CurrentState.Value == PlayerTruckState.Empty)
            CurrentState.Value = PlayerTruckState.loaded;
        Debug.Log("has loaded cargo");
    }

    [ServerRpc]
    public void DeliverCargoServerRpc()
    {
        if (CurrentState.Value != PlayerTruckState.loaded) 
            return;

        CurrentState.Value = PlayerTruckState.Empty;
        Debug.Log("has empty");
        GetComponent<PlayerScore>().AddScore(1);
    }

    //check for specific base for specific player
    [ServerRpc]
    public void DeliverCargoServerRpc(ulong baseOwnerId)
    {
        if(OwnerClientId != baseOwnerId)
        {
            Debug.Log("it is not my base!");
            return;
        }

        DeliverCargoServerRpc();
    }

}
