using UnityEngine;

public class CargoPickupPoint:MonoBehaviour, IInteractable
{
    private PlayerCargoState _nearbyPlayerCargo;

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerCargoState cargo))
            _nearbyPlayerCargo = cargo; 
        Debug.Log("find something enter F");      
    }

    void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerCargoState cargo) && cargo == _nearbyPlayerCargo)
            _nearbyPlayerCargo = null;
        Debug.Log("find something exit F");
    }

    public void Interact()
    {
        _nearbyPlayerCargo?.PickupCargoServerRpc();
    }
}
