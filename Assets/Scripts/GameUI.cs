using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Match UI")]
    [SerializeField] private TMP_Text TimerText;

    [Header("Win UI")]
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private TMP_Text WinText;

    private MatchManager matchManager;
    private bool winScreenShown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        matchManager = FindAnyObjectByType<MatchManager>();

        if (WinScreen != null)
        {
            WinScreen.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (matchManager == null)
            return;

        UpdateTimer();

        if (matchManager.MatchEnded.Value && !winScreenShown)
        {
            ShowWinScreen();
        }
        
    }

    private void UpdateTimer()
    {
        float time = Mathf.Max(0f, matchManager.MatchTimer.Value);

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        TimerText.text = $"{minutes:00}:{seconds:00}";
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
