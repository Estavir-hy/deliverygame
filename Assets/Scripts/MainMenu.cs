using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MainMenu : MonoBehaviour
{
    

    public void QuitGame()
    {
        Debug.Log("Quiting...");
        Application.Quit();
    }

    public void HostGame()
    {
        if(NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Level01", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("Fuck you");
        }

    }
    public void JoinGame()
    {
        NetworkManager.Singleton.StartClient();
    }
}
