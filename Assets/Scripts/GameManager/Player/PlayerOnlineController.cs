using UnityEngine;

public class PlayerOnlineController : PlayerController {

    private Online2PlayerGameManager onlineGameManager;
    private PlayerOnline playerOnline;

    void Awake ()
    {
        player = gameObject.GetComponent<PlayerOnline>();
        whiteBall = GameObject.Find ("WhiteBall");
        thrust = 1f;
        rotationSpeed = 100f;
        offset = GameManager.Instance.offset;
        playerOnline = (PlayerOnline)player;
        onlineGameManager = (Online2PlayerGameManager)GameManager.Instance;
    }

    void Update()
    {
        if (GameManager.Instance.GameFinished) return;
        GuiControls();
        FollowTarget();
        if (GameManager.Instance.InMenu ||
            MouseController.Overriden || 
            onlineGameManager.ChatOpen) return;
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
                if (playerOnline.Active) Skip();
                RotatePlayerAroundTarget();
                break;
        }
        ChangeCamera();
    }

    protected override void GuiControls()
    {        
        if (Input.GetKeyDown(KeyCode.F1) && !GameManager.Instance.InMenu) onlineGameManager.help.SetActive(!onlineGameManager.help.activeInHierarchy);
        if (Input.GetKeyDown(KeyCode.F2) && !GameManager.Instance.InMenu) onlineGameManager.hint.SetActive(!onlineGameManager.hint.activeInHierarchy);
        if (Input.GetKeyDown(KeyCode.T) && !GameManager.Instance.InMenu && !onlineGameManager.ChatOpen) {
            onlineGameManager.ChatOpen = true;
            onlineGameManager.chatController.messagePrompt.ActivateInputField();
        }
        if (Input.GetKeyDown(KeyCode.Return) && onlineGameManager.ChatOpen) {
            onlineGameManager.chatController.SendLobbyMessage();
            onlineGameManager.ChatOpen = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (onlineGameManager.ChatOpen) onlineGameManager.ChatOpen = false;
            else GameManager.Instance.InMenu = !GameManager.Instance.InMenu;
        }
    }
	
}
