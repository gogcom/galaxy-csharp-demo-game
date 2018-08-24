using Galaxy.Api;
using System.Collections.Generic;
using UnityEngine;

public class Matchmaking : MonoBehaviour
{
    #region Variables

    public List<GalaxyID> lobbyList = new List<GalaxyID>();

    public LobbyCreation LobbyCreation;
    public LobbyBrowsing LobbyBrowsing;
    public LobbyManagementMainMenu LobbyManagementMainMenu;
    public LobbyManagementInGame LobbyManagementInGame;
    public LobbyChat LobbyChat;

    // Variables for storing lobbies data
    private GalaxyID currentLobbyID = null;
    public GalaxyID CurrentLobbyID { get { return currentLobbyID; } set { currentLobbyID = value; } }

    // Variables for storing lobby members data
    public string lobbyName = "";
    static GalaxyID lobbyOwnerID;
    public GalaxyID LobbyOwnerID { get { return lobbyOwnerID; } set { lobbyOwnerID = value; } }
    public bool leftOnMyOwn = false;

    #endregion

    #region Behaviours

    void OnDisable()
    {
        ShutdownAllMatchmakingClasses();
    }

    #endregion

    #region Methods

    /* Following methods are used to start and shutdown each and every GalaxyManager feature class separately
    Note: Each class closes its own listeners whyn disabled */
    public void StartLobbyCreation()
    {
        if (LobbyCreation == null) LobbyCreation = gameObject.AddComponent<LobbyCreation>();
    }

    public void ShutdownLobbyCreation()
    {
        if (LobbyCreation != null) Destroy(LobbyCreation);
    }

    public void StartLobbyBrowsing()
    {
        if (LobbyBrowsing == null) LobbyBrowsing = gameObject.AddComponent<LobbyBrowsing>();
    }

    public void ShutdownLobbyBrowsing()
    {
        if (LobbyBrowsing != null) Destroy(LobbyBrowsing);
    }

    public void StartLobbyManagementMainMenu()
    {
        if (LobbyManagementMainMenu == null) LobbyManagementMainMenu = gameObject.AddComponent<LobbyManagementMainMenu>();
    }

    public void ShutdownLobbyManagementMainMenu()
    {
        if (LobbyManagementMainMenu != null) Destroy(LobbyManagementMainMenu);
    }

    public void StartLobbyManagementInGame()
    {
        if (LobbyManagementInGame == null) LobbyManagementInGame = gameObject.AddComponent<LobbyManagementInGame>();
    }

    public void ShutdownLobbyManagementInGame()
    {
        if (LobbyManagementInGame != null) Destroy(LobbyManagementInGame);
    }

    public void StartLobbyChat()
    {
        if (LobbyChat == null) LobbyChat = gameObject.AddComponent<LobbyChat>();
    }

    public void ShutdownLobbyChat()
    {
        if (LobbyChat != null) Destroy(LobbyChat);
    }

    public void ShutdownAllMatchmakingClasses()
    {
        ShutdownLobbyCreation();
        ShutdownLobbyBrowsing();
        ShutdownLobbyManagementMainMenu();
        ShutdownLobbyManagementInGame();
        ShutdownLobbyChat();
    }

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

}