﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Galaxy.Api;

public class ChatGameController : MonoBehaviour {

	public GameObject messageEntriesContainer;
    public GameObject messageEntryPrefab;
    public InputField messagePrompt;
    public ScrollRect messageDisplay;
    private List<GameObject> messagesEntriesList = new List<GameObject>();
    private List<Dictionary<string, string>> lobbyMessageHistory;
    private GalaxyID lobbyID;

    void OnEnable()
    {
        lobbyID = GalaxyManager.Instance.Matchmaking.CurrentLobbyID;
    }

    void Update()
    {
        lobbyMessageHistory = GalaxyManager.Instance.Matchmaking.lobbyMessageHistory;
        if (lobbyMessageHistory.Count > messagesEntriesList.Count)
        {
            DisplayChatMessages();
        }
        messagePrompt.ActivateInputField();
    }

    void OnDisable()
    {
        DisposeEntries();
    }

    public void DisplayChatMessages()
    {
        int messagesToAdd = lobbyMessageHistory.Count - messagesEntriesList.Count;
        int currentMessageIndex = lobbyMessageHistory.Count - messagesToAdd;
        GameObject currentMessageEntry;
        for (; messagesToAdd > 0; messagesToAdd--)
        {
            currentMessageIndex = lobbyMessageHistory.Count - messagesToAdd;
            currentMessageEntry = Instantiate(messageEntryPrefab, messageEntriesContainer.transform);
            currentMessageEntry.transform.GetChild(0).GetComponent<Text>().text = lobbyMessageHistory[currentMessageIndex]["sender"];;
            if (lobbyMessageHistory[currentMessageIndex]["sender"] == GalaxyManager.Instance.Friends.GetMyUsername()) currentMessageEntry.transform.GetChild(0).GetComponent<Text>().color = Color.green;
            else currentMessageEntry.transform.GetChild(0).GetComponent<Text>().color = Color.blue;
            currentMessageEntry.transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Bold;
            currentMessageEntry.transform.GetChild(1).GetComponent<Text>().text = lobbyMessageHistory[currentMessageIndex]["message"];
            messagesEntriesList.Add(currentMessageEntry);
        }
        Canvas.ForceUpdateCanvases();
        messageDisplay.verticalNormalizedPosition = 0;
    }

    void DisposeEntries()
    {
        foreach (GameObject messageEntry in messagesEntriesList)
        {
            Destroy(messageEntry);
        }
        messagesEntriesList.Clear();
        messagesEntriesList.TrimExcess();
    }

    public void SendLobbyMessage()
    {
        string message = messagePrompt.text;
        if (message.Length != 0)
        {
            GalaxyManager.Instance.Matchmaking.SendLobbyMessage(lobbyID, message);
            messagePrompt.text = null;
        }
    }

}
