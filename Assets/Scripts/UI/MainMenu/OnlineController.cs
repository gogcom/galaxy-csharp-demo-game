using UnityEngine;

public class OnlineController : MonoBehaviour
{

    void OnEnable()
    {
        GalaxyManager.Instance.StartMatchmaking();
        GalaxyManager.Instance.Matchmaking.ShutdownAllMatchmakingClasses();
    }

}
