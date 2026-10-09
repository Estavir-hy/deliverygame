using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(PlayerCargoState))]

public class CargoDamageSystem : NetworkBehaviour
{
    [Header("Damage speed range")]
    public float highSpeedDef = 7.5f;
    public float lowSpeedDef = 5f;
    [Header("Crash settings")]
    public LayerMask groundLayer;
    public float damageCooldown = 0.6f;

    private CarController _carController;
    private PlayerCargoState _cargoState;
    private float cooldownTimer;



    private float[] _history = new float[3];
    private int _hIdx;
    private Rigidbody _rb;

    public override void OnNetworkSpawn()
    {
        _carController = GetComponent<CarController>();
        _cargoState = GetComponent<PlayerCargoState>();
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner || _rb == null) return;
        _history[_hIdx] = _rb.linearVelocity.magnitude;
        _hIdx = (_hIdx + 1) % _history.Length;
    }


    private void Update()
    {
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
        if(!IsOwner) return;
        if(_cargoState == null || _carController == null) return;
        
        

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            return;

        bool isWallHit = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y < 0.7f) { isWallHit = true; break; }
        }
        if (!isWallHit) return;

        ReportCrashServerRpc(GetRecentMax());

    }

    [ServerRpc]
    private void ReportCrashServerRpc(float peakSpeed)
    {
        if(cooldownTimer > 0) return;

        peakSpeed = Mathf.Min(peakSpeed, _carController.MaxSpeed+1f); 
        int damage = calculateDamage(peakSpeed);

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
