using Unity.VisualScripting;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Level setting")]
    [SerializeField] public float LevelTime = 20.0f;
    private float levelTimer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelTimer = LevelTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (levelTimer <= 0)
        {
            EndLevel();
        }
        else
        {
            levelTimer -= Time.deltaTime;
            Debug.Log("Level time: " + levelTimer);
        }
    }

    private void EndLevel()
    {
        // Add Game end UI and option for return to main menu here
        Debug.Log("Game End");
    }
}
