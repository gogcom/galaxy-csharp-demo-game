using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameEnd : MonoBehaviour
{

    public void ToMainMenu()
    {
        if (GalaxyManager.Instance.Matchmaking.CurrentLobbyID != null) GalaxyManager.Instance.Matchmaking.LeaveLobby();
        SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
    }

}
