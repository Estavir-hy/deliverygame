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
        }
        else
            Timer += Time.deltaTime;
    }

    void Update()
    {
        Farm();
        //Debug.Log(MaterialNum);
    }

    public void Interact()
    {
        
    }

}
