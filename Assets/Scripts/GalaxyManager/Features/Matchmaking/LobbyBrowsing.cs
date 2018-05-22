using System.Collections.Generic;
using UnityEngine;
using Galaxy.Api;

public class LobbyBrowsing : MonoBehaviour
{
    #region Variables

    private LobbyEnteredListener lobbyEnteredListener;
    private LobbyListListener lobbyListListener;
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
        if (lobbyListListener == null) lobbyListListener = new LobbyListListener();
        if (lobbyEnteredListener == null) lobbyEnteredListener = new LobbyEnteredListener();
        if (lobbyDataListener == null) lobbyDataListener = new LobbyDataListener();
    }

    private void ListenersDispose()
    {
        if (lobbyListListener != null) lobbyListListener.Dispose();
        if (lobbyEnteredListener != null) lobbyEnteredListener.Dispose();
        if (lobbyDataListener != null) lobbyDataListener.Dispose();
    }

    #endregion

    #region Listeners

    /* Informs about the event of retrieving the list of available lobbies
    Callback to methods:
    Matchmaking.RequestLobbyList(bool allowFull = false) */
    private class LobbyListListener : GlobalLobbyListListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        List<GalaxyID> lobbyList = GalaxyManager.Instance.Matchmaking.lobbyList;

        public override void OnLobbyList(uint count, bool ioFailure)
        {
            if (!ioFailure)
            {
                LobbyListRetrieved(count);
            }
            else {
                Debug.LogWarning("OnLobbyList failure");
            }
        }

        private void LobbyListRetrieved(uint count)
        {
            Debug.Log(count + " lobbies OnLobbyList");
            if (count == 0)
            {
                GameObject.Find("OnlineBrowserScreen").GetComponent<OnlineBrowserController>().DisplayLobbyList(lobbyList);
            }
            else
            {
                for (uint i = 0; i < count; i++)
                {
                    GalaxyID lobbyID = GalaxyInstance.Matchmaking().GetLobbyByIndex(i);
                    lobbyList.Add(lobbyID);
                    Debug.Log("Requesting lobby data for lobby " + i + " with lobbyID " + lobbyID.ToString());
                    matchmaking.RequestLobbyData(lobbyID);
                }
            }
        }

    }

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private class LobbyDataListener : GlobalLobbyDataListener
    {
        List<GalaxyID> lobbyList = GalaxyManager.Instance.Matchmaking.lobbyList;
        public uint lobbiesWithDataRetrievedCount = 0;

        public override void OnLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
        {
            Debug.Log("LobbyID: " + lobbyID + "\nMemberID: " + memberID);
            if (memberID == new GalaxyID(0)) LobbyDataUpdated();
        }

        void LobbyDataUpdated()
        {
            lobbiesWithDataRetrievedCount++;
            Debug.Log("Data retrieved for " + lobbiesWithDataRetrievedCount + " lobbies out of " + lobbyList.Count);
            if (lobbiesWithDataRetrievedCount >= lobbyList.Count)
            {
                GameObject.Find("OnlineBrowserScreen").GetComponent<OnlineBrowserController>().DisplayLobbyList(lobbyList);
                lobbiesWithDataRetrievedCount = 0;
                lobbyList.Clear();
                lobbyList.TrimExcess();
            }
        }

    }

    /* Informs about the event of entering a lobby
    Callback for methods:
    Matchmaking.JoinLobby(GalaxyID lobbyID) */
    private class LobbyEnteredListener : GlobalLobbyEnteredListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyEntered(GalaxyID lobbyID, LobbyEnterResult _result)
        {
            switch (_result)
            {
                case LobbyEnterResult.LOBBY_ENTER_RESULT_SUCCESS:
                    LobbyEntered(lobbyID);
                break;
                case LobbyEnterResult.LOBBY_ENTER_RESULT_LOBBY_DOES_NOT_EXIST:
                    LobbyEnteringError("Lobby does not exist");
                break;
                case LobbyEnterResult.LOBBY_ENTER_RESULT_LOBBY_IS_FULL:
                    LobbyEnteringError("Lobby is full");
                break;
                case LobbyEnterResult.LOBBY_ENTER_RESULT_ERROR:
                    LobbyEnteringError("Unspecified error");
                break;
            }
        }

        public void LobbyEntered(GalaxyID lobbyID)
        {
            GalaxyManager.Instance.Matchmaking.CurrentLobbyID = lobbyID;
            GalaxyManager.Instance.Matchmaking.LobbyOwnerID = matchmaking.GetLobbyOwner(lobbyID);
            matchmaking.SetLobbyMemberData("state", "notReady");
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineWait);
        }

        public void LobbyEnteringError(string reason)
        {
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineBrowser);
            GameObject.Find("PopUps").GetComponent<PopUps>().MenuCouldNotJoin(reason);
        }

    }

    #endregion
}
