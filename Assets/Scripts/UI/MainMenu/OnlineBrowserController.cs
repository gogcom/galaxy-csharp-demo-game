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
        // Initialize the required listeners
        GalaxyManager.Instance.Matchmaking.LobbyBrowsingListenersInit();
        RequestLobbyList(false);
    }

    void OnDisable()
    {
        // We don't want to dispose the listeners here because we need them when joining the game,
        // and that happens after this menu is closed.
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
    public void DisplayLobbyList(List<object[]> lobbyList)
    {
        Debug.Log("Displaying lobby list");
        waitingCircularArrow.SetActive(false);
        GameObject currentEntry;

        if (lobbyList == null) return;

        foreach (object[] lobby in lobbyList)
        {
            Debug.Log("Current lobby ID " + lobby[2].ToString());
            currentEntry = Instantiate(entryPrefab, entriesContainer.transform);
            currentEntry.transform.GetChild(0).GetComponent<Text>().text = lobby[0].ToString();
            currentEntry.transform.GetChild(1).GetComponent<Text>().text = lobby[1].ToString();
            currentEntry.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            {
                JoinLobby((GalaxyID)lobby[2]);
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