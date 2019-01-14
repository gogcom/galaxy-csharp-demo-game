using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DebugController : MonoBehaviour {

	public GUIStyle disclaimerHeader;
	public GUIStyle disclaimerText;
	public GUIStyle overrideText;

	void Awake()
	{
		DontDestroyOnLoad(this);
		Application.logMessageReceived += DebugLog;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F9)) topBarEnabled = !topBarEnabled;
		if (Input.GetKeyDown(KeyCode.F10)) MouseController.Overriden = !MouseController.Overriden;
	}

	void OnGUI()
	{
		Disclaimer();
		if (topBarEnabled) TopBar();
		if (MouseController.Overriden) GUI.Label(new Rect(Screen.width/2-80,Screen.height/2-10,160,20), "MOUSE OVERRIDE ON",overrideText);
		if (appsWindowEnabled) appsWindowRect = GUI.Window(0,appsWindowRect,AppsWindow,"Apps");
		if (appsWindowEnabled && productIDWindowEnabled) ProductIDWindow();
		if (achievementsWindowEnabled) achievementsWindowRect = GUI.Window(1,achievementsWindowRect,AchievementsWindow,"Achievements");
		if (achievementsWindowEnabled && achievementsApiKeyWindowEnabled) AchievemntsApiKeysWindow();
		if (statisticsWindowEnabled) statisticsWindowRect = GUI.Window(2,statisticsWindowRect,StatisticsWindow,"Statistics");
		if (statisticsWindowEnabled && statisticsFloatApiKeyWindowEnabled) StatisticsFloatApiKeysWindow();
		if (statisticsWindowEnabled && statisticsIntApiKeyWindowEnabled) StatisticsIntApiKeysWindow();
		if (leaderboardsWindowEnabled) leaderboardsWindowRect = GUI.Window(3,leaderboardsWindowRect,LeaderboardsWindow,"Leaderboards");
		if (leaderboardsWindowEnabled && leaderboardsApiKeyWindowEnabled) LeaderboardsApiKeysWindow();
		if (userStatusWindowEnabled) userStatusWindowRect = GUI.Window(4,userStatusWindowRect,UserStatusWindow,"User status");
		if (storageWindowEnabled) storageWindowRect = GUI.Window(5,storageWindowRect,StorageWindow,"Storage");
		if (gameWindowEnabled) gameWindowRect = GUI.Window(6,gameWindowRect,GameWindow,"Game");
		if (argumentsWindowEnabled) argumentsWindowRect = GUI.Window(7,argumentsWindowRect,ArgumentsWindow,"Arguments");
		if (debugConsoleWindowEnabled) debugConsoleWindowRect = GUI.Window(8,debugConsoleWindowRect,DebugConsole,"Console");
	}

	void OnDestroy()
	{
		Application.logMessageReceived -= DebugLog;
	}

	/****************
	*	DISCLAIMER	*
	****************/
	Rect disclaimerRect = new Rect (Screen.width-172,2,170,57);
	private void Disclaimer()
	{
		if (topBarEnabled) disclaimerRect.y = 22;
		else disclaimerRect.y = 2;

		GUI.BeginGroup(disclaimerRect);

		GUI.Label(new Rect(0,0,170,12),"GOG Galaxy® Integration Demo",disclaimerHeader);

		GUI.Label(new Rect(0,12,170,25),"Created for demonstration of GOG Galaxy SDK basic implementation.\nF10 overrides mouse lock state\nF9 toggles debug menu.",disclaimerText);

		GUI.EndGroup();
	}

	/****************
	*	TOP BAR 	*
	****************/
	bool topBarEnabled = false;
	private void TopBar ()
	{
		GUI.BeginGroup(new Rect(0,0,Screen.width,20));
		
		GUI.Box(new Rect(0,0,Screen.width,20),"");

		if (GUI.Button(new Rect(0,0,100,20),"Apps")) appsWindowEnabled = !appsWindowEnabled;
		if (GUI.Button(new Rect(100,0,100,20),"Achievements")) achievementsWindowEnabled = !achievementsWindowEnabled;
		if (GUI.Button(new Rect(200,0,100,20),"Statistics")) statisticsWindowEnabled = !statisticsWindowEnabled;
		if (GUI.Button(new Rect(300,0,100,20),"Leaderboards")) leaderboardsWindowEnabled = !leaderboardsWindowEnabled;
		if (GUI.Button(new Rect(400,0,100,20),"User status")) userStatusWindowEnabled = !userStatusWindowEnabled;
		if (GUI.Button(new Rect(500,0,100,20),"Storage")) storageWindowEnabled = !storageWindowEnabled;
		if (GUI.Button(new Rect(600,0,100,20),"Game")) gameWindowEnabled = !gameWindowEnabled;
		if (GUI.Button(new Rect(700,0,100,20),"Arguments")) argumentsWindowEnabled = !argumentsWindowEnabled;
		if (GUI.Button(new Rect(800,0,100,20),"Console")) debugConsoleWindowEnabled = !debugConsoleWindowEnabled;
		if (GUI.Button(new Rect(Screen.width-20,0,20,20),"X")) topBarEnabled = false;

		GUI.EndGroup();
	}

	/************
	*	APPS	*
	************/
	bool appsWindowEnabled = false;
	Rect appsWindowRect = new Rect(0,20,335,120);
	string appsDlcProductID = "productID";

	private void AppsWindow (int windowID) 
	{
		if (GUI.Button (new Rect(318,2,15,15),"X")) appsWindowEnabled = false;

		if (GUI.Button(new Rect(5,20,325,20),"GetCurrentGameLanguage")) GalaxyManager.Instance.GetCurrentGameLanguage();

		if (GUI.Button(new Rect(5,45,160,20),"IsDlcInstalled")) GalaxyManager.Instance.IsDlcInstalled(ulong.Parse(appsDlcProductID));
		appsDlcProductID = GUI.TextField(new Rect(170,45,135,20),appsDlcProductID);
		if (GUI.Button(new Rect(310,45,20,20),"?")) productIDWindowEnabled = !productIDWindowEnabled;

		if (GUI.Button(new Rect(5,70,325,20),"SignedIn")) GalaxyManager.Instance.IsSignedIn();

		if (GUI.Button(new Rect(5,95,325,20),"IsLoggedOn")) GalaxyManager.Instance.IsLoggedOn();

		GUI.DragWindow();
	}

	private bool productIDWindowEnabled = false;
	Rect productIDWindowRect = new Rect(335,20,120,60);
	private void ProductIDWindow ()
	{
		productIDWindowRect.Set(appsWindowRect.x + 335,appsWindowRect.y + 45,120,60);

		GUI.BeginGroup(productIDWindowRect);

		if (GUI.Button(new Rect(0,0,120,20),"Base game"))
		{
			appsDlcProductID = "1767680123";
			productIDWindowEnabled = false;
		}

		if (GUI.Button(new Rect(0,20,120,20),"DLC A")) 
		{
			appsDlcProductID = "1751126893";
			productIDWindowEnabled = false;
		}

		if (GUI.Button(new Rect(0,40,120,20),"DLC B")) 
		{
			appsDlcProductID = "1281799802";
			productIDWindowEnabled = false;
		}

		GUI.EndGroup();

	}

	/********************
	*	ACHIEVEMENTS	*
	********************/
	bool achievementsWindowEnabled = false;
	Rect achievementsWindowRect = new Rect(100,20,170,120);
	string achievementApiKey = "apiKey";

	private void AchievementsWindow (int windowID) 
	{
		if (GUI.Button (new Rect(153,2,15,15),"X")) achievementsWindowEnabled = false;

		achievementApiKey = GUI.TextField(new Rect(5,20,135,20),achievementApiKey);
		if (GUI.Button(new Rect(145,20,20,20),"?")) achievementsApiKeyWindowEnabled = !achievementsApiKeyWindowEnabled;

		if (GUI.Button(new Rect(5,45,160,20),"SetAchievement")) GalaxyManager.Instance.StatsAndAchievements.SetAchievement(achievementApiKey);

		if (GUI.Button(new Rect(5,70,160,20),"GetAchievement")) GalaxyManager.Instance.StatsAndAchievements.GetAchievement(achievementApiKey);

		if (GUI.Button(new Rect(5,95,160,20),"ResetStatsAndAchievements")) GalaxyManager.Instance.StatsAndAchievements.ResetStatsAndAchievements();

		GUI.DragWindow();
	}

	bool achievementsApiKeyWindowEnabled = false;
	Vector2 achievementsApiKeyWindowScrollViewVector = Vector2.zero;
	Rect achievementsApiKeyWindowViewRect = new Rect (330,20,136,80);
	Rect achievementsApiKeyWindowContentRect = new Rect (0,0,120,80);
	
	private void AchievemntsApiKeysWindow ()
	{
		float height = GalaxyManager.Instance.achievementsList.Length * 20;
		float horizontalPosition = 0;

		achievementsApiKeyWindowViewRect.Set(achievementsWindowRect.x + 170,achievementsWindowRect.y + 20,136,80);
		achievementsApiKeyWindowContentRect.Set(0,0,120,height);

		achievementsApiKeyWindowScrollViewVector = GUI.BeginScrollView(achievementsApiKeyWindowViewRect,achievementsApiKeyWindowScrollViewVector,achievementsApiKeyWindowContentRect);
		foreach (string entry in GalaxyManager.Instance.achievementsList)
		{
			if (GUI.Button (new Rect(0, horizontalPosition, 120, 20), entry))
			{
				achievementApiKey = entry;
				achievementsApiKeyWindowEnabled = false;
			}
			horizontalPosition += 20;
		}
		GUI.EndScrollView();
	}

	/****************
	*	STATISTICS	*
	****************/
	bool statisticsWindowEnabled = false;
	Rect statisticsWindowRect = new Rect(200,20,335,195);
	string statisticFloatApiKey = "floatApiKey";
	string statisticIntApiKey = "intApiKey";
	string statisticFloatValue = "floatValue";
	string statisticIntValue = "intValue";

	private void StatisticsWindow (int windowID)
	{
		if (GUI.Button (new Rect(318,2,15,15),"X")) statisticsWindowEnabled = false;

		statisticFloatApiKey = GUI.TextField(new Rect(5,20,300,20),statisticFloatApiKey);
		if (GUI.Button(new Rect(310,20,20,20),"?")) statisticsFloatApiKeyWindowEnabled = !statisticsFloatApiKeyWindowEnabled;

		if (GUI.Button(new Rect(5,45,160,20),"SetStatFloat")) GalaxyManager.Instance.StatsAndAchievements.SetStatFloat(statisticFloatApiKey,float.Parse(statisticFloatValue));
		statisticFloatValue = GUI.TextField(new Rect(170,45,160,20),statisticFloatValue);

		if (GUI.Button(new Rect(5,70,325,20),"GetStatFloat")) GalaxyManager.Instance.StatsAndAchievements.GetStatFloat(statisticFloatApiKey);

		statisticIntApiKey = GUI.TextField(new Rect(5,95,300,20),statisticIntApiKey);
		if (GUI.Button(new Rect(310,95,20,20),"?")) statisticsIntApiKeyWindowEnabled = !statisticsIntApiKeyWindowEnabled;

		if (GUI.Button(new Rect(5,120,160,20),"SetStatInt")) GalaxyManager.Instance.StatsAndAchievements.SetStatInt(statisticIntApiKey,int.Parse(statisticIntValue));
		statisticIntValue = GUI.TextField(new Rect(170,120,160,20),statisticIntValue);

		if (GUI.Button(new Rect(5,145,325,20),"GetStatInt")) GalaxyManager.Instance.StatsAndAchievements.GetStatInt(statisticIntApiKey);

		if (GUI.Button(new Rect(5,170,325,20),"ResetStatsAndAchievements")) GalaxyManager.Instance.StatsAndAchievements.ResetStatsAndAchievements();

		GUI.DragWindow();
	}

	bool statisticsFloatApiKeyWindowEnabled = false;
	Vector2 statisticsFloatApiKeyWindowScrollViewVector = Vector2.zero;
	Rect statisticsFloatApiKeyWindowViewRect = new Rect (330,20,136,40);
	Rect statisticsFloatApiKeyWindowContentRect = new Rect (0,0,120,40);
	private void StatisticsFloatApiKeysWindow ()
	{
		float height = GalaxyManager.Instance.statisticsFloatList.Count * 20;
		float horizontalPosition = 0;

		statisticsFloatApiKeyWindowViewRect.Set(statisticsWindowRect.x + 335, statisticsWindowRect.y + 15, 136, 40);
		statisticsFloatApiKeyWindowContentRect.Set(0,0,120,height);

		statisticsFloatApiKeyWindowScrollViewVector = GUI.BeginScrollView(statisticsFloatApiKeyWindowViewRect,statisticsFloatApiKeyWindowScrollViewVector,statisticsFloatApiKeyWindowContentRect);
		foreach (KeyValuePair<string,string> entry in GalaxyManager.Instance.statisticsFloatList)
		{
			if (GUI.Button (new Rect(0, horizontalPosition, 120, 20), entry.Value))
			{
				statisticFloatApiKey = entry.Key;
				statisticsFloatApiKeyWindowEnabled = false;
			}
			horizontalPosition += 20;
		}
		GUI.EndScrollView();
	}

	bool statisticsIntApiKeyWindowEnabled = false;
	Vector2 statisticsIntApiKeyWindowScrollViewVector = Vector2.zero;
	Rect statisticsIntApiKeyWindowViewRect = new Rect (330,20,136,40);
	Rect statisticsIntApiKeyWindowContentRect = new Rect (0,0,120,40);
	private void StatisticsIntApiKeysWindow ()
	{
		float height = GalaxyManager.Instance.statisticsIntList.Count * 20;
		float horizontalPosition = 0;

		statisticsIntApiKeyWindowViewRect.Set(statisticsWindowRect.x + 335, statisticsWindowRect.y + 90, 136, 40);
		statisticsIntApiKeyWindowContentRect.Set(0,0,120,height);

		statisticsIntApiKeyWindowScrollViewVector = GUI.BeginScrollView(statisticsIntApiKeyWindowViewRect,statisticsIntApiKeyWindowScrollViewVector,statisticsIntApiKeyWindowContentRect);
		foreach (KeyValuePair<string,string> entry in GalaxyManager.Instance.statisticsIntList)
		{
			if (GUI.Button (new Rect(0, horizontalPosition, 120, 20), entry.Value))
			{
				statisticIntApiKey = entry.Key;
				statisticsIntApiKeyWindowEnabled = false;
			}
			horizontalPosition += 20;
		}
		GUI.EndScrollView();
	}

	/********************
	*	LEADERBOARDS	*
	********************/
	bool leaderboardsWindowEnabled = false;
	Rect leaderboardsWindowRect = new Rect(300,20,335,120);
	string leaderboardApiKey = "apiKey";
	string leaderboardFrom = "From";
	string leaderboardTo = "To";
	string leaderboardBefore = "Before";
	string leaderboardAfter = "After";
	string leaderboardScore = "Score";

	private void LeaderboardsWindow (int windowID)
	{
		if (GUI.Button (new Rect(318,2,15,15),"X")) leaderboardsWindowEnabled = false;
		
		leaderboardApiKey = GUI.TextField(new Rect(5,20,300,20),leaderboardApiKey);
		if (GUI.Button(new Rect(310,20,20,20),"?")) leaderboardsApiKeyWindowEnabled = !leaderboardsApiKeyWindowEnabled;

		if (GUI.Button(new Rect(5,45,160,20),"ReqLeadEntGlobal")) GalaxyManager.Instance.Leaderboards.RequestLeaderboardEntriesGlobal(leaderboardApiKey, uint.Parse(leaderboardFrom), uint.Parse(leaderboardTo));
		leaderboardFrom = GUI.TextField(new Rect(170,45,77.5f,20),leaderboardFrom);
		leaderboardTo = GUI.TextField(new Rect(252.5f,45,77.5f,20),leaderboardTo);

		if (GUI.Button(new Rect(5,70,160,20),"ReqLeadEntAroundUser")) GalaxyManager.Instance.Leaderboards.RequestLeaderboardEntriesAroundUser(leaderboardApiKey, uint.Parse(leaderboardBefore), uint.Parse(leaderboardAfter), GalaxyManager.Instance.MyGalaxyID);
		leaderboardBefore = GUI.TextField(new Rect(170,70,77.5f,20),leaderboardBefore);
		leaderboardAfter = GUI.TextField(new Rect(252.5f,70,77.5f,20),leaderboardAfter);

		if (GUI.Button(new Rect(5,95,160,20),"SetLeaderboardEntry")) GalaxyManager.Instance.Leaderboards.SetLeaderboardScore(leaderboardApiKey, int.Parse(leaderboardScore));
		leaderboardScore = GUI.TextField(new Rect(170,95,160,20),leaderboardScore);

		GUI.DragWindow();
	}

	bool leaderboardsApiKeyWindowEnabled = false;
	Vector2 leaderboardsApiKeyWindowScrollViewVector = Vector2.zero;
	Rect leaderboardsApiKeyWindowViewRect = new Rect (330,20,136,60);
	Rect leaderboardsApiKeyWindowContentRect = new Rect (0,0,120,60);
	private void LeaderboardsApiKeysWindow ()
	{
		float height = GalaxyManager.Instance.leaderboardNames.Count * 20;
		float horizontalPosition = 0;

		leaderboardsApiKeyWindowViewRect.Set(leaderboardsWindowRect.x + 335, leaderboardsWindowRect.y + 20, 136, 60);
		leaderboardsApiKeyWindowContentRect.Set(0,0,120,height);

		leaderboardsApiKeyWindowScrollViewVector = GUI.BeginScrollView(leaderboardsApiKeyWindowViewRect,leaderboardsApiKeyWindowScrollViewVector,leaderboardsApiKeyWindowContentRect);
		foreach (KeyValuePair<string,string> entry in GalaxyManager.Instance.leaderboardNames)
		{
			if (GUI.Button (new Rect(0, horizontalPosition, 120, 20), entry.Key))
			{
				leaderboardApiKey = entry.Value;
				leaderboardsApiKeyWindowEnabled = false;
			}
			horizontalPosition += 20;
		}
		GUI.EndScrollView();
	}

	/********************
	*	USER STATUS		*
	********************/
	bool userStatusWindowEnabled = false;
	Rect userStatusWindowRect = new Rect(400,20,170,105);
	private void UserStatusWindow (int windowID) 
	{
		if (GUI.Button (new Rect(153,2,15,15),"X")) userStatusWindowEnabled = false;

		GUI.Label(new Rect(5,20,160,20),"Init: " + GalaxyManager.Instance.GalaxyFullyInitialized.ToString());
		GUI.Label(new Rect(5,40,160,20),"SignedIn: " + GalaxyManager.Instance.IsSignedIn(true).ToString());
		GUI.Label(new Rect(5,60,160,20),"LoggedOn: " + GalaxyManager.Instance.IsLoggedOn(true).ToString());
		if (GalaxyManager.Instance.Friends) GUI.Label(new Rect(5,80,160,20),"Username: " + GalaxyManager.Instance.Friends.GetMyUsername(true).ToString());

		GUI.DragWindow();
	}

	/****************
	*	STORAGE		*
	****************/
	bool storageWindowEnabled = false;
	Rect storageWindowRect = new Rect(500,20,335,195);
	string inputFileWriteTarget = "absolutInputPath";
	string fileRemoveTarget = "localStorageRelativePath";
	string fileNameShare = "localStorageRelativePath";
	string fileNameDownload = "fileName";
	string userIDDownload = "userID";
	string fileSharedID = "sharedFileID";

	private void StorageWindow (int windowID) 
	{
		if (GUI.Button (new Rect(318,2,15,15),"X")) storageWindowEnabled = false;

		if (GUI.Button (new Rect(5,20,160,20), "FileWrite")) GalaxyManager.Instance.Storage.CopyFileToLocalStorage(inputFileWriteTarget);
		inputFileWriteTarget = GUI.TextField(new Rect(170,20,160,20), inputFileWriteTarget);

		if (GUI.Button (new Rect(5,45,160,20), "FileRemove")) GalaxyManager.Instance.Storage.RemoveFileFromLocalStorage(fileRemoveTarget);
		fileRemoveTarget = GUI.TextField(new Rect(170,45,160,20), fileRemoveTarget);

		if (GUI.Button (new Rect(5,70,160,20), "FileShare")) GalaxyManager.Instance.Storage.ShareFileFromLocalStorage(fileNameShare);
		fileNameShare = GUI.TextField(new Rect(170,70,160,20), fileNameShare);

		if (GUI.Button (new Rect(5,95,320,20), "FileShareAll")) GalaxyManager.Instance.Storage.ShareAllFilesFromLocalStorage();

		if (GUI.Button (new Rect(5,120,160,20), "FileDownloadBySharedFileID")) DownloadFileBySharedID(fileSharedID);
		fileSharedID = GUI.TextField(new Rect(170,120,160,20),fileSharedID);

		if (GUI.Button (new Rect(5,145,160,20), "FileDownloadByNameAndUser")) DownloadFileFromUser(fileNameDownload,userIDDownload);
		fileNameDownload = GUI.TextField(new Rect(170,145,77.5f,20),fileNameDownload);
		userIDDownload = GUI.TextField(new Rect(252.5f,145,77.5f,20),userIDDownload);

		if (GUI.Button (new Rect(5,170,320,20), "FileListAll")) 
		{
			string[] fileListArray;
			string fileListString;

			fileListArray = GalaxyManager.Instance.Storage.ListAllFilesFromOnlineStorage();
			fileListString= "";

			for(int i = 0; i < fileListArray.Length; i++) 
			{
				if (i == 0) 
				{
					fileListString = string.Concat("1. " + fileListArray[i]);
				} else {
					fileListString = string.Concat(fileListString,"\n",(i+1).ToString(),". ",fileListArray[i]);
				}
			}
			Debug.Log("File list:\n"+fileListString);
		}

		if (GUI.Button(new Rect(5,195,320,20),"Open Local Storage Dir")) 
		{
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
			string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/GOG.com/Galaxy/Applications/50225266424144145/Storage/Shared/Files";
			System.Diagnostics.Process.Start(path);
#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
			Debug.Log("What is the path on macOS??");
#endif
		}
		GUI.DragWindow();
	}

	private void DownloadFileBySharedID(string sharedFileIDString) 
	{
		ulong sharedFileIDUlong;
		sharedFileIDUlong = ulong.Parse(sharedFileIDString);
		GalaxyManager.Instance.Storage.DownloadSharedFile(sharedFileIDUlong);
	}

	private void DownloadFileFromUser(string fileName, string stringGalaxyID) 
	{
		Galaxy.Api.GalaxyID realGalaxyID;
		realGalaxyID = new Galaxy.Api.GalaxyID(ulong.Parse(stringGalaxyID));
		GalaxyManager.Instance.Storage.DownloadSharedFileFromUser(realGalaxyID,fileName);
	}

	/************
	*	GAME	*
	*************/
	bool gameWindowEnabled = false;
	Rect gameWindowRect = new Rect(600,20,170,225);

	private void GameWindow (int windowID) 
	{
		if (GUI.Button (new Rect(153,2,15,15),"X")) gameWindowEnabled = false;

		GUI.Label(new Rect(5,20,160,20), "Remove object:");

		DeactivateGameObjectButton(new Rect(5,40,160,20),"RedBall");

		DeactivateGameObjectButton(new Rect(5,60,160,20),"YellowBall");

		DeactivateGameObjectButton(new Rect(5,80,160,20),"BrownBall");

		DeactivateGameObjectButton(new Rect(5,100,160,20),"GreenBall");

		DeactivateGameObjectButton(new Rect(5,120,160,20),"BlueBall");

		DeactivateGameObjectButton(new Rect(5,140,160,20),"PinkBall");

		DeactivateGameObjectButton(new Rect(5,160,160,20),"BlackBall");

		GUI.Label(new Rect(5,180,160,20), "Other:");

		if (GUI.Button(new Rect(5,200,160,20), "Finish game")) 
		{
			if (GameObject.FindGameObjectWithTag("PoolBalls"))
			{
				GameObject[] poolBalls = GameObject.FindGameObjectsWithTag("PoolBalls");
				foreach (GameObject ball in poolBalls)
				{
					if (ball.name != "WhiteBall") ball.SetActive(false);
				}
				Debug.Log("PoolBalls deactivated, attempting to call GameManager.GameEnd() method.");
			}
			else Debug.Log("Could not find any pool balls, attempting to call GameManager.GameEnd() method.");
			if (GameManager.Instance) GameManager.Instance.GameEnd();
			else Debug.Log("Could not finish game because GameManager.Instance is null");
		}

		GUI.DragWindow();
	}

	private void DeactivateGameObjectButton (Rect rect, string name)
	{
		if (GUI.Button(rect, name)) 
		{
			if (GameObject.Find(name)) GameObject.Find(name).SetActive(false);
			else Debug.Log("Could not find object " + name);
		}
	}

	/****************
	*	Arguments	*
	****************/

	bool argumentsWindowEnabled = false;
	Rect argumentsWindowRect = new Rect(700, 20, 335, 225);
	

	private string printCommandLineArguments() {
		string argList = null;
		string[] args = Environment.GetCommandLineArgs();

        foreach (string arg in args) {
            argList += arg + "\n";
        }

		return argList;
	}

	private void ArgumentsWindow(int windowID) 
	{
		if (GUI.Button (new Rect(318,2,15,15),"X")) argumentsWindowEnabled = false;

		GUI.Label(new Rect(5, 20, 325, 200), printCommandLineArguments());
		
		GUI.DragWindow();
	}

	/************
	*	CONSOLE	*
	************/
	struct LogEntry 
	{
		public string logString;
		public string stackTrace;
		public LogType type;
		public LogEntry (string pLogString, string pStackTrace, LogType pType)
		{
			logString = pLogString;
			stackTrace = pStackTrace;
			type = pType;
		}
	}
	List<LogEntry> logEntries = new List<LogEntry>();

	void DebugLog (string logString, string stackTrace, LogType type)
	{
		if(logEntries.Count > 30)
		{
			logEntries.RemoveAt(0);
		}
		logEntries.Add(new LogEntry(logString, stackTrace, type));
	}

	bool debugConsoleWindowEnabled = false;
	Rect debugConsoleWindowRect = new Rect(0,Screen.height - 225,Screen.width,225);
	Vector2 debugConsoleWindowScrollViewVector = Vector2.zero;
	Rect debugConsolWindowViewRect = new Rect(5, 20, Screen.width - 10, 200);
	Rect debugConsoleWindowContentRect = new Rect(0, 0, Screen.width - 26, 200);

	void DebugConsole (int windowID)
	{
		float height = logEntries.Count * 20;
		float horizontalPosition = 0;

		debugConsoleWindowContentRect.Set(0, 0, Screen.width - 26, height);

		if (GUI.Button (new Rect(debugConsoleWindowRect.width - 17,2,15,15),"X")) debugConsoleWindowEnabled = false;

		debugConsoleWindowScrollViewVector = GUI.BeginScrollView(debugConsolWindowViewRect,debugConsoleWindowScrollViewVector,debugConsoleWindowContentRect);
		foreach (LogEntry entry in logEntries)
		{
			GUI.Label(new Rect(0,horizontalPosition,debugConsoleWindowContentRect.width,20), entry.type.ToString() + ": " + entry.logString);
			horizontalPosition += 20;
		}
		GUI.EndScrollView();
	}

}
