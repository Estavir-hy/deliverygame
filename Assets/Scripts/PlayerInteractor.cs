using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private IInteractable currentInteractable;

    void Update()
    {
        if(currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
            Debug.Log("In Area, Press E to interact");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<IInteractable>(out var interactable))
        {
            if(currentInteractable == interactable)
            {
                currentInteractable = null;
            }
        }
    }
}
