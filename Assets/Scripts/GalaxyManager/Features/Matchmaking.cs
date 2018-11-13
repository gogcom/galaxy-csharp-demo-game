using Galaxy.Api;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Matchmaking : MonoBehaviour
{
    #region Variables

    public List<GalaxyID> lobbyList = new List<GalaxyID>();

    // Variables for storing lobbies data
    private GalaxyID currentLobbyID = null;
    public GalaxyID CurrentLobbyID { get { return currentLobbyID; } set { currentLobbyID = value; } }

    // Variables for storing lobby members data
    public string lobbyName = "";
    static GalaxyID lobbyOwnerID;
    public GalaxyID LobbyOwnerID { get { return lobbyOwnerID; } set { lobbyOwnerID = value; } }
    public bool leftOnMyOwn = false;

    #endregion

    #region Methods

    /* Requests list of available lobbies
    Note: Private lobbies will not be retrieved */
    public void RequestLobbyList(bool allowFull = false)
    {
        Debug.Log("Requesting lobby list");
        try
        {
            GalaxyInstance.Matchmaking().RequestLobbyList(allowFull);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve lobby list for reason: " + e);
        }
    }

    /* Requests lobby data for provided lobbyID 
    Note: Must be called before getting any lobby data, 
    unless you are a member of lobby your trying to get data for (you joined said lobby) */
    public void RequestLobbyData(GalaxyID lobbyID)
    {
        Debug.Log("Requesting data for lobby " + lobbyID);
        try
        {
            GalaxyInstance.Matchmaking().RequestLobbyData(lobbyID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve lobby " + lobbyID + " data for reason: " + e);
        }
    }

    // Creates a lobby with specified parameters
    public void CreateLobby(LobbyType lobbyType, uint maxMembers, bool joinable, LobbyTopologyType lobbyTopologyType)
    {
        Debug.Log("Creating a lobby");
        try
        {
            GalaxyInstance.Matchmaking().CreateLobby(lobbyType, maxMembers, joinable, lobbyTopologyType);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not create lobby for reason: " + e);
        }
    }

    // Joins a specified lobby
    public void JoinLobby(GalaxyID lobbyID)
    {
        Debug.Log("Joining lobby " + lobbyID);
        try
        {
            GalaxyInstance.Matchmaking().JoinLobby(lobbyID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not join lobby " + lobbyID + " for reason: " + e);
        }
    }

    // Leaves currently entered lobby
    public void LeaveLobby()
    {
        Debug.Log("Leaving lobby " + currentLobbyID);
        try
        {
            Debug.Assert(currentLobbyID != null);
            leftOnMyOwn = true;
            GalaxyInstance.Matchmaking().LeaveLobby(currentLobbyID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not leave lobby " + currentLobbyID + " for reason: " + e);
        }
    }

    // Sets lobby 'lobbyID' data 'key' to 'value'
    public void SetLobbyData(GalaxyID lobbyID, string key, string value)
    {
        Debug.Log("Trying to set lobby " + lobbyID + " data " + key + " to " + value);
        try
        {
            GalaxyInstance.Matchmaking().SetLobbyData(lobbyID, key, value);
            Debug.Log("Lobby " + lobbyID + " data " + key + " set to " + value);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not set lobby " + lobbyID + " data " + key + " to " + value + " for reason: " + e);
        }
    }

    // Gets the GalaxyID of the lobby owner
    public GalaxyID GetLobbyOwner(GalaxyID lobbyID)
    {
        GalaxyID lobbyOwnerID = null;
        Debug.Log("Trying to get lobby owner id " + lobbyOwnerID);
        try
        {
            lobbyOwnerID = GalaxyInstance.Matchmaking().GetLobbyOwner(lobbyID);
            Debug.Log("Lobby onwer ID received " + lobbyOwnerID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve lobby " + lobbyID + " owner ID for reason: " + e);
        }
        return lobbyOwnerID;
    }

    // Gets lobby 'lobbyID' data 'key'
    public string GetLobbyData(GalaxyID lobbyID, string key)
    {
        string lobbyData = null;
        try
        {
            lobbyData = GalaxyInstance.Matchmaking().GetLobbyData(lobbyID, key);
            Debug.Log("Lobby " + lobbyID + " data " + key + " read " + lobbyData);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve lobby " + lobbyID + " data " + key + " for reason: " + e);
        }
        return lobbyData;
    }

    // Gets lobby 'lobbyID' member GalaxyID by 'index'
    public GalaxyID GetLobbyMemberByIndex(GalaxyID lobbyID, uint index)
    {
        Debug.Log("Getting lobby member " + index + " GalaxyID");
        GalaxyID memberID = null;
        try
        {
            memberID = GalaxyInstance.Matchmaking().GetLobbyMemberByIndex(lobbyID, index);
            Debug.Log("Lobby member " + index + " GalaxyID " + memberID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not get lobby member " + index + " GalaxyID for reason: " + e);
        }
        return memberID;
    }

    // Sets current user lobby member data 'key' to 'value' in currently entered lobby
    public void SetLobbyMemberData(string key, string value)
    {
        Debug.Log("Trying to set lobby " + currentLobbyID + " member data " + key + " to " + value);
        try
        {
            GalaxyInstance.Matchmaking().SetLobbyMemberData(currentLobbyID, key, value);
            Debug.Log("Lobby " + currentLobbyID + " member data " + key + " set to " + value);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not set lobby " + currentLobbyID + " member data " + key + " : " + value + " for reason " + e);
        }
    }

    // Gets data 'key' of specified member 'memberID' in currently entered lobby
    public string GetLobbyMemberData(GalaxyID memberID, string key)
    {
        Debug.Log("Trying to get lobby " + currentLobbyID + " member " + memberID + " data " + key);
        string memberData = "";
        try
        {
            memberData = GalaxyInstance.Matchmaking().GetLobbyMemberData(currentLobbyID, memberID, key);
            Debug.Log("Lobby " + currentLobbyID + " member " + memberID + " data " + key + " read " + memberData);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not read lobby " + currentLobbyID + " member " + memberID + " data " + key + " for reason " + e);
        }
        return memberData;
    }

    // Gets GalaxyID of the second player in currently entered lobby
    public GalaxyID GetSecondPlayerID()
    {
        Debug.Log("Trying to get second player ID");
        GalaxyID secondPlayerID = null;
        List<GalaxyID> membersList = GetAllLobbyMembers();
        Debug.Assert(membersList.Count == 2);
        if (membersList[0] != GalaxyInstance.User().GetGalaxyID())
        {
            secondPlayerID = membersList[0];
        }
        else secondPlayerID = membersList[1];
        Debug.Log("Second player ID " + secondPlayerID);
        return secondPlayerID;
    }

    /* Gets ping of a user or lobby 'galaxyID'
    Note: To get a ping of a lobby or its member you first have to either
    join it or request said lobby data */
    public int GetPingWith(GalaxyID galaxyID)
    {
        int ping = -1;
        try
        {
            ping = GalaxyInstance.Networking().GetPingWith(galaxyID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.Log(e);
        }
        return ping;
    }

    // Gets a number of lobby members currently in the lobby 'lobbyID'
    public uint GetNumLobbyMembers(GalaxyID lobbyID)
    {
        uint lobbyMembersCount = 0;
        try
        {
            lobbyMembersCount = GalaxyInstance.Matchmaking().GetNumLobbyMembers(lobbyID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not get lobby member count for reason " + e);
        }
        return lobbyMembersCount;
    }

    // Gets a list of all lobby members GalaxyIDs
    private List<GalaxyID> GetAllLobbyMembers()
    {
        Debug.Log("Trying to get all lobby members for lobby: " + currentLobbyID);
        List<GalaxyID> membersList = new List<GalaxyID>();
        uint maxMembers = GetNumLobbyMembers(currentLobbyID);
        for (uint i = 0; i < maxMembers; i++)
        {
            membersList.Add(GetLobbyMemberByIndex(currentLobbyID, i));
        }
        return membersList;
    }

    // Shows overlay dialogs for game invites to the currently entered lobby
    public void ShowOverlayInviteDialog()
    {
        string connectionString = "--JoinLobby=" + currentLobbyID.ToString();
        Debug.Log("Trying to open overlay invite dialog");
        try
        {
            GalaxyInstance.Friends().ShowOverlayInviteDialog(connectionString);
            Debug.Log("Showing Galaxy overlay invite dialog");
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not show Galaxy overlay invite dialog for reason: " + e);
        }
    }

    // Sends game invitation to user for the currently entered lobby 
    public void SendInvitation(GalaxyID userID)
    {
        string connectionString = "--JoinLobby=" + currentLobbyID.ToString();
        Debug.Log("Trying to send invitation to " + userID + " Connection string: " + connectionString);
        try
        {
            GalaxyInstance.Friends().SendInvitation(userID, connectionString);
            Debug.Log("Sent invitation to: " + userID + " Connection string: " + connectionString);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not send invitation to: " + userID + " Connection string: " + connectionString + " for reason: " + e);
        }
    }

    // Sends lobby message
    public void SendLobbyMessage(GalaxyID lobbyID, string message)
    {
        bool messageScheduled;
        try
        {
            messageScheduled = GalaxyInstance.Matchmaking().SendLobbyMessage(lobbyID, message);
            if (!messageScheduled) Debug.LogWarning("Message not scheduled for sending");
            else Debug.Log("Message '" + message + "' was scheduled for sending");
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning(e);
        }
    }
    // Gets a lobby message
    public string GetLobbyMessage(GalaxyID lobbyID, ref GalaxyID senderID, uint messageID)
    {
        string message = null;
        GalaxyInstance.Matchmaking().GetLobbyMessage(lobbyID, messageID, ref senderID, out message);
        return message;
    }

    #endregion

    #region Lobby browsing listeners

    private LobbyListListenerBrowsing lobbyListListenerBrowsing;
    private LobbyDataListenerBrowsing lobbyDataListenerBrowsing;
    private LobbyEnteredListenerBrowsing lobbyEnteredListenerBrowsing;

    /* Informs about the event of retrieving the list of available lobbies
    Callback to methods:
    Matchmaking.RequestLobbyList(bool allowFull = false) */
    private class LobbyListListenerBrowsing : ILobbyListListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        List<GalaxyID> lobbyList = GalaxyManager.Instance.Matchmaking.lobbyList;

        public override void OnLobbyList(uint count, LobbyListResult result)
        {
            if (result == LobbyListResult.LOBBY_LIST_RESULT_SUCCESS)
            {
                LobbyListRetrieved(count);
            }
            else {
                Debug.LogWarning("OnLobbyList failure reason: " + result);
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
    private class LobbyDataListenerBrowsing : ILobbyDataListener
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
    private class LobbyEnteredListenerBrowsing : ILobbyEnteredListener
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

    #region Lobby chat listeners

    public LobbyMessageListenerChat lobbyMessageListener;
    public List<Dictionary<string, string>> lobbyMessageHistory = new List<Dictionary<string, string>>();

	// Lobby message listener
    public class LobbyMessageListenerChat : ILobbyMessageListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        string message = null;

        public override void OnLobbyMessageReceived(GalaxyID lobbyID, GalaxyID senderID, uint messageID, uint messageLength)
        {
            Dictionary<string, string> messageAndSenderDict = new Dictionary<string, string>();
            try
            {
                Debug.Log("Lobby " + lobbyID + " Sender " + senderID + " message " + message);
                message = matchmaking.GetLobbyMessage(GalaxyManager.Instance.Matchmaking.CurrentLobbyID, ref senderID, messageID);
                messageAndSenderDict.Add("sender", GalaxyManager.Instance.Friends.GetFriendPersonaName(senderID));
                messageAndSenderDict.Add("message", message);
                matchmaking.lobbyMessageHistory.Add(messageAndSenderDict);
                Debug.Log("New message from " + GalaxyManager.Instance.Friends.GetFriendPersonaName(senderID) + " to lobbyID " + lobbyID + ": " + message);
                if (GameManager.Instance != null) ((Online2PlayerGameManager)GameManager.Instance).PopChatPrompt();
            }
            catch (GalaxyInstance.Error e)
            {
                Debug.LogWarning(e);
            }
        }
    }

    #endregion

    #region Lobby creation listeners

    private LobbyCreatedListenerCreation lobbyCreatedListener;

    /* Informs about the event of creating a lobby
    Callback to methods:
    Matchmaking.CreateLobby(LobbyType lobbyType, uint maxMembers, bool joinable, LobbyTopologyType lobbyTopologyType) */
    private class LobbyCreatedListenerCreation : ILobbyCreatedListener
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

    #region Lobby managment in game listeners

    private LobbyDataListenerInGame lobbyDataListenerInGame;
    private LobbyLeftListenerInGame lobbyLeftListenerInGame;
    private LobbyMemberStateListenerInGame lobbyMemberStateListenerInGame;

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private class LobbyDataListenerInGame : ILobbyDataListener
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
    private class LobbyLeftListenerInGame : ILobbyLeftListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        public override void OnLobbyLeft(GalaxyID lobbyID, bool ioFailure)
        {
            if (!ioFailure)
            {
                if (!matchmaking.leftOnMyOwn)
                {
                    if (!GameObject.Find("Online2PlayerGameEnd")) HostLeftLobby();
                }
                LobbyLeft(lobbyID);
            }
            else
            {
                Debug.LogWarning("OnLobbyLeft failure " + ioFailure);
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
            Debug.Log("Lobby " + lobbyID + " left");
        }

    }

    /* Informs about the event of lobby member state change */
    private class LobbyMemberStateListenerInGame : ILobbyMemberStateListener
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

    #region Lobby managment main menu listeners

    private LobbyLeftListenerMainMenu lobbyLeftListenerMainMenu;
    private LobbyDataListenerMainMenu lobbyDataListenerMainMenu;

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private class LobbyDataListenerMainMenu : ILobbyDataListener
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
    private class LobbyLeftListenerMainMenu : ILobbyLeftListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        public override void OnLobbyLeft(GalaxyID lobbyID, bool ioFailure)
        {
            if (!ioFailure)
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
            Debug.Log("Lobby " + lobbyID + " left");
        }

    }

    #endregion

}