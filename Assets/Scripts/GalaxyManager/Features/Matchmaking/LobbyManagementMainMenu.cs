using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Galaxy.Api;

public class LobbyManagementMainMenu : MonoBehaviour
{
    #region Variables

    private LobbyLeftListener lobbyLeftListener;
    private LobbyDataListener lobbyDataListener;

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
        if (lobbyLeftListener == null) lobbyLeftListener = new LobbyLeftListener();
        if (lobbyDataListener == null) lobbyDataListener = new LobbyDataListener();
    }

    private void ListenersDispose()
    {
        if (lobbyLeftListener != null) lobbyLeftListener.Dispose();
        if (lobbyDataListener != null) lobbyDataListener.Dispose();
    }

    #endregion

    #region Listeners

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private class LobbyDataListener : GlobalLobbyDataListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
        {
            Debug.Log("LobbyID: " + lobbyID + "\nMemberID: " + memberID);
            if (memberID == new GalaxyID(0))
            {
                LobbyDataUpdatedReady(lobbyID);
                LobbyDataUpdatedSteady(lobbyID);
            }
            else
            {
                LobbyMemberDataUpdatedReady(lobbyID);
            }
        }

        void LobbyDataUpdatedReady(GalaxyID lobbyID)
        {
            if (matchmaking.GetLobbyData(lobbyID, "state") == "ready" && GalaxyManager.Instance.MyGalaxyID == matchmaking.LobbyOwnerID)
            {
                GameObject.Find("OnlineWaitScreen").GetComponent<OnlineWaitController>().startGameButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                GameObject.Find("OnlineWaitScreen").GetComponent<OnlineWaitController>().startGameButton.GetComponent<Button>().interactable = false;
            }
        }

        void LobbyDataUpdatedSteady(GalaxyID lobbyID)
        {
            if (matchmaking.GetLobbyData(lobbyID, "state") == "steady")
            {
                Debug.Assert(matchmaking.GetLobbyMemberData(GalaxyManager.Instance.MyGalaxyID, "state") == "ready");
                matchmaking.SetLobbyMemberData("state", "steady");
                SceneController.Instance.LoadScene(SceneController.SceneName.Online2PlayerGame, true);
                matchmaking.ShutdownLobbyManagementMainMenu();
            }
        }

        void LobbyMemberDataUpdatedReady(GalaxyID lobbyID)
        {
            if (GalaxyManager.Instance.MyGalaxyID == matchmaking.LobbyOwnerID)
            {
                matchmaking.SetLobbyData(lobbyID, "state", AllMembersReady(lobbyID) ? "ready" : "notReady");
            }
        }

        bool AllMembersReady(GalaxyID lobbyID)
        {
            uint ready = 0;
            uint lobbyMembersCount = matchmaking.GetNumLobbyMembers(lobbyID);
            // Checks how many players are ready
            for (uint i = 0; i < lobbyMembersCount; i++)
            {
                if (matchmaking.GetLobbyMemberData(matchmaking.GetLobbyMemberByIndex(lobbyID, i), "state") == "ready")
                {
                    ready++;
                }
            }
            return (ready == 2) ? true : false;
        }
    }

    /* Informs about the event of leaving a lobby 
    Callback for methods: 
    Matchmaking.LeaveLobby() */
    private class LobbyLeftListener : GlobalLobbyLeftListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        public override void OnLobbyLeft(GalaxyID lobbyID, LobbyLeaveReason leaveReason)
        {
            if (leaveReason != LobbyLeaveReason.LOBBY_LEAVE_REASON_CONNECTION_LOST)
            {
                if (!matchmaking.leftOnMyOwn)
                {
                    HostLeftLobby();
                }
                LobbyLeft(lobbyID);
            }
            else
            {
                Debug.LogWarning("OnLobbyLeft failure");
            }
        }

        void HostLeftLobby()
        {
            Debug.Log("Host left the lobby");
            GameObject.Find("PopUps").GetComponent<PopUps>().ClosePopUps();
            GameObject.Find("PopUps").GetComponent<PopUps>().MenuHostLeftLobby();
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.Online);
        }

        void LobbyLeft(GalaxyID lobbyID)
        {
            matchmaking.CurrentLobbyID = null;
            matchmaking.LobbyOwnerID = null;
            matchmaking.leftOnMyOwn = false;
            matchmaking.ShutdownLobbyChat();
            matchmaking.ShutdownLobbyManagementMainMenu();
            Debug.Log("Lobby " + lobbyID + " left");
        }

    }

    #endregion

}
