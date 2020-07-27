using UnityEngine;
using Galaxy.Api;
using Helpers;

public class StatsAndAchievements : MonoBehaviour
{
    #region Variables

    // Achievements listeners
    private UserStatsAndAchievementsRetrieveListener achievementRetrieveListener;
    private AchievementChangeListener achievementChangeListener;
    private StatsAndAchievementsStoreListener achievementStoreListener;

    #endregion

    #region Behaviors

    void OnEnable()
    {
        ListenersInit();
        RequestUserStatsAndAchievements();
    }

    void OnDisable()
    {
        ListenersDispose();
    }

    #endregion

    #region Listeners methods

    private void ListenersInit()
    {
        Listener.Create(ref achievementRetrieveListener);
        Listener.Create(ref achievementChangeListener);
        Listener.Create(ref achievementStoreListener);
    }

    private void ListenersDispose()
    {
        Listener.Dispose(ref achievementStoreListener);
        Listener.Dispose(ref achievementRetrieveListener);
        Listener.Dispose(ref achievementChangeListener);
    }

    #endregion

    #region Methods

    /* Coroutine for retriving stats and achievements 
    Note: This request has to finish successfully before using any achievements or statistics related methods */
    public void RequestUserStatsAndAchievements()
    {
        Debug.Assert(!achievementRetrieveListener.retrieved && GalaxyManager.Instance.IsSignedIn());
        Debug.Log("Requesting Stats and Achievements");
        try
        {
            GalaxyInstance.Stats().RequestUserStatsAndAchievements();
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Achievements definitions could not be retrived for reason: " + e);
        }
    }

    // 	Unlocks achievement specified by API Key
    public void SetAchievement(string apiKey)
    {
        Debug.Log("Trying to unlock achievement " + apiKey);
        Debug.Assert(achievementRetrieveListener.retrieved);
        try
        {
            GalaxyInstance.Stats().SetAchievement(apiKey);
            GalaxyInstance.Stats().StoreStatsAndAchievements();
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Achievement " + apiKey + " could not be unlocked for reason: " + e);
        }
    }

    // 	Gets status of achievement specified by API key
    public bool GetAchievement(string apiKey)
    {
        Debug.Log("Trying to get achievement status for " + apiKey);
        bool unlocked = false;
        Debug.Assert(achievementRetrieveListener.retrieved);
        try
        {
            uint unlockTime = 0;
            GalaxyInstance.Stats().GetAchievement(apiKey, ref unlocked, ref unlockTime);
            Debug.Log("Achievement: \"" + apiKey + "\" unlocked: " + unlocked + " unlock time: " + unlockTime);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not get status of achievement " + apiKey + " for reason: " + e);
        }
        return unlocked;
    }

    // 	Gets name of achievement specified by API key
    public string GetAchievementName(string apiKey)
    {
        Debug.Log("Trying to get achievement name " + apiKey);
        string name = "";
        Debug.Assert(achievementRetrieveListener.retrieved);
        try
        {
            name = GalaxyInstance.Stats().GetAchievementDisplayName(apiKey);
            Debug.Log("Achievement display name: " + name);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not get name of achievement " + apiKey + " for reason: " + e);
        }
        return name;
    }

    // Sets value for a stat of float type specified by API key 
    public void SetStatFloat(string apiKey, float statValue)
    {
        Debug.Log("Setting stat " + apiKey);
        Debug.Assert(achievementRetrieveListener.retrieved);
        try
        {
            GalaxyInstance.Stats().SetStatFloat(apiKey, statValue);
            GalaxyInstance.Stats().StoreStatsAndAchievements();
            Debug.Log("Stat " + apiKey + " set to " + statValue);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not set value of statistic " + apiKey + " for reason: " + e);
        }
    }

    // Sets value for a stat of int type specified by API key 
    public void SetStatInt(string apiKey, int statValue)
    {
        Debug.Log("Setting stat " + apiKey);
        Debug.Assert(achievementRetrieveListener.retrieved);
        try
        {
            GalaxyInstance.Stats().SetStatInt(apiKey, statValue);
            GalaxyInstance.Stats().StoreStatsAndAchievements();
            Debug.Log("Stat " + apiKey + " set to " + statValue);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not set value of statistic " + apiKey + " for reason: " + e);
        }
    }

    // 	Gets a stat of float type by its API key
    public float GetStatFloat(string apiKey)
    {
        Debug.Log("Getting stat " + apiKey);
        float statValue = 0;
        Debug.Assert(achievementRetrieveListener.retrieved);
        try
        {
            statValue = GalaxyInstance.Stats().GetStatFloat(apiKey);
            Debug.Log("Stat with key " + apiKey + " has value " + statValue);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not get value of statistic " + apiKey + " for reason: " + e);
        }
        return statValue;
    }

    // 	Gets a stat of int type by its API key
    public int GetStatInt(string apiKey)
    {
        Debug.Log("Getting stat " + apiKey);
        int statValue = 0;
        Debug.Assert(achievementRetrieveListener.retrieved);
        try
        {
            statValue = GalaxyInstance.Stats().GetStatInt(apiKey);
            Debug.Log("Stat with key " + apiKey + " has value " + statValue);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not get value of statistic " + apiKey + " for reason: " + e);
        }
        return statValue;
    }

    // Resets stats and achievements
    public void ResetStatsAndAchievements()
    {
        Debug.Log("Trying to reset user stats and achievements");
        Debug.Assert(achievementRetrieveListener.retrieved);
        try
        {
            GalaxyInstance.Stats().ResetStatsAndAchievements();
            Debug.Log("User stats and achievements reset");
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not get reset user stats and achievements for reason: " + e);
        }
    }

    #endregion

    #region Listeners

    /* Informs about the event of receiving user stats and achievements definitions
    Callbacks for method: 
    RequestUserStatsAndAchievements() */
    private class UserStatsAndAchievementsRetrieveListener : GlobalUserStatsAndAchievementsRetrieveListener
    {
        public bool retrieved;

        public override void OnUserStatsAndAchievementsRetrieveSuccess(GalaxyID userID)
        {
            retrieved = true;
            GalaxyManager.Instance.StatsAndAchievements.SetAchievement("launchTheGame");
            Debug.Log("User " + userID + " stats and achievements retrieved");
        }

        public override void OnUserStatsAndAchievementsRetrieveFailure(GalaxyID userID, FailureReason failureReason)
        {
            retrieved = false;
            Debug.LogWarning("User " + userID + " stats and achievements could not be retrieved, for reason " + failureReason);
        }
    }

    /* Informs about the event of storing changes made to the achievement or statiscis of a user
    Callback for method: 
    GalaxyInstance.Stats().StoreStatsAndAchievements(); */
    private class StatsAndAchievementsStoreListener : GlobalStatsAndAchievementsStoreListener
    {

        public override void OnUserStatsAndAchievementsStoreFailure(FailureReason failureReason)
        {
            Debug.LogWarning("OnUserStatsAndAchievementsStoreFailure: " + failureReason);
        }

        public override void OnUserStatsAndAchievementsStoreSuccess()
        {
            Debug.Log("OnUserStatsAndAchievementsStoreSuccess");
        }
    }

    /* Informs about the event of unlocking an achievement
    Callback for methods:
    SetAchievement(string apiKey) */
    private class AchievementChangeListener : GlobalAchievementChangeListener
    {
        public override void OnAchievementUnlocked(string name)
        {
            Debug.Log("Achievement \"" + name + "\" unlocked");
        }
    }

    #endregion

}