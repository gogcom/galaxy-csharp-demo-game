using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalGameEnd : MonoBehaviour
{

    public void ToMainMenu()
    {
        SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
    }

}
