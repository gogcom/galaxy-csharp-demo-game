using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsController : MonoBehaviour
{

    public GameObject entries;
    public GameObject entryPrefab;
    private List<GameObject> entryList = new List<GameObject>();
    private Dictionary<Galaxy.Api.PersonaState, string> personaStateEnumToStringMap = new Dictionary<Galaxy.Api.PersonaState, string>()
    {
        {Galaxy.Api.PersonaState.PERSONA_STATE_OFFLINE, "Offline"},
        {Galaxy.Api.PersonaState.PERSONA_STATE_ONLINE, "Online"},
    };

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
            Galaxy.Api.GalaxyID galaxyID = null;
            Galaxy.Api.PersonaState personaStateEnum = Galaxy.Api.PersonaState.PERSONA_STATE_OFFLINE;
            string personaStateName = null;
            GameObject currentObject = null;
            galaxyID = GalaxyManager.Instance.Friends.GetFriendByIndex(i);
            personaStateEnum = GalaxyManager.Instance.Friends.GetFriendPersonaState(galaxyID);
            personaStateEnumToStringMap.TryGetValue(personaStateEnum, out personaStateName);
            currentObject = Instantiate(entryPrefab, entries.transform);
            currentObject.transform.GetChild(0).GetComponent<Text>().text = GalaxyManager.Instance.Friends.GetFriendPersonaName(galaxyID);
            currentObject.transform.GetChild(1).GetComponent<Text>().text = (personaStateName != null) ? personaStateName : "Unknown" ;
            entryList.Add(currentObject);
        }
    }

    void DisposeFriendList()
    {
        foreach (GameObject child in entryList)
        {
            Destroy(child);
        }
        entryList.Clear();
        entryList.TrimExcess();
    }

}