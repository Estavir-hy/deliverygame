using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    [Header("Match Setting")]
    [SerializeField] private float MatchLength = 20f;

    public NetworkVariable<float> MatchTimer = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> MatchEnded = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        
        MatchTimer.Value = MatchLength;
        MatchEnded.Value = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;
        
        if (LobbyManager.Instance != null && !LobbyManager.Instance.isStarted.Value)
            return;

        if (MatchEnded.Value)
            return;
        
        if (MatchTimer.Value <= 0f)
        {
            MatchTimer.Value = 0f;
            EndMatch();
            return;
        }
       
        MatchTimer.Value -= Time.deltaTime;
        //Debug.Log($"Match Timer: {MatchTimer.Value}");
    }

    private void EndMatch()
    {
        MatchEnded.Value = true;
        Debug.Log("Game End");
    }
}
