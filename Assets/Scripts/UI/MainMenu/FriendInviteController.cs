using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Galaxy.Api;

public class FriendInviteController : MonoBehaviour
{

    public GameObject entryPrefab;
    public GameObject entries;
    private List<GameObject> list = new List<GameObject> { };

    void OnEnable()
    {
        DisplayFriendList();
    }

    void OnDisable()
    {
        DisposeFriendList();
    }

    void DisplayFriendList()
    {

        uint friendsCount = GalaxyManager.Instance.Friends.GetFriendCount();
        
        for (uint i = 0; i < friendsCount; i++)
        {
            GameObject currentObject = null;
            Button sendInviteButton = null;
            GalaxyID galaxyID = GalaxyManager.Instance.Friends.GetFriendByIndex(i);
            if (GalaxyManager.Instance.Friends.GetFriendPersonaState(galaxyID) == Galaxy.Api.PersonaState.PERSONA_STATE_ONLINE)
            {
                currentObject = Instantiate(entryPrefab, entries.transform);
                currentObject.transform.GetChild(0).GetComponent<Text>().text = GalaxyManager.Instance.Friends.GetFriendPersonaName(galaxyID);
                currentObject.transform.GetChild(1).GetComponent<Text>().text = "Online";
                sendInviteButton = currentObject.transform.GetChild(2).GetComponent<Button>();
                sendInviteButton.onClick.AddListener(() =>
                {
                    SendInvitation(galaxyID);
                });
                list.Add(currentObject);
            }
        }

    }

    void DisposeFriendList()
    {
        foreach (GameObject child in list)
        {
            Destroy(child);
        }
        list.Clear();
        list.TrimExcess();
    }

    private void SendInvitation(GalaxyID galaxyID)
    {
        GalaxyManager.Instance.Matchmaking.SendInvitation(galaxyID);
    }

}