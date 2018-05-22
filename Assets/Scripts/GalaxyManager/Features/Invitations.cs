using UnityEngine;
using System;
using Galaxy.Api;
using UnityEngine.SceneManagement;

public class Invitations : MonoBehaviour
{
    #region Variables

    private GalaxyID pendingLobbyID = null;

    //  Listeners
    private GameInvitationReceivedListener inviteReceivedListener;
    private GameJoinRequestedListener joinRequestedListener;

    #endregion

    #region Behaviors

    void OnEnable()
    {
        ListenersInit();
        CheckForInviteFromClient();
    }

    void OnDisable()
    {
        ListenersDispose();
    }

    #endregion

    #region Listeners methods

    private void ListenersInit()
    {
        if (inviteReceivedListener == null) inviteReceivedListener = new GameInvitationReceivedListener();
        if (joinRequestedListener == null) joinRequestedListener = new GameJoinRequestedListener();
    }

    private void ListenersDispose()
    {
        if (inviteReceivedListener != null) inviteReceivedListener.Dispose();
        if (joinRequestedListener != null) joinRequestedListener.Dispose();
    }

    #endregion

    #region Methods

    // Gets GalaxyID of lobby that user was invited to from connection string
    GalaxyID ParseLobbyIDFromConnectionString(string connectionString)
    {
        Debug.Assert(connectionString != null);
        Debug.Log("Parsing lobbyID from connection string " + connectionString);
        string lobbyIDString = connectionString.Remove(0, 12);
        UInt64 lobbyIDUint64 = Convert.ToUInt64(lobbyIDString);
        GalaxyID lobbyID = new GalaxyID(lobbyIDUint64);
        Debug.Log("LobbyID parsed from connection string to " + lobbyID);
        return lobbyID;
    }

    // Checks application command line argument for an "--JoinLobby=..." argument, that tells us that user accepted an invite from GOG Galaxy Client
    void CheckForInviteFromClient()
    {
        Debug.Log("Checking for game invites accepted from client");
        // Get Command Line Arguments
        string[] args = Environment.GetCommandLineArgs();
        string connectionString = null;
        // First command line argument will always be the executable name
        if (args.Length > 1)
        {
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith("--JoinLobby=")) 
                {
                    connectionString = args[i];
                    Debug.Log("Invite accepted from client detected. Connection string " + connectionString);
                    SetUpInviteFromClient(connectionString);
                    return;
                }
            }
        }
        Debug.Log("No invite accepted from client detected");
    }

    // Sets up the game for joining lobby specified in invitation that was received and accepted in GOG Galaxy Client
    void SetUpInviteFromClient(string connectionString)
    {
        Debug.Log("Setting up the game for joining lobby from game invite accepted from client");
        pendingLobbyID = ParseLobbyIDFromConnectionString(connectionString);
        if (pendingLobbyID.IsValid())
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                Debug.Log("Main menu scene is already loaded, joining lobby");
                JoinLobby();
            }
            else
            {
                Debug.Log("Main menu scene not loaded, subscribing to SceneManager.sceneLoaded event");
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }
    }

    // Sets up the game for joining lobby specified in invitation that was received and accepted while the game was running
    void SetUpInviteInGame(string connectionString)
    {
        Debug.Log("Setting up the game for joining lobby from game invite accepted from the game");
        pendingLobbyID = new GalaxyID(ParseLobbyIDFromConnectionString(connectionString));
        if (pendingLobbyID.IsValid())
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                Debug.Log("Main menu scene is already loaded, joining lobby");
                JoinLobby();
            }
            else
            {
                Debug.Log("Main menu scene not loaded, subscribing to SceneManager.sceneLoaded event and loading MainMenu scene");
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
            }
        }
    }

    /* If invite was accepted when scene other than main menu was active, this listener is attached to sceneLoaded event
    Note: we unsubscribe from sceneLoaded event in the listener as we only want this to happen once */
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            JoinLobby();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // Joins lobby after accepting a game invitation
    void JoinLobby()
    {
        Debug.Log("Joining LobbyID " + pendingLobbyID);
        GalaxyManager.Instance.StartMatchmaking();
        GalaxyManager.Instance.Matchmaking.StartLobbyBrowsing();
        GalaxyManager.Instance.Matchmaking.JoinLobby(pendingLobbyID);
        GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineJoining);
        pendingLobbyID = null;
    }

    #endregion

    #region Listeners

    /* Informs about the even of accepting an invitation from another user */
    private class GameJoinRequestedListener : GlobalGameJoinRequestedListener
    {
        public override void OnGameJoinRequested(GalaxyID userID, string connectionString)
        {
            Debug.Log("OnGameJoinRequested userID: \"" + userID + "\" connectionString \"" + connectionString + "\"");
            GalaxyManager.Instance.Invitations.SetUpInviteInGame(connectionString);
        }
    }

    /* Informs about the even of receiving an invitation from another user */
    private class GameInvitationReceivedListener : GlobalGameInvitationReceivedListener
    {
        public override void OnGameInvitationReceived(GalaxyID userID, string connectionString)
        {
            Debug.Log("OnGameInvitationReceived userID: \"" + userID + "\" connectionString \"" + connectionString + "\"");
        }
    }

    #endregion

}
