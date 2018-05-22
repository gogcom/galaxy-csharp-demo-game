using UnityEngine;
using UnityEngine.UI;
using Galaxy.Api;

public class DisplayGameName : MonoBehaviour
{

    // Use this for initialization
    void OnEnable()
    {
        GalaxyID lobbyID = GalaxyManager.Instance.Matchmaking.CurrentLobbyID;
        Debug.Log("Lobby ID " + lobbyID);
        gameObject.GetComponent<Text>().text = GalaxyManager.Instance.Matchmaking.GetLobbyData(lobbyID, "name");
    }

}
