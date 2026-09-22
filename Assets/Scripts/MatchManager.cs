using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    [Header("Match Setting")]
    [SerializeField] private float MatchLength = 20f;
    private bool matchEnded;

    public NetworkVariable<float> MatchTimer = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        
        MatchTimer.Value = MatchLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;
        
        if (LobbyManager.Instance != null && !LobbyManager.Instance.isStarted.Value)
            return;
        
        if (MatchTimer.Value <= 0f)
        {
            MatchTimer.Value = 0f;

            if (!matchEnded)
            {
                matchEnded = true;
                EndMatch();
            }       

            return;
        }
       
        MatchTimer.Value -= Time.deltaTime;
        //Debug.Log($"Match Timer: {MatchTimer.Value}");
    }

    private void EndMatch()
    {
        Debug.Log("Game End");
    }
}
