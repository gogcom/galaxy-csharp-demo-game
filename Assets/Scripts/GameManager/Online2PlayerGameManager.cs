using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Online2PlayerGameManager : GameManager
{

    #region Variables

    public Text endPlayer2Name;
    public Text endPlayer2Score;
    public PlayerOnline me;
    public PlayerOnline opponent;
    public GameObject slider;
    public override bool InMenu
    {
        get 
        { 
            return inMenu;
        }
        set
        {
			GameManager.Instance.escMenu.SetActive(value);
            MouseController.InMenu = value;
            inMenu = value;
        }
    }
    public GameObject chatWindow;
    public bool chatOpen;
    public bool ChatOpen 
    {
        get
        {
            return chatOpen;
        }
        set
        {
            chatWindow.SetActive(value);
            chatOpen = value;
        }
    }
	public override Player ActivePlayer 
	{
		get
		{
			return activePlayer;
		}
		set
		{
			activePlayer = value;
			Debug.Log("activePlayer set to " + value);
		}
	}
	public override Player PassivePlayer
		{
		get
		{
			return passivePlayer;
		}
		set
		{
			passivePlayer = value;
			Debug.Log("passivePlayer set to " + value);
		}
	}

     public override GameCameraController.CameraPositionType CurrentCameraPositionType { 
        get
        { 
            return currentCameraPositionType; 
        } 
        set 
        {
            switch(value)
            {
                case GameCameraController.CameraPositionType.Player:
                    if (me.PlayerState == GameManager.PlayerState.ActiveWIH) 
                        Camera.main.GetComponent<GameCameraController>().CurrentCameraPositionObject = GameObject.Find("CameraPositionWIHGround");
                    else
                        Camera.main.GetComponent<GameCameraController>().CurrentCameraPositionObject = me.transform.GetChild(1).gameObject;
                break;
                case GameCameraController.CameraPositionType.Top:
					Camera.main.GetComponent<GameCameraController>().CurrentCameraPositionObject = GameObject.Find("CameraPositionTop");
				break;
                default:
                    Camera.main.GetComponent<GameCameraController>().CurrentCameraPositionObject = GameObject.Find("CameraPositionMain");
                break;
            }
            currentCameraPositionType = value;
        } 
    }

    #endregion

    #region Behaviors

    // First thing called on game start
    void Awake()
    {
        galaxyManagerActive = false;
    	shotsTaken = 0;
    	fouls = 0;
		ballOn = BallColorEnum.Red;
		foul = FoulEnum.Clear;
		threshold = 0.001f;
		allBalls = new GameObject[22];
		shotBallsOnTable = new List<BallColorEnum>();
		shotBallsHit = new List<BallColorEnum>();
		shotBallsPut = new List<BallColorEnum>();
		inMenu = false;
		CurrentCameraPositionType = GameCameraController.CameraPositionType.Top;
        GameFinished = false;

        if (Instance == null) Instance = this;
        else Destroy(this);

        allBalls = GameObject.Find("PoolBalls").GetComponent<PoolBalls>().poolBalls;
        allSpots = GameObject.FindGameObjectsWithTag("Spot");

        escMenu.SetActive(false);
        message.SetActive(false);

        if (GameObject.Find("GalaxyManager") && GalaxyManager.Instance.GalaxyFullyInitialized && GalaxyManager.Instance.IsSignedIn())
        {
            galaxyManagerActive = true;
            fouls = GalaxyManager.Instance.StatsAndAchievements.GetStatInt("fouls");
            shotsTaken = GalaxyManager.Instance.StatsAndAchievements.GetStatInt("shotsTaken");
        }

    }

    void OnEnable()
    {
        ResetBalls();
        CreatePlayers();
        GalaxyManager.Instance.Matchmaking.StartLobbyManagementInGame();
        GalaxyManager.Instance.Matchmaking.SetLobbyMemberData("state", "go");
        GalaxyManager.Instance.StartNetworking();
    }

    void Update()
    {
        UiFrameUpdate();
    }

    void OnDestroy()
    {
        Time.timeScale = 1.0f;
        RemovePlayers();
        Destroy(GameManager.Instance);
    }

    #endregion

    #region Creator

    protected override void CreatePlayers()
    {
        playerList = new Player[2];
        GameObject currentObject;

        for (int i = 0; i < playerList.Length; i++)
        {
            currentObject = Instantiate(playerPrefab, GameObject.Find("WhiteBall").transform.position + offset, Quaternion.identity, GameObject.Find("Players").transform);
            playerList[i] = currentObject.AddComponent<PlayerOnline>();
            playerList[i].Score = 0;
            playerList[i].Model = currentObject;
            playerList[i].Model.name = "Player" + (i + 1);
            playerList[i].PlayerName = "Player " + (i + 1);
            playerList[i].Cue = playerList[i].Model.transform.GetChild(0).gameObject;
        }

        if (GalaxyManager.Instance.Matchmaking.LobbyOwnerID == GalaxyManager.Instance.MyGalaxyID)
        {
            me = (PlayerOnline)playerList[0];
            opponent = (PlayerOnline)playerList[1];
            me.Active = true;
            opponent.Active = false;
            PassivePlayer = opponent;
            ActivePlayer = me;
        }
        else
        {
            me = (PlayerOnline)playerList[1];
            opponent = (PlayerOnline)playerList[0];
            me.Active = false;
            opponent.Active = true;
            PassivePlayer = me;
            ActivePlayer = opponent;
        }
        PassivePlayer.PlayerState = PlayerState.Passive;
        ActivePlayer.PlayerState = PlayerState.ActiveWIH;

        me.Controller = me.gameObject.AddComponent<PlayerOnlineController>();
        opponent.Cue.SetActive(false);
        me.PlayerName = GalaxyManager.Instance.Friends.GetMyUsername();
        opponent.PlayerName = GalaxyManager.Instance.Friends.GetFriendPersonaName(GalaxyManager.Instance.Matchmaking.GetSecondPlayerID());
    }

    #endregion

    #region Methods

    protected override void ShotLong()
    {
        Debug.Log("Shot long");
        if (galaxyManagerActive)
        {
            shotsTaken++;
            GalaxyManager.Instance.StatsAndAchievements.SetStatInt("shotsTaken", shotsTaken);
        }
        CountBallsOnTable();
        shotScore = ScoreCounter();
        foul = FoulDecider();
        ballOn = BallOnDecider();
        Respot();
        GalaxyManager.Instance.Networking.SendPacketWithPlayerShot(foul, ballOn, shotScore);
        ShotDecider();
        StartCoroutine(DisplayShotStats());
        PlayingNext();
        GameEnd();
        ShotCleanUp();
    }

    public override void ShotShort()
    {
        Debug.Log("Shot short");
        CountBallsOnTable();
        ShotDecider();
        StartCoroutine(DisplayShotStats());
        PlayingNext();
        GameEnd();
        ShotCleanUp();
    }

    protected override void PlayingNext()
    {
        if (foul == FoulEnum.Clear && shotScore != 0)
        {
            ActivePlayer.PlayerState = PlayerState.ActiveClear;
        }
        else
        {
            tempPlayer = PassivePlayer;
            PassivePlayer = ActivePlayer;
            ActivePlayer = tempPlayer;

            ((PlayerOnline)PassivePlayer).Active = false;
            ((PlayerOnline)ActivePlayer).Active = true;
            
            PassivePlayer.PlayerState = PlayerState.Passive;
            
            if (foul == FoulEnum.WhiteInHole)
            {
                ActivePlayer.PlayerState = PlayerState.ActiveWIH;
            }
            else
            {
                ActivePlayer.PlayerState = PlayerState.ActiveClear;
            }
            CurrentCameraPositionType = CurrentCameraPositionType;
        }
    }

    public override void GameEnd()
    {
        if ((shotBallsOnTable.Count == 1 && shotBallsOnTable[0] == BallColorEnum.White) || shotBallsOnTable.Count == 0)
        {
            endMessage.SetActive(true);
            GameFinished = true;
            if (me.Score > opponent.Score)
            {
                endTitle.text = "You won!";
                if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement("winMPRound");
            }
            else if (me.Score < opponent.Score)
            {
                endTitle.text = opponent.PlayerName + " won!";
                if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement("loseMPRound");
            }
            else
            {
                endTitle.text = "Draw!";
                if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement("drawMPRound");
            }
            if (galaxyManagerActive)
            {
                GalaxyManager.Instance.StatsAndAchievements.SetStatFloat("highestScore", me.Score);
                GalaxyManager.Instance.StatsAndAchievements.SetStatFloat("lastScore", me.Score);
                GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("highestScore", me.Score);
                GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("fouls", GalaxyManager.Instance.StatsAndAchievements.GetStatInt("fouls"), true);
                GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("shotsTaken", GalaxyManager.Instance.StatsAndAchievements.GetStatInt("shotsTaken"), true);
            }
            endPlayer1Name.text = playerList[0].PlayerName;
            endPlayer1Score.text = playerList[0].Score.ToString();
            endPlayer2Name.text = playerList[1].PlayerName;
            endPlayer2Score.text = playerList[1].Score.ToString();
        }
    }

    #endregion

    #region UI

    protected override void UiFrameUpdate () 
	{
		activePlayerText.text = ActivePlayer.PlayerName;
		playerScoreText.text = "" + ActivePlayer.Score;
		ballOnText.text = "" + ballOn;
		thrustSlider.value = me.Controller.Thrust;
		thrustSlider.value = me.Controller.Thrust;
	}

    public void PopChatPrompt ()
    {
        chatWindow.SetActive(true);
        StartCoroutine(CloseChatPrompt());
    }

    private IEnumerator CloseChatPrompt (float secondsToClose = 2.5f)
    {
        yield return new WaitForSeconds (secondsToClose);
        if (!ChatOpen) chatWindow.SetActive(false);
    }

    #endregion

}