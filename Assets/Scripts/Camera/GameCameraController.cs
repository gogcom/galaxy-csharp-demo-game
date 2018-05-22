using System.Collections;
using UnityEngine;

public class GameCameraController : MonoBehaviour
{
    public enum CameraPositionType {
        Main,
        Top,
        Player
    }
    
    private bool coroutineActive = false;
    private GameObject currentCameraPositionObject;
    public GameObject CurrentCameraPositionObject
    { 
        get 
        { 
            return currentCameraPositionObject;
        }
        set
        {
            if (transform == value.transform) return;
            if (coroutineActive) StopAllCoroutines();
            StartCoroutine(MoveCameraSpeed(value));
        }
    }

    private bool dynamic = false;
    public bool Dynamic {
        get 
        { 
            return dynamic;
        }
        set 
        { 
            dynamic = value;
        }
    }

    void Awake () 
    {
        DontDestroyOnLoad(gameObject);
    }

    void LateUpdate()
    {
        if (dynamic && !coroutineActive)
        {
            if (Vector3.Distance(transform.position, currentCameraPositionObject.transform.position) > 0.25f) StartCoroutine(MoveCameraSpeed(currentCameraPositionObject));
            else
            {
                transform.position = currentCameraPositionObject.transform.position;
                transform.rotation = currentCameraPositionObject.transform.rotation;
            }
        }
    }

    private IEnumerator MoveCameraTime(GameObject endPoint, float timeInSeconds = 0.2f)
    {
        if (timeInSeconds == 0f) yield break;
        coroutineActive = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float timeStart = Time.time;
        float timePassed = Time.time - timeStart;
        float frac = 0;

        while (timePassed < timeInSeconds)
        {
            frac = timePassed / timeInSeconds;
            transform.position = Vector3.Lerp(startPosition, endPoint.transform.position, frac);
            transform.rotation = Quaternion.Slerp(startRotation, endPoint.transform.rotation, frac);
            timePassed = Time.time - timeStart;
            yield return null;
        }

        currentCameraPositionObject = endPoint;
        coroutineActive = false;
    }

    private IEnumerator MoveCameraSpeed(GameObject endPoint, float speedPerSecond = 4f)
    {
        if (transform.position == endPoint.transform.position) yield break;
        coroutineActive = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float distanceFull = Vector3.Distance(transform.position, endPoint.transform.position);

        if (distanceFull < 0.1f)
        {
            coroutineActive = false;
            StartCoroutine(MoveCameraTime(endPoint));
            yield break;
        }

        float fps = Time.timeScale / Time.deltaTime;
        float distanceCovered = 0;
        float speedPerFrame = speedPerSecond / fps;
        float fracDistance = distanceCovered / distanceFull;

        while (distanceCovered < distanceFull)
        {
            fracDistance = distanceCovered / distanceFull;
            transform.position = Vector3.Lerp(startPosition, endPoint.transform.position, fracDistance);
            transform.rotation = Quaternion.Slerp(startRotation, endPoint.transform.rotation, fracDistance);
            fps = Time.timeScale / Time.deltaTime;
            speedPerFrame = speedPerSecond / fps;
            distanceCovered += speedPerFrame;
            yield return null;
        }

        currentCameraPositionObject = endPoint;
        coroutineActive = false;
    }

}