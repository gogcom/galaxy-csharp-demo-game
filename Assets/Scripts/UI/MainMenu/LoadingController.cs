using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{

    public GameObject parent;
    public GameObject prefab;
    private GameObject current;
    public GameObject loadingStatus;
    public RawImage background;
    public RawImage gogLogo;
    public RawImage madeWithUnity;
    public RawImage disclaimer;
    public RawImage title;
    private bool[] displayed = new bool[5];
    private bool galaxyInitialized = false;
    private bool allLogosDisplayed = false;

    void Start()
    {
        StartCoroutine(PrintGalaxyStatus());
        StartCoroutine(DisplaySplashScreens());
        SceneController.Instance.LoadScene(SceneController.SceneName.GameObjects);
    }

    private IEnumerator PrintGalaxyStatus()
    {

        PrintText("Initializing Galaxy SDK...", 0);

        for (int i = 1; i <= 50; i++)
        {

            if (GalaxyManager.Instance.GalaxyFullyInitialized)
            {
                PrintText("Galaxy Initialized", 1);
                PrintText("Signing in...", 2);
                if (GalaxyManager.Instance.IsSignedIn())
                {
                    PrintText(GalaxyManager.Instance.Friends.GetMyUsername() + " signed in", 3);
                    PrintText("Launching game...", 4);
                }
            }

            if (displayed[4]) break;

            yield return new WaitForSeconds(0.1f);

        }

        if (!displayed[4])
        {
            current = Instantiate(prefab, parent.transform);
            current.GetComponent<Text>().text = "Failed to initialize Galaxy within 5s";
            current = Instantiate(prefab, parent.transform);
            current.GetComponent<Text>().text = "Launching game...";
        }

        yield return new WaitForSeconds(2f);

        SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu);

        galaxyInitialized = true;

        if (allLogosDisplayed) StartCoroutine(CloseLoadingScreen());

    }

    private IEnumerator DisplaySplashScreens() 
    {
        madeWithUnity.CrossFadeAlpha(255f,.5f,true);
        gogLogo.CrossFadeAlpha(255f,.5f,true);
        disclaimer.CrossFadeAlpha(255f,.5f,true);
        yield return new WaitForSeconds(4.5f);

        disclaimer.CrossFadeAlpha(1f,.5f,true);
        yield return new WaitForSeconds(.5f);

        title.CrossFadeAlpha(255f,.5f,true);
        yield return new WaitForSeconds(1.5f);

        allLogosDisplayed = true;

        if (galaxyInitialized) StartCoroutine(CloseLoadingScreen());
    }

    private IEnumerator CloseLoadingScreen() 
    {
        loadingStatus.SetActive(false);
        background.CrossFadeAlpha(0f,.5f,true);
        madeWithUnity.CrossFadeAlpha(0f,.5f,true);
        gogLogo.CrossFadeAlpha(0f,.5f,true);
        disclaimer.CrossFadeAlpha(0f,.5f,true);
        title.CrossFadeAlpha(0f,.5f,true);
        yield return new WaitForSeconds(1f);

        SceneController.Instance.SwitchActiveScenesAndUnloadOld(SceneController.SceneName.MainMenu);
    }

    private void PrintText(string text, int index)
    {
        if (index == 0)
        {
            current = Instantiate(prefab, parent.transform);
            current.GetComponent<Text>().text = text;
            displayed[index] = true;
        }
        else if (displayed[index - 1] && !displayed[index])
        {
            current = Instantiate(prefab, parent.transform);
            current.GetComponent<Text>().text = text;
            displayed[index] = true;
        }
    }

}
