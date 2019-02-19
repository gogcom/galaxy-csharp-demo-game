using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopUps : MonoBehaviour {

	public GameObject popUpWithButton;
	private Text popUpWithButtonText;
	private Button popUpWithButtonButton;
	CursorLockMode lastState = CursorLockMode.None;

	void Awake()
	{
		popUpWithButtonText = popUpWithButton.transform.Find("Text").GetComponent<Text>();
		popUpWithButtonButton = popUpWithButton.transform.Find("Button").GetComponent<Button>();
	}

	public void MenuHostLeftLobby () 
	{
		popUpWithButton.SetActive(true);
		lastState = Cursor.lockState;
		MouseController.ChangeMouseLockMode(CursorLockMode.None);

		popUpWithButtonText.text = "Host left the lobby";
		popUpWithButtonButton.onClick.AddListener (() => {
			ClosePopUps();
		});
	}

	public void MenuCouldNotCreate () 
	{
		popUpWithButton.SetActive(true);
		lastState = Cursor.lockState;
		MouseController.ChangeMouseLockMode(CursorLockMode.None);

		popUpWithButtonText.text = "Could not create lobby";
		popUpWithButtonButton.onClick.AddListener (() => {
			ClosePopUps();
		});
	}

	public void MenuCouldNotJoin (string reason)
	{
		popUpWithButton.SetActive(true);
		lastState = Cursor.lockState;
		MouseController.ChangeMouseLockMode(CursorLockMode.None);

		popUpWithButtonText.text = "Could not join lobby\nReason: " + reason;
		popUpWithButtonButton.onClick.AddListener (() => {
			ClosePopUps();
		});
	}

	public void GameWaitingForOtherPlayer () 
	{
		popUpWithButton.SetActive(true);
		lastState = Cursor.lockState;
		MouseController.ChangeMouseLockMode(CursorLockMode.None);

		popUpWithButtonButton.onClick.AddListener ( () => {
			GalaxyManager.Instance.Matchmaking.LeaveLobby();
			SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
		});
	}

	public void GameClientLeftLobby () 
	{
		popUpWithButton.SetActive(true);
		lastState = Cursor.lockState;
		MouseController.ChangeMouseLockMode(CursorLockMode.None);

		popUpWithButtonText.text = "Other player left the lobby";
		popUpWithButtonButton.onClick.AddListener (() => {
			GalaxyManager.Instance.Matchmaking.LeaveLobby();
			SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
		});
	}

	public void ConnectionToLobbyLost () 
	{
		popUpWithButton.SetActive(true);
		lastState = Cursor.lockState;
		MouseController.ChangeMouseLockMode(CursorLockMode.None);

		popUpWithButtonText.text = "Connection to lobby lost lobby";
		popUpWithButtonButton.onClick.AddListener (() => {
			SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
		});
	}

	public void GameHostLeftLobby () 
	{
		popUpWithButton.SetActive(true);
		lastState = Cursor.lockState;
		MouseController.ChangeMouseLockMode(CursorLockMode.None);

		popUpWithButtonText.text = "Host left the lobby";
		popUpWithButtonButton.onClick.AddListener (() => {
			SceneController.Instance.LoadScene(SceneController.SceneName.MainMenu, true);
		});
	}

	public void ClosePopUps () 
	{
		if (popUpWithButton.activeInHierarchy) popUpWithButton.SetActive(false);
		MouseController.ChangeMouseLockMode(lastState);
	}
	
}
