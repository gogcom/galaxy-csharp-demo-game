using UnityEngine;

public class SpotTrigger : MonoBehaviour
{

    public bool available = true;
    public GameManager.BallColorEnum myColor;

    void OnTriggerEnter()
    {
        available = false;
    }

    void OnTriggerExit()
    {
        available = true;
    }

}
