using UnityEngine;

public class CargoDeliveryPoint:MonoBehaviour,IInteractable
{
    private PlayerCargoState _nearbyCargoState;

    public void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerCargoState cargo))
            _nearbyCargoState = cargo;
        Debug.Log("find something enter");
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerCargoState cargo) && cargo == _nearbyCargoState)
            _nearbyCargoState = null;
        Debug.Log("find something exit");
    }


    public void Interact()
    {
        _nearbyCargoState?.DeliverCargoServerRpc();    
    }
}
