using System.Collections.Generic;
using UnityEngine;

public class CargoDeliveryPoint:MonoBehaviour,IInteractable
{
    private BaseOwnership _baseOwnerShip;

    private readonly List<PlayerCargoState> _nearby = new();


    public void Awake()
    {
        _baseOwnerShip = GetComponent<BaseOwnership>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerCargoState cargo) && !_nearby.Contains(cargo))
            _nearby.Add(cargo);
        Debug.Log("find something enter");
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerCargoState cargo))
            _nearby.Remove(cargo);
        Debug.Log("find something exit");
    }


    public void Interact()
    {
        PlayerCargoState local = _nearby.Find(c => c.IsOwner);
        if (local == null || _baseOwnerShip == null) return;

        if (_baseOwnerShip.OwnerPlayerId.Value != local.OwnerClientId)
        {
            Debug.Log("IT IS NOT THE BASE");
            return;
        }

        Debug.Log($"【交付校验】基地归属ID：{_baseOwnerShip.OwnerPlayerId.Value} | 当前玩家ID：{local.OwnerClientId}");
        local.DeliverCargoServerRpc(_baseOwnerShip.OwnerPlayerId.Value);
    }
}
