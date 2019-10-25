using UnityEngine;
using Galaxy.Api;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Friends : MonoBehaviour
{
    #region Variables

    private Dictionary<string, string> sceneToRichPresenceDict = new Dictionary<string, string>() 
    {
        {"StartingScreen", ""},
        {"MainMenu", "In main menu"},
        {"Local1PlayerGame", "In local 1 player match"},
        {"Local2PlayerGame", "In local 2 player match"},
        {"Online2PlayerGame", "In online 2 player match"}
    };
    // Friends listeners
    private FriendListListener friendListListener;
    private RichPresenceChangeListener richPresenceChangeListener;

    #endregion

    #region Behaviors

    void OnEnable()
    {
        ListenersInit();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        ListenersDispose();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Passive methods

    private void ListenersInit()
    {
        friendListListener = new FriendListListener();
        richPresenceChangeListener = new RichPresenceChangeListener();
    }

    private void ListenersDispose()
    {
        if (friendListListener != null) friendListListener.Dispose();
        if (richPresenceChangeListener != null) richPresenceChangeListener.Dispose();
    }

    // Sets the user rich presence based on the currently loaded scene name
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string value;
        sceneToRichPresenceDict.TryGetValue(scene.name, out value);
        SetRichPresence("status", value);
    }

    #endregion

    #region Active methods

    // Gets the count of current user friends list
    public uint GetFriendCount()
    {
        Debug.Log("Trying to get friend list count");
        uint friendListCount = 0;
        Debug.Assert(friendListListener.retrieved);
        try
        {
            friendListCount = GalaxyInstance.Friends().GetFriendCount();
            Debug.Log("Friend list count " + friendListCount);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogError("Failed to retrieve friend list count " + e);
        }

        return friendListCount;
    }

    // Gets a GalaxyID of friend user using his/her index on current user friends list
    public GalaxyID GetFriendByIndex(uint index)
    {
        Debug.Log("Trying to get friend " + index + " Galaxy ID");
        GalaxyID galaxyID = null;
        Debug.Assert(friendListListener.retrieved);
        try
        {
            galaxyID = GalaxyInstance.Friends().GetFriendByIndex(index);
            Debug.Log("Friend " + index + " Galaxy ID: " + galaxyID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogError("Failed to get friend by index " + index + " for reason " + e);
        }
        return galaxyID;
    }

    // 	Gets username of a user currently signed in to GOG Galaxy Client
    public string GetMyUsername(bool silent = false)
    {
        if (!silent) Debug.Log("Looking up current user name...");
        string username = "";
        try
        {
            username = GalaxyInstance.Friends().GetPersonaName();
            if (!silent) Debug.Log("Current user name: " + username);
        }
        catch (GalaxyInstance.Error e)
        {
            if (!silent) Debug.LogError("Failed to get current user name for reason " + e);
        }
        return username;
    }

    // Gets the username of a user specified by his/her GalaxyID
    public string GetFriendPersonaName(GalaxyID galaxyID)
    {
        Debug.Log("Trying to get friend " + galaxyID + " name");
        string name = "";
        Debug.Assert(friendListListener.retrieved);
        try
        {
            name = GalaxyInstance.Friends().GetFriendPersonaName(galaxyID);
            Debug.Log("Friend " + galaxyID + " name: " + name);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogError("Failed to get friend " + galaxyID + " for reason " + e);
        }
        return name;
    }

    // Gets the state of a user specified by his/her GalaxyID
    public PersonaState GetFriendPersonaState(GalaxyID galaxyID)
    {
        Debug.Log("Trying to get friend " + galaxyID + " state");
        PersonaState state = PersonaState.PERSONA_STATE_OFFLINE;
        Debug.Assert(friendListListener.retrieved);
        try
        {
            state = GalaxyInstance.Friends().GetFriendPersonaState(galaxyID);
            Debug.Log("Friend " + galaxyID + " state: " + state);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogError("Failed to get friend " + galaxyID + " for reason " + e);
        }
        return state;
    }

    /* Sets users rich presence visible to other user in friend list
    Note: Rich presence can use following keys:
    "status" displayed to other users in friend list,
    "metadata" that describes the status to other instances of the game,
    "connect" is basically a connection string that can be used by other players to join the game*/
    public void SetRichPresence(string key, string value)
    {
        Debug.Log("Trying to set rich presence key " + key + " value " + value);
        try
        {
            GalaxyInstance.Friends().SetRichPresence(key, value);
            Debug.Log("Rich presence " + key + " set to value " + value);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.Log("Failed to set rich presence key " + key + " value " + value + " for reason " + e);
        }
    }

    #endregion

    #region Listeners

    /* Informs about the event of receiving friends list definition 
    Historically you were required to call RequestFriendList() 
    now this is done automatically on successfull sign in
    but the callback is still fired when the list is retrieved */

    private class FriendListListener : GlobalFriendListListener
    {
        public bool retrieved = false;

        public override void OnFriendListRetrieveSuccess()
        {
            Debug.Log("Friend list retrieved");
            retrieved = true;
        }

        public override void OnFriendListRetrieveFailure(FailureReason failureReason)
        {
            Debug.Log("Friend list couldn't be retrieved, for reason " + failureReason);
        }

    }

    /* Informs about the event of setting users rich presence
    Callback for methods:
    SetRichPresence(string key, string value) */
    private class RichPresenceChangeListener : GlobalRichPresenceChangeListener
    {
        public override void OnRichPresenceChangeSuccess()
        {
            Debug.Log("Rich presence updated successfully");
        }

        public override void OnRichPresenceChangeFailure(FailureReason failureReason)
        {
            Debug.Log("Rich presence update failure for reason " + failureReason);
        }
    }

    #endregion

}
