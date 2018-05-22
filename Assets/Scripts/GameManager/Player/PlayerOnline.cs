using UnityEngine;

public class PlayerOnline : Player
{
    private bool active;
    public bool Active
    {
        get 
        { 
            return active;
        }
        set
        {
            if (this == ((Online2PlayerGameManager)GameManager.Instance).me)
            {
                ((Online2PlayerGameManager)GameManager.Instance).slider.SetActive(value);
            }
            Time.timeScale = 1.0f;
            active = value;
            Debug.Log(gameObject.name + " active set to " + value);
        }
    }

    public override GameManager.PlayerState PlayerState
    {
        get { return playerState; }
        set
        {
            playerState = value;
            if (this == ((Online2PlayerGameManager)GameManager.Instance).me)
            {
                switch (value)
                {
                    case GameManager.PlayerState.Passive:
                        if ((((Online2PlayerGameManager)GameManager.Instance).me).Active) GameManager.Instance.hint.GetComponent<Hints>().NewHint("Passive");
                        else GameManager.Instance.hint.GetComponent<Hints>().NewHint("Waiting");
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
            }
            Time.timeScale = 1.0f;
        }
    }

}
