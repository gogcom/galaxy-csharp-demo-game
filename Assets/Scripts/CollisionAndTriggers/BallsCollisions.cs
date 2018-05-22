using UnityEngine;

public class BallsCollisions : MonoBehaviour
{
    private Vector3 myPosition;
    public Vector3 MyPosition 
    {
        get 
        { 
            return myPosition;
        }
    }
    public GameManager.BallColorEnum myColor;
    void Awake() 
    {
        myPosition = gameObject.transform.position;
    }
    void OnCollisionEnter(Collision c)
    {
        if (c.collider.name == "WhiteBall")
        {
            GameManager.Instance.shotBallsHit.Add(myColor);
        }
    }

}