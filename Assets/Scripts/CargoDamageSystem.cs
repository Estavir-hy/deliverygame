using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(PlayerCargoState))]

public class CargoDamageSystem : NetworkBehaviour
{
    [Header("Damage speed range")]
    public float highSpeedDef = 7f;
    public float lowSpeedDef = 5f;
    [Header("Crash settings")]
    public LayerMask groundLayer;
    public float damageCooldown = 0.6f;

    private CarController _carController;
    private PlayerCargoState _cargoState;
    private float cooldownTimer;



    private float[] _history = new float[30];

    public override void OnNetworkSpawn()
    {
        _carController = GetComponent<CarController>();
        _cargoState = GetComponent<PlayerCargoState>();
    }


    private void Update()
    {
        //Debug.Log(_carController.GetCurrentSpeed());
        if(!IsServer) return;

        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private float GetRecentMax()
    {
        float m = 0f;
        foreach (var s in _history) m = Mathf.Max(m, s);
        return m;
    }




    private void OnCollisionEnter(Collision collision)
    {
        if(!IsServer) return;
        if(_cargoState == null || _carController == null) return;
        
        if(cooldownTimer > 0) return;

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            return;

        bool isWallHit = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y < 0.7f) { isWallHit = true; break; }
        }
        if (!isWallHit) return;

        float peak = GetRecentMax();
        int damage = calculateDamage(peak);


        if(damage > 0)
        {
            _cargoState.TakeDamage(damage);
            cooldownTimer = damageCooldown;
            
        }

    }

    private int calculateDamage(float speed)
    {
        if(speed >= highSpeedDef)
            return 2;
        else if(speed >= lowSpeedDef)
        return 1;
        return 0;
    }


}
