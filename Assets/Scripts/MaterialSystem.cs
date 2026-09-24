using Unity.Netcode;
using UnityEngine;

public class MaterialSystem : NetworkBehaviour, IInteractable
{
    [Header("Material Data")]

    public NetworkVariable<int> MaterialNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<ulong> BaseOwnerId = new NetworkVariable<ulong>(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float Timer;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        MaterialNum .Value = 0;
        Timer = 0;
    }

    void Update()
    {
        if (!IsServer)
            return;

        if (LobbyManager.Instance != null && !LobbyManager.Instance.isStarted.Value) return;
        
        Farm();
    }

    private void Farm()
    {
        if (Timer >= 1f)
        {
            MaterialNum.Value += 1;
            Timer = 0f;

            //Debug.Log($"Materials: {MaterialNum.Value}");
        }
        else
        {
            Timer += Time.deltaTime;
        }
    }

    public void Interact()
    {
        
    }

}
