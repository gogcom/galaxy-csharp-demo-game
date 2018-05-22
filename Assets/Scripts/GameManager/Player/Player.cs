using UnityEngine;

public class Player : MonoBehaviour
{

    #region variables

    // Variable storing current player state
    public GameManager.PlayerState playerState;
    private string playerName;
    private int score;
    private GameObject model;
    protected GameObject cue;
    public PlayerController Controller;

    #endregion

    #region Get/Set

    public virtual GameManager.PlayerState PlayerState
    {
        get { return playerState; }
        set
        {
            playerState = value;
            switch (value)
            {
                case GameManager.PlayerState.Passive:
                    GameManager.Instance.hint.GetComponent<Hints>().NewHint("Passive");
                    cue.SetActive(false);
                    MouseController.ChangeMouseLockMode(CursorLockMode.Locked);
                    break;
                case GameManager.PlayerState.ActiveWIH:
                    GameManager.Instance.hint.GetComponent<Hints>().NewHint("Placing");
                    cue.SetActive(false);
                    MouseController.ChangeMouseLockMode(CursorLockMode.None);
                    break;
                case GameManager.PlayerState.ActiveClear:
                    GameManager.Instance.hint.GetComponent<Hints>().NewHint("Shooting");
                    cue.SetActive(true);
                    MouseController.ChangeMouseLockMode(CursorLockMode.Locked);
                    break;
            }
            Time.timeScale = 1.0f;
        }
    }

    public string PlayerName { get { return playerName; } set { playerName = value; } }
    public int Score { get { return score; } set { score = value; } }
    public GameObject Model { get { return model; } set { model = value; } }
    public GameObject Cue { get { return cue; } set { cue = value; } }

    #endregion

}