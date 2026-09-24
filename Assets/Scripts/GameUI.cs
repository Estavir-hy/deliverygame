using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SocialPlatforms;

public class GameUI : MonoBehaviour
{
    [Header("Match UI")]
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text MaterialText;

    [Header("Win UI")]
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private TMP_Text WinText;

    private MatchManager matchManager;
    private PlayerScore playerScore;
    private MaterialSystem materialSystem;

    private bool winScreenShown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        matchManager = FindAnyObjectByType<MatchManager>();
        materialSystem = FindAnyObjectByType<MaterialSystem>();

        if (WinScreen != null)
        {
            WinScreen.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Find local player score
        FindLocalPlayerScore();
        FindLocalPlayerBase();

        if (matchManager == null)
            return;

        UpdateTimer();
        UpdateScore();
        UpdateMaterialNumber();

        if (matchManager.MatchEnded.Value && !winScreenShown)
        {
            ShowWinScreen();
        }
        
    }

    private void FindLocalPlayerScore()
    {
        // If already found
        if (playerScore != null)
            return;

        if (NetworkManager.Singleton == null)
            return;

        if (!NetworkManager.Singleton.IsClient)
            return;

        NetworkObject localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;

        if (localPlayer == null)
            return;
        playerScore = localPlayer.GetComponent<PlayerScore>();

        if (playerScore != null)
        {
            Debug.Log( $"GameUI found local PlayerScore. " + $"Client ID: {NetworkManager.Singleton.LocalClientId}");
        }
    }

    private void FindLocalPlayerBase()
    {
        if (materialSystem != null)
            return;
        
        if (NetworkManager.Singleton == null)
            return;
        
        if (!NetworkManager.Singleton.IsClient)
            return;
        
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        MaterialSystem[] bases = FindObjectsByType<MaterialSystem>();

        foreach (MaterialSystem baseSystem in bases)
        {
            if (baseSystem.BaseOwnerId.Value == localClientId)
            {
                materialSystem = baseSystem;
                Debug.Log($"GameUI found local base. " +$"Client ID: {localClientId}");
                return;
            }
        }
    }

    private void UpdateTimer()
    {
        float time = Mathf.Max(0f, matchManager.MatchTimer.Value);

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        TimerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateScore()
    {
        if (playerScore == null)
            return;

        ScoreText.text = $"Score: {playerScore.Score.Value}";
    }

    private void UpdateMaterialNumber()
    {
        if (materialSystem == null)
            return;
        
        MaterialText.text = $"Material: {materialSystem.MaterialNum.Value}";
    }

    private void ShowWinScreen()
    {
        winScreenShown = true;

        if (WinScreen != null)
        {
            WinScreen.SetActive(true);
        }

        if (WinText != null)
        {
            WinText.text = "Some Player Win"; // Change text here!
        }
    }
}
