using System.Collections.Generic;
using UnityEngine;
using Galaxy.Api;

public class LobbyChat : MonoBehaviour {

    public LobbyMessageListener lobbyMessageListener;
    public List<Dictionary<string, string>> lobbyMessageHistory = new List<Dictionary<string, string>>();

    void OnEnable()
    {
        ListenersInit();
    }

    void OnDisable()
    {
        ListenersDispose();
    }

	// Instantiantes listeners
    private void ListenersInit()
    {
        if (lobbyMessageListener == null) lobbyMessageListener = new LobbyMessageListener();
    }

    // Disposes listeners
    public void ListenersDispose()
    {
        if (lobbyMessageListener != null) lobbyMessageListener.Dispose();
    }

	// Lobby message listener
    public class LobbyMessageListener : GlobalLobbyMessageListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        string message = null;

        public override void OnLobbyMessageReceived(GalaxyID lobbyID, GalaxyID senderID, uint messageID, uint messageLength)
        {
            Dictionary<string, string> messageAndSenderDict = new Dictionary<string, string>();
            try
            {
                Debug.Log("Lobby " + lobbyID + " Sender " + senderID + " message " + message);
                message = matchmaking.GetLobbyMessage(GalaxyManager.Instance.Matchmaking.CurrentLobbyID, ref senderID, messageID);
                messageAndSenderDict.Add("sender", GalaxyManager.Instance.Friends.GetFriendPersonaName(senderID));
                messageAndSenderDict.Add("message", message);
                matchmaking.LobbyChat.lobbyMessageHistory.Add(messageAndSenderDict);
                Debug.Log("New message from " + GalaxyManager.Instance.Friends.GetFriendPersonaName(senderID) + " to lobbyID " + lobbyID + ": " + message);
                if (GameManager.Instance != null) ((Online2PlayerGameManager)GameManager.Instance).PopChatPrompt();
            }
            catch (GalaxyInstance.Error e)
            {
                Debug.LogWarning(e);
            }
        }
    }

}
