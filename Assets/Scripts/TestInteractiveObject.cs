using UnityEngine;

public class TestInteractiveObject : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("succeed interact");
    }
}
