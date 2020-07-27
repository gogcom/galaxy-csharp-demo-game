using UnityEngine;

public class MainController : MonoBehaviour 
{
	void OnEnable () 
	{
		GalaxyManager.Instance.ShutdownMatchmaking();
	}
	
}
