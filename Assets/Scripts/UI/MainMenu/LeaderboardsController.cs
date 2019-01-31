using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardsController : MonoBehaviour
{

    #region Variables

    public Dropdown displayedLeaderboard;
    public Toggle globalToggle;
    public Text message;
    public GameObject waitingCircularArrow;
    public GameObject entriesContainer;
    public GameObject entryPrefab;
    private List<GameObject> entriesList = new List<GameObject> { };

    #endregion

    #region Behaviors

    void OnEnable()
    {
        RequestLeaderboard(false);
    }

    void OnDisable()
    {
        DisposeLeaderboard();
        StopAllCoroutines();
    }

    #endregion

    #region Methods

    public void RequestLeaderboard(bool refresh)
    {
        if (GameObject.Find("GalaxyManager") == null || !GalaxyManager.Instance.GalaxyFullyInitialized)
        {
            DisplayMessage("Galaxy not initialized");
        }
        else
        {
            message.gameObject.SetActive(false);
            waitingCircularArrow.SetActive(true);
            if (refresh)
            {
                DisposeLeaderboard();
            }
            string leaderboardKey;
            GalaxyManager.Instance.leaderboardNames.TryGetValue(displayedLeaderboard.captionText.text, out leaderboardKey);
            if (globalToggle.isOn)
            {
                GalaxyManager.Instance.Leaderboards.RequestLeaderboardEntriesGlobal(leaderboardKey, 0, 100);
            }
            else
            {
                GalaxyManager.Instance.Leaderboards.RequestLeaderboardEntriesAroundUser(leaderboardKey, 50, 49, GalaxyManager.Instance.MyGalaxyID);
            }
        }
    }

    public void DisplayLeaderboard()
    {
        GameObject currentObject;
        waitingCircularArrow.SetActive(false);
        foreach (object[] entry in GalaxyManager.Instance.Leaderboards.LeaderboardEntries)
        {
            currentObject = Instantiate(entryPrefab, entriesContainer.transform);
            currentObject.transform.GetChild(0).GetComponent<Text>().text = entry[2].ToString();
            currentObject.transform.GetChild(1).GetComponent<Text>().text = entry[1].ToString();
            entriesList.Add(currentObject);
        }
    }

    void DisposeLeaderboard()
    {
        waitingCircularArrow.SetActive(true);
        foreach (GameObject child in entriesList)
        {
            Destroy(child);
        }
        entriesList.Clear();
        entriesList.TrimExcess();
    }

    public void DisplayMessage(string text)
    {
        message.text = text;
        message.gameObject.SetActive(true);
        waitingCircularArrow.SetActive(false);
    }

    #endregion

}