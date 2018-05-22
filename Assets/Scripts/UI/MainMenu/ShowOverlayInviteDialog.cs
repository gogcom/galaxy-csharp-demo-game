using UnityEngine;

public class ShowOverlayInviteDialog : MonoBehaviour
{

    public void ActionOnClick()
    {
        GameObject.Find("GalaxyManager").GetComponent<GalaxyManager>().Matchmaking.ShowOverlayInviteDialog();
    }

}
