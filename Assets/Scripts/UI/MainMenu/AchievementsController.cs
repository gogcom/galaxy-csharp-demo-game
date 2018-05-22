using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsController : MonoBehaviour
{

    public GameObject entryPrefab;
    public GameObject entries;
    private List<GameObject> entryList = new List<GameObject> { };

    void OnEnable()
    {
        DisplayAchievements();
    }

    void OnDisable()
    {
        DisposeAchievements();
    }

    void DisplayAchievements()
    {

        int i = 0;
        GameObject currentObject;

        foreach (string entry in GalaxyManager.Instance.achievementsList)
        {
            currentObject = Instantiate(entryPrefab, entries.transform);
            currentObject.name = entry;
            currentObject.transform.GetChild(0).GetComponent<Text>().text = "" + GalaxyManager.Instance.StatsAndAchievements.GetAchievementName(entry);
            currentObject.transform.GetChild(1).GetComponent<Text>().text = "" + GalaxyManager.Instance.StatsAndAchievements.GetAchievement(entry);
            entryList.Add(currentObject);
            i++;

        }

    }

    void DisposeAchievements()
    {

        foreach (GameObject child in entryList)
        {

            Destroy(child);

        }

        entryList.Clear();
        entryList.TrimExcess();

    }

}