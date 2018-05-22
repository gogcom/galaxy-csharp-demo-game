using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

	public static SceneController Instance;
	public enum SceneName {
		StartingScreen,
		GameObjects,
		MainMenu,
		Local1PlayerGame,
		Local2PlayerGame,
		Online2PlayerGame
	}
	public Dictionary<string,SceneName> MapStringToSceneName = new Dictionary<string, SceneName>() {
		{"StartingScreen" , SceneName.StartingScreen},
		{"GameObjects" , SceneName.GameObjects},
		{"MainMenu" , SceneName.MainMenu},
		{"Local1PlayerGame" , SceneName.Local1PlayerGame},
		{"Local2PlayerGame" , SceneName.Local2PlayerGame},
		{"Online2PlayerGame" , SceneName.Online2PlayerGame}
	};
	List<int> scenesToLoad = new List<int>();
	bool sceneLoading = false;
	List<int> scenesLoaded = new List<int>();
	List<int> scenesToUnload = new List<int>();
	bool sceneUnloading = false;

	void Awake () {
		if (Instance == null) {
			DontDestroyOnLoad(gameObject);
			Instance = this;
		} else {
			Destroy(this);
		}
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
	}

	void Update()
	{
		if (scenesToLoad.Count > 0 && !sceneLoading) LoadNextInQueue();
		if (scenesToUnload.Count > 0 && !sceneUnloading) UnloadNextInQueue();
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
	}

	private void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
        scenesToLoad.Remove(scene.buildIndex);
		scenesLoaded.Add(scene.buildIndex);
		sceneLoading = false;
    }

	private void OnSceneUnloaded (Scene scene) {
		scenesToUnload.Remove(scene.buildIndex);
		scenesLoaded.Remove(scene.buildIndex);
		sceneUnloading = false;
	}

	private void SwitchActiveScenesAndUnload (Scene scene, LoadSceneMode mode) {
		SwitchActiveScenesAndUnloadOld(scene.buildIndex);
		SceneManager.sceneLoaded -= SwitchActiveScenesAndUnload;
	}

	public void LoadScene (SceneName sceneToLoad, bool switchSceneWhenReady = false) {
		LoadScene((int)sceneToLoad, switchSceneWhenReady);
	}

	private void LoadScene (int sceneToLoad, bool switchSceneWhenReady = false) {
		if (!scenesLoaded.Contains(sceneToLoad) && !scenesToLoad.Contains(sceneToLoad)) {
			Debug.Log("Added " + sceneToLoad + " to scenesToLoad list");
			scenesToLoad.Add(sceneToLoad);
		} else {
			Debug.Log("Scene " + sceneToLoad + " already loaded or loading");
		}
		if (switchSceneWhenReady) {
			SceneManager.sceneLoaded += SwitchActiveScenesAndUnload;
		}
	}

	public void SwitchActiveScenesAndUnloadOld (SceneName newActiveScene) {
		SwitchActiveScenesAndUnloadOld((int)newActiveScene);
	}

	private void SwitchActiveScenesAndUnloadOld (int newActiveScene) {
		int oldActiveScene = SceneManager.GetActiveScene().buildIndex;
		if (scenesLoaded.Contains((int)newActiveScene)) {
			Debug.Log("Switching active scene from " + (SceneName)oldActiveScene + " to " + (SceneName)newActiveScene);
			SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(newActiveScene));
			UnloadScene(oldActiveScene);
		} else {
			Debug.LogWarning("Can't switch to scene " + newActiveScene + " because it is not loaded.");
		}
    }

	public void UnloadScene (SceneName sceneToUnload) {
		UnloadScene((int)sceneToUnload);
	}

	private void UnloadScene (int sceneToUnload) {
		if (scenesLoaded.Contains(sceneToUnload) && !scenesToUnload.Contains(sceneToUnload)) {
			Debug.Log("Added " + sceneToUnload + " to scenesToUnload list");
			scenesToUnload.Add(sceneToUnload);
		} else {
			Debug.Log("Scene " + sceneToUnload + " already unloaded or unloading");
		}
	}

	private void LoadNextInQueue () {
		Debug.Log("Loading next scene in queue, scene: " + scenesToLoad[0]);
		sceneLoading =  true;
		SceneManager.LoadSceneAsync(scenesToLoad[0], LoadSceneMode.Additive);
	}

	private void UnloadNextInQueue () {
		Debug.Log("Unloading next scene in queue, scene: " + scenesToUnload[0]);
		sceneUnloading =  true;
		SceneManager.UnloadSceneAsync(scenesToUnload[0]);
	}
	
}
