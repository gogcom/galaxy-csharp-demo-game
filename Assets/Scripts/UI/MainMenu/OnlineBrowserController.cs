using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Galaxy.Api;

public class OnlineBrowserController : MonoBehaviour
{
    public GameObject waitingCircularArrow;
    public GameObject entriesContainer;
    public GameObject entryPrefab;
    private List<GameObject> entriesList = new List<GameObject>();

    void OnEnable()
    {
        GalaxyManager.Instance.Matchmaking.StartLobbyBrowsing();
        RequestLobbyList(false);
    }

    void OnDisable()
    {
        DisposeLobbiesList();
    }

    public void RequestLobbyList(bool refresh)
    {
        waitingCircularArrow.SetActive(true);
        if (refresh)
        {
            DisposeLobbiesList();
        }
        GalaxyManager.Instance.Matchmaking.RequestLobbyList();
    }

    // This is called in OnLobbyDataUpdated listener
    public void DisplayLobbyList(List<GalaxyID> lobbyList)
    {
        Debug.Log("Displaying lobby list");
        waitingCircularArrow.SetActive(false);
        GameObject currentEntry;
        foreach (GalaxyID lobbyID in lobbyList)
        {
            Debug.Log("Current lobby ID " + lobbyID);
            currentEntry = Instantiate(entryPrefab, entriesContainer.transform);
            currentEntry.transform.GetChild(0).GetComponent<Text>().text = GalaxyManager.Instance.Matchmaking.GetLobbyData(lobbyID, "name");
            currentEntry.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {
                JoinLobby(lobbyID);
            });
            entriesList.Add(currentEntry);
        }
    }

    private void DisposeLobbiesList()
    {
        Debug.Log("Disposing lobbies list");
        waitingCircularArrow.SetActive(true);
        foreach (GameObject child in entriesList)
        {
            Destroy(child);
        }
        entriesList.Clear();
        entriesList.TrimExcess();
    }

    public void JoinLobby(GalaxyID lobbyID)
    {
        Debug.Log("Joining lobby " + lobbyID);
        GalaxyManager.Instance.Matchmaking.JoinLobby(lobbyID);
        GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineJoining);
    }

}