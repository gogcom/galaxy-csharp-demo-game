using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Galaxy.Api;
using Helpers;

public class Matchmaking : MonoBehaviour
{

    #region Variables

    public uint lobbyCount = 0;
    public List<GalaxyID> lobbyList = new List<GalaxyID>();

    // Variables for storing lobbies data
    private GalaxyID currentLobbyID = null;
    public GalaxyID CurrentLobbyID { get { return currentLobbyID; } set { currentLobbyID = value; } }

    // Variables for storing lobby members data
    public string lobbyName = "";
    static GalaxyID lobbyOwnerID = null;
    public GalaxyID LobbyOwnerID { get { return lobbyOwnerID; } set { lobbyOwnerID = value; } }

    #endregion

    #region Behaviours

    void OnDisable()
    {
        // Make sure that all listeners are properly disposed when Matchmaking is closed.
        LobbyBrowsingListenersDispose();
        LobbyCreationListenersDispose();
        LobbyChatListenersDispose();
        LobbyManagmentMainMenuListenersDispose();
        LobbyManagmentInGameListenersDispose();
    }

    #endregion

    #region Methods

    /* Requests list of available lobbies
    Note: Private lobbies will not be retrieved */
    public void RequestLobbyList(bool allowFull = false)
    {
        Debug.Log("Requesting lobby list");
        try
        {
            Debug.Assert(lobbyListListenerBrowsing != null);
            GalaxyInstance.Matchmaking().RequestLobbyList(allowFull, lobbyListListenerBrowsing);
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
            Debug.Assert(lobbyDataRetrieveListenerBrowsing != null);
            GalaxyInstance.Matchmaking().RequestLobbyData(lobbyID, lobbyDataRetrieveListenerBrowsing);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve lobby " + lobbyID + " data for reason: " + e);
        }
    }

    // Joins a specified lobby
    public void JoinLobby(GalaxyID lobbyID)
    {
        Debug.Log("Joining lobby " + lobbyID);
        try
        {
            Debug.Assert(lobbyEnteredListenerBrowsing != null);
            GalaxyInstance.Matchmaking().JoinLobby(lobbyID, lobbyEnteredListenerBrowsing);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not join lobby " + lobbyID + " for reason: " + e);
        }
    }

    // Creates a lobby with specified parameters
    public void CreateLobby(string gameName, LobbyType lobbyType, uint maxMembers, bool joinable, 
        LobbyTopologyType lobbyTopologyType)
    {
        Debug.Log("Creating a lobby");
        try
        {
            lobbyName = gameName;
            Debug.Assert(lobbyCreatedListener != null);
            GalaxyInstance.Matchmaking().CreateLobby(lobbyType, maxMembers, joinable, lobbyTopologyType, lobbyCreatedListener);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not create lobby for reason: " + e);
        }
    }

