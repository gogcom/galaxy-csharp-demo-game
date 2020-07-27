using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsController : MonoBehaviour
{

    public GameObject entryPrefab;
    public GameObject entries;
    private List<GameObject> list = new List<GameObject> { };

    void OnEnable()
    {
        DisplayStats();
    }

    void OnDisable()
    {
        DisposeStats();
    }

    void DisplayStats()
    {

        if (GameObject.Find("GalaxyManager") == null || !GameObject.Find("GalaxyManager").GetComponent<GalaxyManager>().GalaxyFullyInitialized)
        {
            Debug.Log("Galaxy not initialized");
            return;
        }
        int i = 0;
        GameObject currentObject;
        foreach (KeyValuePair<string, string> entry in GalaxyManager.Instance.statisticsIntList)
        {
            currentObject = Instantiate(entryPrefab, entries.transform);
            currentObject.name = entry.Key;
            currentObject.transform.GetChild(0).GetComponent<Text>().text = entry.Value;
            currentObject.transform.GetChild(1).GetComponent<Text>().text = "" + GameObject.Find("GalaxyManager").GetComponent<GalaxyManager>().StatsAndAchievements.GetStatInt(entry.Key);

            list.Add(currentObject);
            i++;
        }
        foreach (KeyValuePair<string, string> entry in GalaxyManager.Instance.statisticsFloatList)
        {

            currentObject = Instantiate(entryPrefab, entries.transform);
            currentObject.name = entry.Key;
            currentObject.transform.GetChild(0).GetComponent<Text>().text = entry.Value;
            currentObject.transform.GetChild(1).GetComponent<Text>().text = "" + GameObject.Find("GalaxyManager").GetComponent<GalaxyManager>().StatsAndAchievements.GetStatFloat(entry.Key);

            list.Add(currentObject);
            i++;

        }
    }

    void DisposeStats()
    {
        foreach (GameObject child in list)
        {
            Destroy(child);
        }
        list.Clear();
        list.TrimExcess();
    }

}