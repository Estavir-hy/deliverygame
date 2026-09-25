using UnityEngine;

public class CargoDeliveryPoint:MonoBehaviour,IInteractable
{
    private PlayerCargoState _nearbyCargoState;
    private BaseOwnership _baseOwnerShip;


    public void Awake()
    {
        _baseOwnerShip = GetComponent<BaseOwnership>();
    }
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
        if(_nearbyCargoState == null || _baseOwnerShip == null)
        {
            return;
        }

        if(_baseOwnerShip.OwnerPlayerId.Value != _nearbyCargoState.OwnerClientId)
        {
            Debug.Log("IT IS NOT THE BASE");
            return;
        }

        Debug.Log($"【交付校验】基地归属ID：{_baseOwnerShip.OwnerPlayerId.Value} | 当前玩家ID：{_nearbyCargoState.OwnerClientId}");
        _nearbyCargoState?.DeliverCargoServerRpc(_baseOwnerShip.OwnerPlayerId.Value);    
    }
}
