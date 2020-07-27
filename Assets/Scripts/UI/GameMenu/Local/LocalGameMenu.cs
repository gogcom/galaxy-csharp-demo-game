using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalGameMenu : MonoBehaviour
{

    public void BackToMainMenu()
    {
        SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
    }

    public void CloseApp()
    {
        Application.Quit();
    }


}
