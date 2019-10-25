using System.Collections.Generic;
using UnityEngine;
using Galaxy.Api;

public class Leaderboards : MonoBehaviour
{
    #region Variables

    // Variables for storing leaderboards data
    private static List<object[]> leaderboardEntries = new List<object[]>();
    public List<object[]> LeaderboardEntries { get { return leaderboardEntries; } }

    // Leaderboards listeners
    private LeaderboardsRetrieveListener leadRetrieveListener;
    private LeaderboardEntriesRetrieveListener leadEntriesRetrieveListener;
    private LeaderboardScoreUpdateListener leadScoreUpdateListener;

    #endregion

    #region Behaviors

    void OnEnable()
    {
        ListenersInit();
        RequestLeaderboards();
    }

    void OnDisable()
    {
        ListenersDispose();
    }

    #endregion

    #region Listeners methods

    // Instantiates listeners 
    private void ListenersInit()
    {
        leadRetrieveListener = new LeaderboardsRetrieveListener();
        leadEntriesRetrieveListener = new LeaderboardEntriesRetrieveListener();
        leadScoreUpdateListener = new LeaderboardScoreUpdateListener();
    }

    // Disposes listeners
    private void ListenersDispose()
    {
        if (leadRetrieveListener != null) leadRetrieveListener.Dispose();
        if (leadEntriesRetrieveListener != null) leadEntriesRetrieveListener.Dispose();
        if (leadScoreUpdateListener != null) leadScoreUpdateListener.Dispose();
    }

    #endregion

    #region Methods

    /* Request leadeboard definitions for later use
    Note: This request has to finish successfully before using any leadeboards related methods */
    public void RequestLeaderboards()
    {
        Debug.Assert(!leadRetrieveListener.retrieved && GalaxyManager.Instance.IsLoggedOn());
        Debug.Log("Requesting Leaderboards");
        try
        {
            GalaxyInstance.Stats().RequestLeaderboards();
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Leaderboard definitions couldn't be retrieved for reason: " + e);
        }
    }


    // Retrieves global leaderboard entries
    public void RequestLeaderboardEntriesGlobal(string leaderboardName, uint rangeStart, uint rangeEnd)
    {
        Debug.Log("Trying to get leaderboard \"" + leaderboardName + "\" entries global");
        Debug.Assert(leadRetrieveListener.retrieved);
        try
        {
            GalaxyInstance.Stats().RequestLeaderboardEntriesGlobal(leaderboardName, rangeStart, rangeEnd);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve leaderboard \"" + leaderboardName + "\" global entries for reason: " + e);
        }
    }

    // Retrieves leaderboard entries around user
    public void RequestLeaderboardEntriesAroundUser(string leaderboardName, uint countBefore, uint countAfter, GalaxyID userID)
    {
        Debug.Log("Trying to get leaderboard \"" + leaderboardName + "\" entries around user");
        Debug.Assert(leadRetrieveListener.retrieved);
        try
        {
            GalaxyInstance.Stats().RequestLeaderboardEntriesAroundUser(leaderboardName, countBefore, countAfter, userID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve leaderboard \"" + leaderboardName + "\" entries around user for reason: " + e);
        }
    }

    // Sets users leaderboard score
    public void SetLeaderboardScore(string leaderboardName, int score, bool forceUpdate = false)
    {
        Debug.Log("Trying to set leaderboard \"" + leaderboardName + "\" score");
        Debug.Assert(leadRetrieveListener.retrieved);
        try
        {
            GalaxyInstance.Stats().SetLeaderboardScore(leaderboardName, score, forceUpdate);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not set leaderboard \"" + leaderboardName + "\" entry for reason: " + e);
        }
    }

    #endregion

    #region Listeners

    /* Informs about the event of receiving leaderboards definitions
    Callbacks for method: 
    RequestLeaderboards() */
    private class LeaderboardsRetrieveListener : GlobalLeaderboardsRetrieveListener
    {
        public bool retrieved = false;

        public override void OnLeaderboardsRetrieveSuccess()
        {
            Debug.Log("Leaderboard definitions retrieved");
            retrieved = true;
        }
        public override void OnLeaderboardsRetrieveFailure(FailureReason failureReason)
        {
            Debug.LogWarning("Leaderboard definitions couldn't be retrieved, for reason: " + failureReason);
        }

    }

    /* Informs about the event of retrieving leaderboards entries
    Callbacks for method: 
    RequestLeaderboardEntriesGlobal(string leaderboardName, uint rangeStart, uint rangeEnd),
    RequestLeaderboardEntriesAroundUser(string leaderboardName, uint countBefore, uint countAfter, GalaxyID userID) */
    private class LeaderboardEntriesRetrieveListener : GlobalLeaderboardEntriesRetrieveListener
    {
        public override void OnLeaderboardEntriesRetrieveSuccess(string leaderboardName, uint entryCount)
        {
            Debug.Log("Leaderboard \"" + leaderboardName + "\" entries retrieved\nEntry count: " + entryCount);

            leaderboardEntries.Clear();
            leaderboardEntries.TrimExcess();

            for (uint i = 0; i < entryCount; i++)
            {
                GalaxyID userID = new GalaxyID();
                uint rank = 0;
                int score = 0;
                string username = null;
                object[] entryDetails = new object[] { rank, score, username };
                
                GalaxyInstance.Stats().GetRequestedLeaderboardEntry(i, ref rank, ref score, ref userID);
                username = GalaxyManager.Instance.Friends.GetFriendPersonaName(userID);
                entryDetails[0] = rank;
                entryDetails[1] = score; 
                entryDetails[2] = username;
                Debug.Log("Created object #" + i + " | " + rank + " | " + score + " | " + username);
                leaderboardEntries.Add(entryDetails);
            }

            if (GameObject.Find("LeaderboardsScreen")) GameObject.Find("LeaderboardsScreen").GetComponent<LeaderboardsController>().DisplayLeaderboard();

        }

        public override void OnLeaderboardEntriesRetrieveFailure(string leaderboardName, FailureReason failureReason)
        {
            Debug.LogWarning("Could not retrieve leaderboard \"" + leaderboardName + "\" entries for reason: " + failureReason);
            if (GameObject.Find("LeaderboardsScreen"))
            {
                if (failureReason == FailureReason.FAILURE_REASON_NOT_FOUND) GameObject.Find("LeaderboardsScreen").GetComponent<LeaderboardsController>().DisplayMessage("No entry for the current user in selected leadeboard");
                if (failureReason == FailureReason.FAILURE_REASON_UNDEFINED) GameObject.Find("LeaderboardsScreen").GetComponent<LeaderboardsController>().DisplayMessage("Failure reason undefined");
            }
        }
    }

    /* Informs about the event of updating leaderboards entry
    Callbacks for method: 
    SetLeaderboardScore(string leaderboardName, int score, bool forceUpdate = false) */
    private class LeaderboardScoreUpdateListener : GlobalLeaderboardScoreUpdateListener
    {
        public override void OnLeaderboardScoreUpdateSuccess(string leaderboardName, int score, uint oldRank, uint newRank)
        {
            Debug.Log("Set leaderboard \"" + leaderboardName + "\" score to " + score);
        }
        public override void OnLeaderboardScoreUpdateFailure(string leaderboardName, int score, FailureReason failureReason)
        {
            Debug.LogWarning("Could not set leaderboard \"" + leaderboardName + "\" entry for reason: " + failureReason);
        }
    }

    #endregion

}