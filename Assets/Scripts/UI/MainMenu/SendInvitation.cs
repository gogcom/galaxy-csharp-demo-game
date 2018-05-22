using UnityEngine;

public class SendInvitation : MonoBehaviour
{

    public string connectionString;
    public Galaxy.Api.GalaxyID galaxyID;

    public void ActionOnClick()
    {
        GameObject.Find("GalaxyManager").GetComponent<GalaxyManager>().Matchmaking.SendInvitation(galaxyID);
    }

}
