using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject[] menuList = new GameObject[15];
    public GameObject[] galaxySignedIn = new GameObject[2];
    public GameObject[] galaxyLoggedOn = new GameObject[3];
    public GameObject[] dlc = new GameObject[2];
    public GameObject dlcAManager;
    public GameObject dlcBManager;
    public enum MenuEnum {
        Main,
        Local,
        Online,
        OnlineBrowser,
        OnlineJoining,
        OnlineCreate,
        OnlineCreating,
        OnlineWait,
        FriendInvite,
        Statistics,
        Achievements,
        Leaderboards,
        Friends,
        DLCA,
        DLCB
    }

    void Awake ()
    {
        Camera.main.GetComponent<GameCameraController>().CurrentCameraPositionObject = GameObject.Find("CameraPositionMain");
        Camera.main.GetComponent<GameCameraController>().Dynamic = true;
    }

    void OnEnable()
    {
        MouseController.ChangeMouseLockMode(CursorLockMode.None);
        if (GameObject.Find("GalaxyManager"))
        {
            GalaxyCheck();
            DlcCheck();
        }
    }

    public void SwitchMenu(MenuEnum menuToOpen)
    {
        foreach (GameObject menu in menuList)
        {
            menu.SetActive(false);
        }
        menuList[(int)menuToOpen].SetActive(true);
    }

    public void SwitchMenu(string toOpenString)
    {
        MenuEnum toOpenEnum = 0;
        for (int i = 0; i < 15; i++)
        {
            if (toOpenString == ((MenuEnum)i).ToString()) toOpenEnum = (MenuEnum)i;
        }
        SwitchMenu(toOpenEnum);
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    public void LoadScene(string name)
    {
        SceneController.SceneName sceneName;
        if (SceneController.Instance.MapStringToSceneName.TryGetValue(name, out sceneName)) SceneController.Instance.LoadScene(sceneName, true);
    }

    public void GalaxyCheck()
    {
        if (GalaxyManager.Instance.GalaxyFullyInitialized)
        {
            if (GameObject.Find("GalaxyManager").GetComponent<GalaxyManager>().IsSignedIn())
            {
                foreach (GameObject obj in galaxySignedIn)
                {
                    obj.GetComponent<Button>().interactable = true;
                }
            }
            if (GameObject.Find("GalaxyManager").GetComponent<GalaxyManager>().IsLoggedOn())
            {
                foreach (GameObject obj in galaxyLoggedOn)
                {
                    obj.GetComponent<Button>().interactable = true;
                }
            }
        }
    }

    public void DlcCheck()
    {
        GameObject currentObject;

        if (GalaxyManager.Instance.IsDlcInstalled(1751126893))
        {
            dlc[0].GetComponent<Button>().interactable = true;
            if (!GameObject.Find("DlcAManager"))
            {
                currentObject = Instantiate(dlcAManager);
                currentObject.name = "DlcAManager";
            }
        }
        else
        {
            dlc[0].GetComponent<Button>().interactable = false;
            if (GameObject.Find("DlcAManager")) Destroy(GameObject.Find("DlcAManager"));
        }

        if (GalaxyManager.Instance.IsDlcInstalled(1281799802))
        {
            dlc[1].GetComponent<Button>().interactable = true;
            if (!GameObject.Find("DlcBManager"))
            {
                currentObject = Instantiate(dlcBManager);
                currentObject.name = "DlcBManager";
            }
        }
        else
        {
            dlc[1].GetComponent<Button>().interactable = false;
            if (GameObject.Find("DlcBManager")) Destroy(GameObject.Find("DlcBManager"));
        }

    }

}
