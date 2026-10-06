using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCargoState :NetworkBehaviour
{
    public NetworkVariable<PlayerTruckState> CurrentState = new NetworkVariable<PlayerTruckState>(PlayerTruckState.Empty);

    public NetworkVariable<int> CargoHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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
        {
            CurrentState.Value = PlayerTruckState.loaded;
            CargoHealth.Value = 5;
        }
    }

    [ServerRpc]
    public void DeliverCargoServerRpc()
    {
        if (CurrentState.Value != PlayerTruckState.loaded) 
            return;

        CurrentState.Value = PlayerTruckState.Empty;
        Debug.Log("has empty");
        int finalScore = Mathf.Max(0, CargoHealth.Value);
        GetComponent<PlayerScore>().AddScore(finalScore);
        CargoHealth.Value = 0;
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

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;
        if (CurrentState.Value != PlayerTruckState.loaded) return;
        if (CargoHealth.Value <= 0) return;

        CargoHealth.Value = Mathf.Max(0,CargoHealth.Value - damage);

        if(CargoHealth.Value <= 0)
        {
            CurrentState.Value = PlayerTruckState.Empty;
        }
    }

}
