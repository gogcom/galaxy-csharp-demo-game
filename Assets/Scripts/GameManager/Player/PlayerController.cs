using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    protected Player player;
    protected float thrust;
    public float Thrust { get { return thrust; } set { thrust = value; } }
    protected float rotationSpeed;
    protected Vector3 offset;
    protected GameObject whiteBall;
    Ray ray;
    RaycastHit hit;

    void Awake ()
    {
        player = gameObject.GetComponent<Player>();
        whiteBall = GameObject.Find ("WhiteBall");
        thrust = 1f;
        rotationSpeed = 100f;
        offset = GameManager.Instance.offset;
    }

    void Update()
    {
        if (GameManager.Instance.GameFinished) return;
        GuiControls();
        FollowTarget();
        if (GameManager.Instance.InMenu || MouseController.Overriden) return;
        switch (player.PlayerState)
        {
            case GameManager.PlayerState.ActiveWIH:
                SetWhiteBall();
                break;
            case GameManager.PlayerState.ActiveClear:
                Shot();
                SetThrust();
                RotatePlayerAroundTarget();
                break;
            case GameManager.PlayerState.Passive:
                SetThrust();
                Skip();
                RotatePlayerAroundTarget();
                break;
        }
        ChangeCamera();
    }

	protected void SetWhiteBall()
    {

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!whiteBall.activeInHierarchy)
        {
            whiteBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            whiteBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            whiteBall.transform.position = new Vector3(1.1935f, 0.02625f, 0);
            whiteBall.SetActive(true);
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1, QueryTriggerInteraction.Collide) && hit.collider.name == "DSpot")
        {
            whiteBall.GetComponent<Rigidbody>().MovePosition(hit.point);
        }

        if (Input.GetMouseButtonUp(0) && !MouseController.Overriden)
        {
            GameManager.Instance.ActivePlayer.PlayerState = GameManager.PlayerState.ActiveClear;
            GameManager.Instance.CurrentCameraPositionType = GameManager.Instance.CurrentCameraPositionType;
        }

    }

    protected void RotatePlayerAroundTarget()
    {
        float rotation = Input.GetAxis("Mouse X") * rotationSpeed;
        rotation *= Time.deltaTime / Time.timeScale;
        offset = Quaternion.AngleAxis(rotation, Vector3.up) * offset;
        transform.position = whiteBall.transform.position + offset;
        transform.Rotate(0, rotation, 0);
    }

    protected void FollowTarget()
    {
        transform.position = whiteBall.transform.position + offset;
    }

    protected void SetThrust()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && thrust < 20)
        {
            thrust++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && thrust > 1)
        {
            thrust--;
        }
    }

    protected void Shot()
    {
        if (Input.GetMouseButtonUp(0))
        {
            whiteBall.GetComponent<Rigidbody>().AddForce(offset * -thrust, ForceMode.Impulse);
            StartCoroutine(GameManager.Instance.ShotStart());
        }
    }

    protected void Skip()
    {
        if (Input.GetMouseButtonUp(0) && Time.timeScale == 1.0f)
        {
            Time.timeScale = 8.0f;
            Debug.Log("Time scale set to " + Time.timeScale);
        }
    }

    protected void ChangeCamera()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {   
            if (GameManager.Instance.CurrentCameraPositionType == GameCameraController.CameraPositionType.Top) GameManager.Instance.CurrentCameraPositionType = GameCameraController.CameraPositionType.Player;
            else GameManager.Instance.CurrentCameraPositionType = GameCameraController.CameraPositionType.Top;
        }
    }

    protected virtual void GuiControls()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.InMenu = !GameManager.Instance.InMenu;
        if (Input.GetKeyDown(KeyCode.F1) && !GameManager.Instance.InMenu) GameManager.Instance.help.SetActive(!GameManager.Instance.help.activeInHierarchy);
        if (Input.GetKeyDown(KeyCode.F2) && !GameManager.Instance.InMenu) GameManager.Instance.hint.SetActive(!GameManager.Instance.hint.activeInHierarchy);
    }
	
}
