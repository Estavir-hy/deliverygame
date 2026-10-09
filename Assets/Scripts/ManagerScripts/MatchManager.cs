using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class MatchManager : NetworkBehaviour
{
    [Header("Match Setting")]
    [SerializeField] private float MatchLength = 20f;

    public NetworkVariable<FixedString512Bytes> LeaderboardText = new NetworkVariable<FixedString512Bytes>(new FixedString512Bytes(""), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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
        BuildLeaderboard();
        MatchEnded.Value = true;
        Debug.Log("Game End");
    }

    private void BuildLeaderboard()
    {
        var results = new System.Collections.Generic.List<(ulong ClientId, int Score)>();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject == null)
                continue;
            
            PlayerScore playerScore = client.PlayerObject.GetComponent<PlayerScore>();

            if (playerScore == null)
                continue;

                results.Add((client.ClientId, playerScore.Score.Value));
        }

        // Highest score.
        results = results.OrderByDescending(player => player.Score).ThenBy(player => player.ClientId).ToList();

        StringBuilder leaderboard = new StringBuilder();

        leaderboard.AppendLine("MATCH RESULT");
        leaderboard.AppendLine();

        int currentRank = 0;
        int previousScore = int.MinValue;

        for (int i = 0; i < results.Count; i++)
        {
            var player = results[i];

            if (player.Score != previousScore)
            {
                currentRank = i + 1;
                previousScore = player.Score;
            }

            leaderboard.AppendLine($"{i + 1}. Player {player.ClientId + 1} : {player.Score} points");
        }

        if (results.Count > 0)
        {
            int highestScore = results[0].Score;

            var winners = results
            .Where(player => player.Score == highestScore)
            .Select(player => $"Player {player.ClientId + 1}").ToList();

            leaderboard.AppendLine();

            if (winners.Count == 1)
            {
                leaderboard.Append($"Winner: {winners[0]}!");
            }
            else
            {
                leaderboard.Append($"Tie! Winners: {string.Join(", ", winners)}!");
            }
        
        }
        else
        {
            leaderboard.Append("No players found.");
        }

        LeaderboardText.Value = new FixedString512Bytes(leaderboard.ToString());
    }
}
