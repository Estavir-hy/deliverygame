using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text TimerText;

    private MatchManager matchManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        matchManager = FindAnyObjectByType<MatchManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (matchManager == null)
            return;

        float time = Mathf.Max(0f, matchManager.MatchTimer.Value);

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        TimerText.text = $"{minutes:00}:{seconds:00}";
    }
}
