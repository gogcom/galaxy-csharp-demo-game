using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnlineController : PlayerController {

    void Awake ()
    {
        player = gameObject.GetComponent<PlayerOnline>();
        whiteBall = GameObject.Find ("WhiteBall");
        thrust = 1f;
        rotationSpeed = 100f;
        offset = GameManager.Instance.offset;
    }

    // On every frame
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
                if (((PlayerOnline)player).Active) Skip();
                RotatePlayerAroundTarget();
                break;
        }
        ChangeCamera();
    }

    protected override void GuiControls()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (((Online2PlayerGameManager)GameManager.Instance).ChatOpen) ((Online2PlayerGameManager)GameManager.Instance).ChatOpen = false;
            else GameManager.Instance.InMenu = !GameManager.Instance.InMenu;
        }
        if (Input.GetKeyDown(KeyCode.F1) && !GameManager.Instance.InMenu) GameManager.Instance.help.SetActive(!GameManager.Instance.help.activeInHierarchy);
        if (Input.GetKeyDown(KeyCode.F2) && !GameManager.Instance.InMenu) GameManager.Instance.hint.SetActive(!GameManager.Instance.hint.activeInHierarchy);
        if (Input.GetKeyDown(KeyCode.T) && !GameManager.Instance.InMenu && !((Online2PlayerGameManager)GameManager.Instance).ChatOpen) ((Online2PlayerGameManager)GameManager.Instance).ChatOpen = true;
        if (Input.GetKeyDown(KeyCode.Return) && ((Online2PlayerGameManager)GameManager.Instance).ChatOpen)
        {
            ((Online2PlayerGameManager)GameManager.Instance).chatWindow.GetComponent<ChatGameController>().SendLobbyMessage();
            ((Online2PlayerGameManager)GameManager.Instance).ChatOpen = false;
        }
    }
	
}
