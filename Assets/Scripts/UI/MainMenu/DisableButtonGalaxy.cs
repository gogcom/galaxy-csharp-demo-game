using UnityEngine;
using UnityEngine.UI;

public class DisableButtonGalaxy : MonoBehaviour
{

    void OnEnable()
    {

        // this if is seperated into two because if GalaxyManager would be null checking SignedIn status would cause an issue
        if (GameObject.Find("GalaxyManager") == null)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
        else if (!GameObject.Find("GalaxyManager").GetComponent<GalaxyManager>().IsSignedIn())
        {
            gameObject.GetComponent<Button>().interactable = false;
        }

    }

}
