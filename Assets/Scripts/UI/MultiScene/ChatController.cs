using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{

    public GameObject messageEntriesContainer;
    public GameObject messageEntryPrefab;
    public InputField messagePrompt;
    public ScrollRect messageDisplay;
    private List<GameObject> messagesEntriesList = new List<GameObject>();

    public void DisplayChatMessage(Dictionary<string, string> currentMessageDict)
    {
        GameObject currentMessageEntry = Instantiate(messageEntryPrefab, messageEntriesContainer.transform);
        currentMessageEntry.transform.GetChild(0).GetComponent<Text>().text = currentMessageDict["sender"];
        if (currentMessageDict["sender"] == GalaxyManager.Instance.Friends.GetMyUsername()) {
            currentMessageEntry.transform.GetChild(0).GetComponent<Text>().color = Color.green;
        } else {
            currentMessageEntry.transform.GetChild(0).GetComponent<Text>().color = Color.blue;
        }
        currentMessageEntry.transform.GetChild(1).GetComponent<Text>().text = currentMessageDict["message"];
        messagesEntriesList.Add(currentMessageEntry);
        Canvas.ForceUpdateCanvases();
        messageDisplay.verticalNormalizedPosition = 0;
    }

    public void DisposeEntries()
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
            GalaxyManager.Instance.Matchmaking.SendLobbyMessage(GalaxyManager.Instance.Matchmaking.CurrentLobbyID, message);
            messagePrompt.text = null;
        }
    }

}