    // Leaves currently entered lobby
    public void LeaveLobby()
    {
        Debug.Log("Leaving lobby " + currentLobbyID);
        try
        {
            Debug.Assert(currentLobbyID != null);
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

    // Checks if user is the lobby owner
    public bool IsCurrentUserLobbyOwner() {
        return GalaxyManager.Instance.MyGalaxyID == LobbyOwnerID;
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
            Debug.LogWarning("Could not send invitation to: " + userID + " Connection string: " + connectionString +
                " for reason: " + e);
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

    #region Lobby browsing specific listeners

    public void LobbyBrowsingListenersInit()
    {
        Listener.Create(ref lobbyListListenerBrowsing);
        Listener.Create(ref lobbyDataRetrieveListenerBrowsing);
        Listener.Create(ref lobbyEnteredListenerBrowsing);
    }

    public void LobbyBrowsingListenersDispose()
    {
        Listener.Dispose(ref lobbyListListenerBrowsing);
        Listener.Dispose(ref lobbyDataRetrieveListenerBrowsing);
        Listener.Dispose(ref lobbyEnteredListenerBrowsing);
    }

    /* Informs about the event of retrieving the list of available lobbies
    Callback to methods:
    Matchmaking.RequestLobbyList(bool allowFull = false) */
    private LobbyListListenerBrowsing lobbyListListenerBrowsing;
    private class LobbyListListenerBrowsing : ILobbyListListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyList(uint count, LobbyListResult result)
        {
            if (result != LobbyListResult.LOBBY_LIST_RESULT_SUCCESS)
            {
                Debug.LogWarning("OnLobbyList failure reason: " + result);
                return;
            }
            Debug.Log(count + " lobbies OnLobbyList");
            matchmaking.lobbyCount = count;
            if (count == 0)
            {
                GameObject.Find("OnlineBrowserScreen").GetComponent<OnlineBrowserController>().DisplayLobbyList(null);
                return;
            }
            for (uint i = 0; i < count; i++)
            {
                GalaxyID lobbyID = GalaxyInstance.Matchmaking().GetLobbyByIndex(i);
                Debug.Log("Requesting lobby data for lobby " + i + " with lobbyID " + lobbyID.ToString());
                matchmaking.RequestLobbyData(lobbyID);
            }
        }

    }

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private LobbyDataRetrieveListenerBrowsing lobbyDataRetrieveListenerBrowsing;
    private class LobbyDataRetrieveListenerBrowsing : ILobbyDataRetrieveListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        List<object[]> lobbyListWithDetails = new List<object[]>();
        public int lobbiesWithDataRetrievedCount = 0;

        public override void OnLobbyDataRetrieveSuccess(GalaxyID lobbyID)
        {
            Debug.Log("Data retrieved for " + lobbiesWithDataRetrievedCount + " lobbies out of " + matchmaking.lobbyCount);
            string name = GalaxyManager.Instance.Matchmaking.GetLobbyData(lobbyID, "name");
            string ping = GalaxyManager.Instance.Matchmaking.GetPingWith(lobbyID).ToString();
            object[] lobbyWithDetails = {name, ping, lobbyID};
            lobbyListWithDetails.Add(lobbyWithDetails);
            lobbiesWithDataRetrievedCount ++;
            if (lobbiesWithDataRetrievedCount >= matchmaking.lobbyCount) 
            {
                GameObject.Find("OnlineBrowserScreen").GetComponent<OnlineBrowserController>().DisplayLobbyList(lobbyListWithDetails);
                lobbiesWithDataRetrievedCount = 0;
                matchmaking.lobbyCount = 0;
                lobbyListWithDetails.Clear();
                lobbyListWithDetails.TrimExcess();
            }
        }

        public override void OnLobbyDataRetrieveFailure(GalaxyID lobbyID, FailureReason failureReason)
        {
            Debug.LogWarning("LobbyDataRetrieveListenerBrowsing failure reason: " + failureReason);
        }

    }

    /* Informs about the event of entering a lobby
    Callback for methods:
    Matchmaking.JoinLobby(GalaxyID lobbyID) */
    private LobbyEnteredListenerBrowsing lobbyEnteredListenerBrowsing;
    private class LobbyEnteredListenerBrowsing : ILobbyEnteredListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        Friends friends = GalaxyManager.Instance.Friends;

        public override void OnLobbyEntered(GalaxyID lobbyID, LobbyEnterResult _result)
        {
            if (_result != LobbyEnterResult.LOBBY_ENTER_RESULT_SUCCESS) 
            {
                Debug.Log("LobbyEnteredListenerBrowsing failed");
                GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineBrowser);
                GameObject.Find("PopUps").GetComponent<PopUps>().PopUpWithClosePopUpsButton("Could not join lobby\nReason: " +
                    _result.ToString(), "Back");
                return;
            }
            Debug.Log("LobbyEnteredListenerBrowsing succeded");
            matchmaking.CurrentLobbyID = lobbyID;
            matchmaking.LobbyOwnerID = matchmaking.GetLobbyOwner(lobbyID);
            matchmaking.SetLobbyMemberData("state", "notReady");
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineWait);
            friends.SetRichPresence("status", "In online lobby");
            friends.SetRichPresence("connect", "--JoinLobby=" + lobbyID);
            matchmaking.LobbyManagmentMainMenuListenersInit();
            matchmaking.LobbyChatListenersInit();
            matchmaking.LobbyBrowsingListenersDispose();
        }

    }

    #endregion

    #region Lobby creation specific listeners
    public void LobbyCreationListenersInit()
    {
        Listener.Create(ref lobbyCreatedListener);
    }

    public void LobbyCreationListenersDispose()
    {
        Listener.Dispose(ref lobbyCreatedListener);
    }

    /* Informs about the event of creating a lobby
    Callback to methods:
    Matchmaking.CreateLobby(LobbyType lobbyType, uint maxMembers, bool joinable, LobbyTopologyType lobbyTopologyType) */
    private LobbyCreatedListener lobbyCreatedListener;
    private class LobbyCreatedListener : ILobbyCreatedListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        Friends friends = GalaxyManager.Instance.Friends;

        public override void OnLobbyCreated(GalaxyID lobbyID, LobbyCreateResult _result)
        {
            if (_result != LobbyCreateResult.LOBBY_CREATE_RESULT_SUCCESS)
            {
                Debug.Log("LobbyCreatedListenerCreation failed");
                GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineCreate);
                GameObject.Find("PopUps").GetComponent<PopUps>().PopUpWithClosePopUpsButton("Could not create lobby\nReason: " +
                    _result.ToString(), "back");
                return;
            }
            Debug.Log("LobbyCreatedListenerCreation succeded");
            matchmaking.SetLobbyData(lobbyID, "name", matchmaking.lobbyName);
            matchmaking.SetLobbyData(lobbyID, "state", "notReady");
            matchmaking.CurrentLobbyID = lobbyID;
            matchmaking.LobbyOwnerID = matchmaking.GetLobbyOwner(lobbyID);
            matchmaking.SetLobbyMemberData("state", "notReady");
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineWait);
            friends.SetRichPresence("status", "In online lobby");
            friends.SetRichPresence("connect", "--JoinLobby=" + lobbyID);
            matchmaking.LobbyManagmentMainMenuListenersInit();
            matchmaking.LobbyChatListenersInit();
            matchmaking.LobbyCreationListenersDispose();
        }

    }

    #endregion

    #region Lobby managment in game global listeners

    public void LobbyManagmentInGameListenersInit() 
    {
        Listener.Create(ref lobbyDataListenerInGame);
        Listener.Create(ref lobbyLeftListenerInGame);
        Listener.Create(ref lobbyMemberStateListenerInGame);
    }

    public void LobbyManagmentInGameListenersDispose()
    {
        Listener.Dispose(ref lobbyDataListenerInGame);
        Listener.Dispose(ref lobbyLeftListenerInGame);
        Listener.Dispose(ref lobbyMemberStateListenerInGame);
    }

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private LobbyDataListenerInGame lobbyDataListenerInGame;
    private class LobbyDataListenerInGame : GlobalLobbyDataListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
        {
            Debug.Log("LobbyID: " + lobbyID + "\nMemberID: " + memberID);
            if (memberID.IsValid())
            {
                if (AllMembersGo(lobbyID) && matchmaking.IsCurrentUserLobbyOwner())
                {
                    matchmaking.SetLobbyData(lobbyID, "state", "go");
                }
                return;
            }
            if (matchmaking.GetLobbyData(lobbyID, "state") == "go")
            {
                GameObject.Find("GameManager").GetComponent<Online2PlayerGameManager>().enabled = true;
                GameObject.Find("PopUps").GetComponent<PopUps>().ClosePopUps();
            }
        }

        bool AllMembersGo(GalaxyID lobbyID)
        {
            uint lobbyMembersCount = matchmaking.GetNumLobbyMembers(lobbyID);
            if (lobbyMembersCount < 2)
                return false;
            for (uint i = 0; i < lobbyMembersCount; i++)
            {
                if (matchmaking.GetLobbyMemberData(matchmaking.GetLobbyMemberByIndex(lobbyID, i), "state") != "go")
                    return false;
            }
            return true;
        }
        
    }

    /* Informs about the event of leaving a lobby 
    Callback for methods: 
    Matchmaking.LeaveLobby() */
    private LobbyLeftListenerInGame lobbyLeftListenerInGame;
    private class LobbyLeftListenerInGame : GlobalLobbyLeftListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        public override void OnLobbyLeft(GalaxyID lobbyID, LobbyLeaveReason leaveReason)
        {
            if (leaveReason != LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT)
                GameObject.Find("PopUps").GetComponent<PopUps>().PopUpWithLoadSceneButton("Connection to lobby lost\nReason: " +
                    leaveReason, "Back");
            matchmaking.CurrentLobbyID = null;
            matchmaking.LobbyOwnerID = null;
            GameManager.Instance.GameFinished = true;
            GalaxyManager.Instance.ShutdownNetworking();
            matchmaking.LobbyManagmentInGameListenersDispose();
            matchmaking.LobbyChatListenersDispose();
        }

    }

    /* Informs about the event of lobby member state change */
    private LobbyMemberStateListenerInGame lobbyMemberStateListenerInGame;
    private class LobbyMemberStateListenerInGame : GlobalLobbyMemberStateListener
    {
        public override void OnLobbyMemberStateChanged(GalaxyID lobbyID, GalaxyID memberID, LobbyMemberStateChange memberStateChange)
        {
            Debug.Log("OnLobbyMemberStateChanged lobbyID: " + lobbyID + " memberID: " + memberID + " change: " + memberStateChange);
            if (memberStateChange != LobbyMemberStateChange.LOBBY_MEMBER_STATE_CHANGED_ENTERED &&
                GameObject.Find("Online2PlayerGameEnd") == null)
            {
                GameObject.Find("PopUps").GetComponent<PopUps>().PopUpWithLeaveLobbyButton("Other player left lobby", "Back");
                GameManager.Instance.GameFinished = true;
            }
        }

    }

    #endregion

    #region Lobby managment main menu global listeners

    public void LobbyManagmentMainMenuListenersInit() 
    {
        Listener.Create(ref lobbyLeftListenerMainMenu);
        Listener.Create(ref lobbyDataListenerMainMenu);
        Listener.Create(ref lobbyMemberStateListenerMainMenu);
    }

    public void LobbyManagmentMainMenuListenersDispose()
    {
        Listener.Dispose(ref lobbyLeftListenerMainMenu);
        Listener.Dispose(ref lobbyDataListenerMainMenu);
        Listener.Dispose(ref lobbyMemberStateListenerMainMenu);
    }

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private LobbyDataListenerMainMenu lobbyDataListenerMainMenu;
    private class LobbyDataListenerMainMenu : GlobalLobbyDataListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        Friends friends = GalaxyManager.Instance.Friends;

        public override void OnLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
        {
            Debug.Log("LobbyID: " + lobbyID + "\nMemberID: " + memberID);
            if (memberID.IsValid())
            {
                if (matchmaking.IsCurrentUserLobbyOwner())
                {
                    matchmaking.SetLobbyData(lobbyID, "state", AllMembersReady(lobbyID) ? "ready" : "notReady");
                }
                return;
            }

            GameObject.Find("OnlineWaitScreen").GetComponent<OnlineWaitController>().startGameButton.GetComponent<Button>().
                interactable = (matchmaking.GetLobbyData(lobbyID, "state") == "ready" &&
                matchmaking.IsCurrentUserLobbyOwner());

            if (matchmaking.GetLobbyData(lobbyID, "state") == "steady")
            {
                Debug.Assert(matchmaking.GetLobbyMemberData(GalaxyManager.Instance.MyGalaxyID, "state") == "ready");
                matchmaking.SetLobbyMemberData("state", "steady");
                friends.SetRichPresence("connect", null);
                matchmaking.LobbyManagmentMainMenuListenersDispose();
                matchmaking.LobbyManagmentInGameListenersInit();
                SceneController.Instance.LoadScene(SceneController.SceneName.Online2PlayerGame, true);
            }
        }

        bool AllMembersReady(GalaxyID lobbyID)
        {
            uint lobbyMembersCount = matchmaking.GetNumLobbyMembers(lobbyID);
            if (lobbyMembersCount < 2)
                return false;
            for (uint i = 0; i < lobbyMembersCount; i++)
            {
                if (matchmaking.GetLobbyMemberData(matchmaking.GetLobbyMemberByIndex(lobbyID, i), "state") != "ready")
                    return false;
            }
            return true;
        }
    }

    /* Informs about the event of leaving a lobby 
    Callback for methods: 
    Matchmaking.LeaveLobby() */
    private LobbyLeftListenerMainMenu lobbyLeftListenerMainMenu;
    private class LobbyLeftListenerMainMenu : GlobalLobbyLeftListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        Friends friends = GalaxyManager.Instance.Friends;
        public override void OnLobbyLeft(GalaxyID lobbyID, LobbyLeaveReason leaveReason)
        {
            if (leaveReason != LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT)
                GameObject.Find("PopUps").GetComponent<PopUps>().PopUpWithClosePopUpsButton("Host left the lobby", "Back");
            matchmaking.CurrentLobbyID = null;
            matchmaking.LobbyOwnerID = null;
            matchmaking.lobbyName = "";
            friends.SetRichPresence("connect", null);
            GameObject.Find("OnlineChat").GetComponent<ChatController>().DisposeEntries();
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.Online);
            matchmaking.LobbyManagmentMainMenuListenersDispose();
            matchmaking.LobbyChatListenersDispose();
        }

    }

    /* Informs about the event of lobby member state change */
    private LobbyMemberStateListenerMainMenu lobbyMemberStateListenerMainMenu;
    private class LobbyMemberStateListenerMainMenu : GlobalLobbyMemberStateListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        public override void OnLobbyMemberStateChanged(GalaxyID lobbyID, GalaxyID memberID, LobbyMemberStateChange memberStateChange)
        {
            Debug.Log("OnLobbyMemberStateChanged lobbyID: " + lobbyID + " memberID: " + memberID + " change: " + memberStateChange);
            if (memberStateChange != LobbyMemberStateChange.LOBBY_MEMBER_STATE_CHANGED_ENTERED)
            {
                matchmaking.SetLobbyData(lobbyID, "state", "notReady");
                matchmaking.SetLobbyMemberData("state", "notReady");
            }
        }

    }

    #endregion

    #region Lobby chat global listeners

    public void LobbyChatListenersInit() {
        Listener.Create(ref lobbyMessageListenerChat);
    }

    public void LobbyChatListenersDispose() {
        Listener.Dispose(ref lobbyMessageListenerChat);
    }

	// Lobby message listener
    public LobbyMessageListenerChat lobbyMessageListenerChat;
    public class LobbyMessageListenerChat : GlobalLobbyMessageListener
    {
        public List<Dictionary<string, string>> chatLobbyMessageHistory = new List<Dictionary<string, string>>();
        private Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        private Friends friends = GalaxyManager.Instance.Friends;
        string message = null;

        public override void OnLobbyMessageReceived(GalaxyID lobbyID, GalaxyID senderID, uint messageID, uint messageLength)
        {
            Dictionary<string, string> messageAndSenderDict = new Dictionary<string, string>();
            ChatController chatMenuController = GameObject.Find("OnlineChat").GetComponent<ChatController>();
            Debug.Log("Message from lobby: \'" + lobbyID + "\', sender: \'" + senderID + "\', with value: \'" + message +
                "\' received.");
            message = matchmaking.GetLobbyMessage(matchmaking.CurrentLobbyID, ref senderID, messageID);
            messageAndSenderDict.Add("sender", friends.GetFriendPersonaName(senderID));
            messageAndSenderDict.Add("message", message);
            chatLobbyMessageHistory.Add(messageAndSenderDict);
            if (chatMenuController != null) chatMenuController.DisplayChatMessage(messageAndSenderDict);
            if (GameManager.Instance != null) ((Online2PlayerGameManager)GameManager.Instance).PopChatPrompt();
        }
    }

    #endregion

}