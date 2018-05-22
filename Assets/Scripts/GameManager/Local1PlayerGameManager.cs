using UnityEngine;
using System.Collections.Generic;

public class Local1PlayerGameManager : GameManager
{

    #region events

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
        // Setting GameManager instance to this
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

    // On game start
    void OnEnable()
    {
        ResetBalls();
        CreatePlayers();
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

    #region overrides

    protected override void CreatePlayers()
    {
        base.playerList = new Player[1];
        base.CreatePlayers();
        ActivePlayer = playerList[0];
        ActivePlayer.PlayerState = GameManager.PlayerState.ActiveWIH;
    }

    protected override void PlayingNext()
    {
        ActivePlayer = playerList[0];
        if (foul == FoulEnum.WhiteInHole)
        {
            ActivePlayer.PlayerState = PlayerState.ActiveWIH;
            CurrentCameraPositionType = CurrentCameraPositionType;
        }
        else 
        {
            ActivePlayer.PlayerState = PlayerState.ActiveClear;
        }
    }

    public override void GameEnd()
    {
        if ((shotBallsOnTable.Count == 1 && shotBallsOnTable[0] == BallColorEnum.White) || shotBallsOnTable.Count == 0)
        {
            endMessage.SetActive(true);
            GameFinished = true;
            endPlayer1Score.text = "" + playerList[0].Score;
            if (galaxyManagerActive)
            {
                GalaxyManager.Instance.StatsAndAchievements.SetAchievement("winSPRound");
                GalaxyManager.Instance.StatsAndAchievements.SetStatFloat("highestScore", playerList[0].Score);
                GalaxyManager.Instance.StatsAndAchievements.SetStatFloat("lastScore", playerList[0].Score);
                GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("highestScore", playerList[0].Score);
                GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("fouls", GalaxyManager.Instance.StatsAndAchievements.GetStatInt("fouls"), true);
                GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("shotsTaken", GalaxyManager.Instance.StatsAndAchievements.GetStatInt("shotsTaken"), true);
            }
        }
    }

    #endregion

}