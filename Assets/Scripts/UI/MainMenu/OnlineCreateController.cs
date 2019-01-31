using UnityEngine;
using Galaxy.Api;
using UnityEngine.UI;

public class OnlineCreateController : MonoBehaviour
{
    public Toggle privacy;
    public InputField gameName;
    public Text message;

    void OnEnable()
    {
        // Initialize the required listeners
        GalaxyManager.Instance.Matchmaking.LobbyCreationListenersInit();
    }

    // Creates lobby
    public void CreateLobby()
    {
        LobbyTopologyType lobbyTopologyTypeFCM = Galaxy.Api.LobbyTopologyType.LOBBY_TOPOLOGY_TYPE_FCM;
        uint maxMembers = 2;
        LobbyType lobbyPrivacy;
        if (gameName.text == "")
        {
            Debug.Log("Failure to create lobby: Game name can't be empty");
            message.text = "Game name can't be empty";
            message.gameObject.SetActive(true);
            return;
        }
        else 
        {
            if (privacy.isOn)
            {
                lobbyPrivacy = Galaxy.Api.LobbyType.LOBBY_TYPE_PRIVATE;
            }
            else {
                lobbyPrivacy = Galaxy.Api.LobbyType.LOBBY_TYPE_PUBLIC;
            }
            GalaxyManager.Instance.Matchmaking.CreateLobby(gameName.text, lobbyPrivacy, maxMembers, true, lobbyTopologyTypeFCM);
            message.gameObject.SetActive(false);
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineCreating);
        }
    }

}
