﻿using System.Collections;
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
    private IEnumerator displayPlayerListCoroutine;

    void OnEnable()
    {
        // Dispose unnecessary listeners
        GalaxyManager.Instance.Matchmaking.LobbyBrowsingListenersDispose();
        GalaxyManager.Instance.Matchmaking.LobbyCreationListenersDispose();
        // Initialize required listeners
        GalaxyManager.Instance.Matchmaking.LobbyManagmentMainMenuListenersInit();

        lobbyID = GalaxyManager.Instance.Matchmaking.CurrentLobbyID;
        
        GalaxyManager.Instance.Friends.SetRichPresence("status", "In online lobby");
        GalaxyManager.Instance.Friends.SetRichPresence("connect", "--JoinLobby=" + lobbyID);

        displayPlayerListCoroutine = DisplayPlayerListCoroutine();
        StartCoroutine(displayPlayerListCoroutine);
    }

    void OnDisable()
    {
        GalaxyManager.Instance.Friends.SetRichPresence("connect", null);
        startGameButton.GetComponent<Button>().interactable = false;
        GalaxyManager.Instance.Matchmaking.LobbyManagmentMainMenuListenersDispose();
        StopCoroutine(displayPlayerListCoroutine);
        DisposeEntries();
    }

    IEnumerator DisplayPlayerListCoroutine()
    {
        uint lobbyMembersCount;
        GameObject currentEntry;
        GalaxyID currentMember;

        for(;;)
        {
            lobbyMembersCount = GalaxyManager.Instance.Matchmaking.GetNumLobbyMembers(lobbyID);
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
            yield return new WaitForSecondsRealtime(0.5f);
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
        GalaxyManager.Instance.Matchmaking.LeaveLobby();
        GalaxyManager.Instance.Friends.SetRichPresence("status", "In main menu");
    }

    public void ShowOverlayInviteDialog()
    {
        GalaxyManager.Instance.Matchmaking.ShowOverlayInviteDialog();
    }

}