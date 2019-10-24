using UnityEngine;
using Galaxy.Api;

public class LobbyManagementInGame : MonoBehaviour
{
    #region Variables

    private LobbyLeftListener lobbyLeftListener;
    private LobbyDataListener lobbyDataListener;
    private LobbyMemberStateListener lobbyMemberStateListener;

    #endregion

    #region Behaviors

    void OnEnable()
    {
        ListenersInit();
        GameObject.Find("PopUps").GetComponent<PopUps>().GameWaitingForOtherPlayer();
        GalaxyManager.Instance.Matchmaking.SetLobbyMemberData("state", "go");
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
        if (lobbyMemberStateListener == null) lobbyMemberStateListener = new LobbyMemberStateListener();
    }

    private void ListenersDispose()
    {
        if (lobbyLeftListener != null) lobbyLeftListener.Dispose();
        if (lobbyDataListener != null) lobbyDataListener.Dispose();
        if (lobbyMemberStateListener != null) lobbyMemberStateListener.Dispose();
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
            if (memberID != new GalaxyID(0)) LobbyMemberDataUpdated(lobbyID, memberID);
        }

        void LobbyMemberDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
        {
            if (AllMembersGo(lobbyID, memberID))
            {
                matchmaking.SetLobbyData(lobbyID, "state", "go");
                GameObject.Find("GameManager").GetComponent<Online2PlayerGameManager>().enabled = true;
                GameObject.Find("PopUps").GetComponent<PopUps>().ClosePopUps();
            }
        }

        bool AllMembersGo(GalaxyID lobbyID, GalaxyID memberID)
        {
            uint go = 0;

            // Check how many players are in game
            for (uint i = 0; i < 2; i++)
            {
                if (matchmaking.GetLobbyMemberData(matchmaking.GetLobbyMemberByIndex(lobbyID, i), "state") == "go")
                {
                    go++;
                }
            }

            return (go == 2) ? true : false;

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
                    if (!GameObject.Find("Online2PlayerGameEnd")) HostLeftLobby();
                }
                LobbyLeft(lobbyID);
            }
            else
            {
                Debug.LogWarning("OnLobbyLeft failure " + leaveReason);
            }
        }

        void HostLeftLobby()
        {
            Debug.Log("Host left the lobby");
            GameObject.Find("PopUps").GetComponent<PopUps>().ClosePopUps();
            GameObject.Find("PopUps").GetComponent<PopUps>().GameHostLeftLobby();
            GameManager.Instance.GameFinished = true;
        }

        void LobbyLeft(GalaxyID lobbyID)
        {
            matchmaking.CurrentLobbyID = null;
            matchmaking.LobbyOwnerID = null;
            matchmaking.leftOnMyOwn = false;
            GalaxyManager.Instance.ShutdownNetworking();
            matchmaking.ShutdownLobbyChat();
            matchmaking.ShutdownLobbyManagementInGame();
            Debug.Log("Lobby " + lobbyID + " left");
        }

    }

    /* Informs about the event of lobby member state change */
    private class LobbyMemberStateListener : GlobalLobbyMemberStateListener
    {
        public override void OnLobbyMemberStateChanged(GalaxyID lobbyID, GalaxyID memberID, LobbyMemberStateChange memberStateChange)
        {
            Debug.Log(string.Format("OnLobbyMemberStateChanged lobbyID: {0} memberID: {1} change: {2}", lobbyID, memberID, memberStateChange));
            if (memberStateChange != LobbyMemberStateChange.LOBBY_MEMBER_STATE_CHANGED_ENTERED)
            {
                if (!GameObject.Find("Online2PlayerGameEnd")) ClientLeftLobby();
            }
        }

        private void ClientLeftLobby()
        {
            Debug.Log("Client left the lobby.");
            GameObject.Find("PopUps").GetComponent<PopUps>().ClosePopUps();
            GameObject.Find("PopUps").GetComponent<PopUps>().GameClientLeftLobby();
            GameManager.Instance.GameFinished = true;
        }

    }

    #endregion

}
