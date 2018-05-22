using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Galaxy.Api;


public class OnlineWaitController : MonoBehaviour
{

    public GameObject entriesContainer;
    public GameObject entryPrefab;
    public GameObject startGameButton;
    private List<GameObject> entriesList = new List<GameObject>();
    private GalaxyID lobbyID;

    void OnEnable()
    {
        GalaxyManager.Instance.Matchmaking.ShutdownLobbyCreation();
        GalaxyManager.Instance.Matchmaking.ShutdownLobbyBrowsing();

        GalaxyManager.Instance.Matchmaking.StartLobbyManagementMainMenu();
        GalaxyManager.Instance.Matchmaking.StartLobbyChat();

        lobbyID = GalaxyManager.Instance.Matchmaking.CurrentLobbyID;
        GalaxyManager.Instance.Friends.SetRichPresence("status", "In online lobby");
        GalaxyManager.Instance.Friends.SetRichPresence("connect", "--JoinLobby=" + lobbyID);
    }

    void Update()
    {
        DisplayPlayerList();
    }

    void OnDisable()
    {
        GalaxyManager.Instance.Friends.SetRichPresence("connect", null);
        startGameButton.GetComponent<Button>().interactable = false;
        DisposeEntries();
    }

    void DisplayPlayerList()
    {
        uint lobbyMembersCount = GalaxyManager.Instance.Matchmaking.GetNumLobbyMembers(lobbyID);
        GameObject currentEntry;
        GalaxyID currentMember;

        DisposeEntries();

        for (uint i = 0; i < lobbyMembersCount; i++)
        {
            currentMember = GalaxyManager.Instance.Matchmaking.GetLobbyMemberByIndex(lobbyID, i);
            currentEntry = Instantiate(entryPrefab, entriesContainer.transform);
            currentEntry.name = currentMember.ToString();
            currentEntry.transform.GetChild(0).GetComponent<Text>().text = GalaxyManager.Instance.Friends.GetFriendPersonaName(currentMember);
            currentEntry.transform.GetChild(1).GetComponent<Text>().text = GalaxyManager.Instance.Matchmaking.GetPingWith(currentMember).ToString();
            currentEntry.transform.GetChild(2).GetComponent<Text>().text = GalaxyManager.Instance.Matchmaking.GetLobbyMemberData(currentMember, "state");
            entriesList.Add(currentEntry);
        }

    }

    void DisposeEntries()
    {
        foreach (GameObject entry in entriesList)
        {
            Destroy(entry);
        }
        entriesList.Clear();
        entriesList.TrimExcess();

    }

    public void Ready()
    {
        GalaxyManager.Instance.Matchmaking.SetLobbyMemberData("state", "ready");
    }

    public void StartGame()
    {
        GalaxyManager.Instance.Matchmaking.SetLobbyData(lobbyID, "state", "steady");
        GalaxyManager.Instance.Friends.SetRichPresence("status", "In online 2 player match");
    }

    public void LeaveLobby()
    {
        if (GalaxyManager.Instance.Matchmaking.CurrentLobbyID != null) GalaxyManager.Instance.Matchmaking.LeaveLobby();
        GalaxyManager.Instance.Friends.SetRichPresence("status", "In main menu");
    }

    public void ShowOverlayInviteDialog()
    {
        GalaxyManager.Instance.Matchmaking.ShowOverlayInviteDialog();
    }

}