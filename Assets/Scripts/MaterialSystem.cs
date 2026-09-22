using UnityEngine;

public class MaterialSystem : MonoBehaviour, IInteractable
{
    [Header("material data")]
    public float MaterialNum;
    private float Timer;

    void Start()
    {
        MaterialNum = 0;
        Timer = 0;
    }

    public void Farm()
    {
        if(Timer >= 1)
        {
            MaterialNum += 1;
            Timer = 0;
            // Debug the material when its supposed to add not in update!
            //Debug.Log(MaterialNum);
        }
        else
            Timer += Time.deltaTime;
    }

    void Update()
    {
        if (LobbyManager.Instance != null && !LobbyManager.Instance.isStarted.Value) return;
        Farm();
    }

    public void Interact()
    {
        
    }

}
