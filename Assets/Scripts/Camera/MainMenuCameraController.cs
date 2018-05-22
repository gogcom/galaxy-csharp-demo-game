using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    private float angleSpeed = 0.1f;
    public GameObject target;

    void LateUpdate()
    {
        transform.RotateAround(target.transform.position, Vector3.up, angleSpeed);
    }

}
