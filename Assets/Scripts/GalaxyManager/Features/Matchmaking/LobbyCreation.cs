using UnityEngine;
using Galaxy.Api;

public class LobbyCreation : MonoBehaviour
{
    #region Variables

    private LobbyCreatedListener lobbyCreatedListener;

    #endregion

    #region Behaviors

    void OnEnable()
    {
        ListenersInit();
    }

    void OnDisable()
    {
        ListenersDispose();
    }

    #endregion

    #region Listeners methods

    private void ListenersInit()
    {
        if (lobbyCreatedListener == null) lobbyCreatedListener = new LobbyCreatedListener();
    }

    private void ListenersDispose()
    {
        if (lobbyCreatedListener != null) lobbyCreatedListener.Dispose();
    }

    #endregion

    #region Listeners

    /* Informs about the event of creating a lobby
    Callback to methods:
    Matchmaking.CreateLobby(LobbyType lobbyType, uint maxMembers, bool joinable, LobbyTopologyType lobbyTopologyType) */
    private class LobbyCreatedListener : GlobalLobbyCreatedListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyCreated(GalaxyID lobbyID, LobbyCreateResult _result)
        {
            switch (_result)
            {
                case LobbyCreateResult.LOBBY_CREATE_RESULT_SUCCESS:
                    LobbyCreated(lobbyID);
                break;
                case LobbyCreateResult.LOBBY_CREATE_RESULT_ERROR:
                    Timeout();
                break;
            }
        }

        private void LobbyCreated(GalaxyID lobbyID)
        {
            matchmaking.SetLobbyData(lobbyID, "name", matchmaking.lobbyName);
            matchmaking.SetLobbyData(lobbyID, "state", "notReady");

            matchmaking.CurrentLobbyID = lobbyID;
            matchmaking.LobbyOwnerID = GalaxyInstance.Matchmaking().GetLobbyOwner(lobbyID);
            matchmaking.SetLobbyMemberData("state", "notReady");
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineWait);
        }

        private void Timeout()
        {
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineCreate);
            GameObject.Find("PopUps").GetComponent<PopUps>().MenuCouldNotCreate();
        }

    }

    #endregion

}
