using UnityEngine;

// This script takes care of all the stuff that happens when you put a ball

public class HoleTrigger : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {

        GameManager.Instance.shotBallsPut.Add(other.GetComponent<BallsCollisions>().myColor);
        other.gameObject.SetActive(false);
        other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        other.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    }

}