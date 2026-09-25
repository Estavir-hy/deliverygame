using UnityEngine;

public class CargoPickupPoint:MonoBehaviour, IInteractable
{
    private PlayerCargoState _nearbyPlayerCargo;
    [Header("Time")]
    public float StayTime = 2;
    public float timer;
    public bool isCounting = false;

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
        if(_nearbyPlayerCargo.CurrentState.Value == PlayerTruckState.loaded)
        {
            return;
        }

        timer = 0;
        isCounting = true;

    }
    public void Update()
    {
        if(!isCounting)
            return;

        timer += Time.deltaTime;

        if(timer >= StayTime)
        {
            _nearbyPlayerCargo?.PickupCargoServerRpc();
            isCounting = false;
            timer = 0;
        }

        
    }
}
