using System.Collections;
using UnityEngine;

public class RotateAround : MonoBehaviour
{

    void OnEnable()
    {
        StartCoroutine(Rotate());
    }

    void OnDisable()
    {
        StopCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        for (;;)
        {
            gameObject.transform.Rotate(Vector3.back * 200 * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }

    }

}
