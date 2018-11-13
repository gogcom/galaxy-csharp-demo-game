using UnityEngine;
using Galaxy.Api;
using UnityEngine.UI;

public class OnlineCreateController : MonoBehaviour
{
    public Toggle privacy;
    public InputField gameName;
    public Text message;

    // Creates lobby
    public void CreateLobby()
    {
        LobbyTopologyType lobbyTopologyTypeFCM = Galaxy.Api.LobbyTopologyType.LOBBY_TOPOLOGY_TYPE_FCM;
        uint maxMembers = 2;
        LobbyType lobbyPrivacy;
        if (!(gameName.text == ""))
        {
            if (privacy.isOn)
            {
                lobbyPrivacy = Galaxy.Api.LobbyType.LOBBY_TYPE_PRIVATE;
            }
            else {
                lobbyPrivacy = Galaxy.Api.LobbyType.LOBBY_TYPE_PUBLIC;
            }
            GalaxyManager.Instance.Matchmaking.lobbyName = gameName.text;
            GalaxyManager.Instance.Matchmaking.CreateLobby(lobbyPrivacy, maxMembers, true, lobbyTopologyTypeFCM);
            message.gameObject.SetActive(false);
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineCreating);
        }
        else {
            Debug.Log("Game name can't be empty");
            message.gameObject.SetActive(true);
        }
    }

}
