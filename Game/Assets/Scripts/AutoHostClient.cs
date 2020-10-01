using Mirror;
using UnityEngine;

public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    void Start()
    {
        if (!Application.isBatchMode)
        { //Headless build
            Debug.Log($"=== Client Build ===");
            networkManager.StartClient();
        }
        else
        {
            Debug.Log($"=== Server Build ===");
        }
    }

    /// <summary>
    /// this function will not be used as the game will be only server authority play
    /// </summary>
    public void JoinLocal()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }
}