using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Match UI")]
    // [SerializeField] private TMP_Text CargoHealthText;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text MaterialText;

    [Header("Timer Color")]
    [SerializeField] private Color TimerStartColor = Color.green;
    [SerializeField] private Color TimerHalfwayColor = Color.yellow;
    [SerializeField] private Color TimerEndColor = Color.red;

    [Header("Timer Warning")]
    [SerializeField] private bool EnableTimerFlashing = true;
    [SerializeField] private float FlashingTimeStart = 20f;
    [SerializeField] private float FlashSpeed = 2f;

    private float timerFlashElapsed = 0f;

    [Header("Material Notification")]
    [SerializeField] private bool EnableMaterialFlashing = true;
    [SerializeField] private float MaterialFlashSpeed = 2f;
    [SerializeField] private float MaterialFlashDuration = 0.5f;
    private int previousMaterialCount = -1;
    private float materialFlashTimer = 0f;
    private float materialFlashElapsed = 0f;

    [Header("Score Notification")]
    [SerializeField] private bool EnableScoreFlashing = true;
    [SerializeField] private float ScoreFlashSpeed = 4f;
    [SerializeField] private float ScoreFlashDuration = 0.5f;
    private int previousScore = -1;
    private float scoreFlashTimer = 0f;

    [Header("Win UI")]
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private TMP_Text WinText;
    [SerializeField] private Button BackToMainMenuButton;
  
    [Header("Level Manager")]
    [SerializeField] private LevelManager levelManager;

    private MatchManager matchManager;
    private PlayerScore playerScore;
    private MaterialSystem materialSystem;

    private bool winScreenShown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        matchManager = FindAnyObjectByType<MatchManager>();
        //materialSystem = FindAnyObjectByType<MaterialSystem>();

        if (WinScreen != null)
        {
            WinScreen.SetActive(false);
        }

        if (BackToMainMenuButton != null)
        {
            BackToMainMenuButton.gameObject.SetActive(false);
            BackToMainMenuButton.onClick.AddListener(OnBackToMainMenuClicked);
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

        float matchLength = matchManager.MatchLengthValue;

        if (matchLength <= 0f)
            return;

        float timeRemaining = time / matchLength;

        // Lerp color from Green to Yellow to Red
        if (timeRemaining > 0.5f)
        {
            float progress = Mathf.InverseLerp(1f, 0.5f, timeRemaining);
            TimerText.color = Color.Lerp(TimerStartColor, TimerHalfwayColor, progress);
        }
        else
        {
            float progress = Mathf.InverseLerp(0.5f, 0f, timeRemaining);
            TimerText.color = Color.Lerp(TimerHalfwayColor, TimerEndColor, progress);
        }

        // Critical time warning
        if (time <= FlashingTimeStart && EnableTimerFlashing)
        {
            timerFlashElapsed += Time.deltaTime;
            float flash = Mathf.PingPong(timerFlashElapsed * FlashSpeed, 1f);
            TimerText.alpha = Mathf.Lerp(0.25f, 1f, flash);
               
        }
        else
        {
            timerFlashElapsed = 0f;
            TimerText.alpha = 1f;
        }
    }

    private void UpdateScore()
    {
        if (playerScore == null)
            return;

        int currentScore = playerScore.Score.Value;

        ScoreText.SetText("Score: {0}", currentScore);

        // Initialize without flashing when the player is first found.
        if (previousScore == -1)
        {
            previousScore = currentScore;
            return;
        }

        // Trigger a flash when the score increases.
        if (currentScore > previousScore)
        {
            if (EnableScoreFlashing)
            {
                scoreFlashTimer = ScoreFlashDuration;
            }
        }

        previousScore = currentScore;

        // Animate the flash.
        if (scoreFlashTimer > 0f && EnableScoreFlashing)
        {
            scoreFlashTimer -= Time.deltaTime;
            float flash = Mathf.PingPong(Time.time * ScoreFlashSpeed, 1f);
            ScoreText.alpha = Mathf.Lerp(0.25f, 1f, flash);
        }
        else
        {
            scoreFlashTimer = 0f;
            ScoreText.alpha = 1f;
        }
    }

    private void UpdateMaterialNumber()
    {
        if (materialSystem == null)
            return;

        int currentMaterialCount = materialSystem.MaterialNum.Value;

        MaterialText.SetText("Material: {0}", currentMaterialCount);

        // Initialize without flashing when the base is first found.
        if (previousMaterialCount == -1)
        {
            previousMaterialCount = currentMaterialCount;
            return;
        }

        // Trigger a flash when materials increase.
        if (currentMaterialCount > previousMaterialCount)
        {
            materialFlashTimer = MaterialFlashDuration;
            materialFlashElapsed = 0f;
        }

        previousMaterialCount = currentMaterialCount;

        // Animate the flash.
        if (materialFlashTimer > 0f && EnableMaterialFlashing)
        {
            materialFlashTimer -= Time.deltaTime;
            materialFlashElapsed += Time.deltaTime;
            float flash = Mathf.PingPong(Time.time * MaterialFlashSpeed, 1f);
            MaterialText.alpha = Mathf.Lerp(0.25f, 1f, flash);
        }
        else
        {
            materialFlashTimer = 0f;
            materialFlashElapsed = 0f;
            MaterialText.alpha = 1f;
        }
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
            WinText.text = matchManager.LeaderboardText.Value.ToString();
        }

        if (BackToMainMenuButton != null)
        {
            BackToMainMenuButton.gameObject.SetActive(true);
        }
    }

    private void OnBackToMainMenuClicked()
    {
        if (levelManager == null)
        {
            Debug.LogError("LevelManager reference is missing.");
            return;
        }

        levelManager.ReturnToMainMenu();
    }
}
