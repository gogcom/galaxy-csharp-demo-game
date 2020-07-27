using UnityEngine;
using UnityEngine.UI;

public class PopUps : MonoBehaviour {

	public GameObject popUp;
	public Text popUpText;
	public Button popUpButton;
	public Text popUpButtonText;
	CursorLockMode lastState = CursorLockMode.None;

	public void PopUpWithClosePopUpsButton(string popUpText, string buttonText) 
	{	
		PopUpWithButton(popUpText, buttonText);
		popUpButton.onClick.AddListener (() => {
			ClosePopUps();
		});
	}

	public void PopUpWithLeaveLobbyButton(string popUpText, string buttonText) 
	{
		PopUpWithButton(popUpText, buttonText);
		popUpButton.onClick.AddListener (() => {
			GalaxyManager.Instance.Matchmaking.LeaveLobby();
			SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
		});
	}

	public void PopUpWithLoadSceneButton(string popUpText, string buttonText) 
	{
		PopUpWithButton(popUpText, buttonText);
		popUpButton.onClick.AddListener (() => {
			SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
		});
	}

	public void ClosePopUps()
	{
		if (popUp.activeInHierarchy) popUp.SetActive(false);
		MouseController.ChangeMouseLockMode(lastState);
	}

	private void PopUpWithButton(string desc, string button)
	{
		ClosePopUps();
		popUp.SetActive(true);
		lastState = Cursor.lockState;
		MouseController.ChangeMouseLockMode(CursorLockMode.None);
		popUpText.text = desc;
		popUpButtonText.text = button;
	}
	
}
