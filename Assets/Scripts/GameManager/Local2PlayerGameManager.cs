using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Local2PlayerGameManager : GameManager
{

    #region Variables

    public Text endPlayer2Name;
    public Text endPlayer2Score;

    #endregion

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

    protected override void CreatePlayers()
    {
        playerList = new Player[2];
        base.CreatePlayers();
        PassivePlayer = playerList[1];
        ActivePlayer = playerList[0];
        playerList[1].PlayerState = GameManager.PlayerState.Passive;
        playerList[0].PlayerState = GameManager.PlayerState.ActiveWIH;
    }

    protected override void PlayingNext()
    {
        if (foul == FoulEnum.Clear && shotBallsPut.Capacity != 0)
        {
            ActivePlayer.PlayerState = PlayerState.ActiveClear;
        }
        else
        {
            tempPlayer = PassivePlayer;
            PassivePlayer = ActivePlayer;
            ActivePlayer = tempPlayer;

            PassivePlayer.PlayerState = PlayerState.Passive;
            
            if (foul == FoulEnum.WhiteInHole)
            {
                ActivePlayer.PlayerState = PlayerState.ActiveWIH;
            }
            else {
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
            if (playerList[0].Score > playerList[1].Score)
            {
                endTitle.text = playerList[0].PlayerName + " won!";
                if (galaxyManagerActive)
                {
                    GalaxyManager.Instance.StatsAndAchievements.SetAchievement("win2PRound");
                    GalaxyManager.Instance.StatsAndAchievements.SetStatFloat("highestScore", playerList[0].Score);
                    GalaxyManager.Instance.StatsAndAchievements.SetStatFloat("lastScore", playerList[0].Score);
                    GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("highestScore", playerList[0].Score);
                }
            }
            else if (playerList[0].Score < playerList[1].Score)
            {
                endTitle.text = playerList[1].PlayerName + " won!";
                if (galaxyManagerActive)
                {
                    GalaxyManager.Instance.StatsAndAchievements.SetAchievement("win2PRound");
                    GalaxyManager.Instance.StatsAndAchievements.SetStatFloat("highestScore", playerList[1].Score);
                    GalaxyManager.Instance.StatsAndAchievements.SetStatFloat("lastScore", playerList[1].Score);
                    GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("highestScore", playerList[1].Score);
                }
            }
            else
            {
                endTitle.text = "Draw!";
                if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement("draw2PRound");
            }
            if (galaxyManagerActive) 
            {
                GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("fouls", GalaxyManager.Instance.StatsAndAchievements.GetStatInt("fouls"), true);
                GalaxyManager.Instance.Leaderboards.SetLeaderboardScore("shotsTaken", GalaxyManager.Instance.StatsAndAchievements.GetStatInt("shotsTaken"), true);
            }
            endPlayer1Name.text = playerList[0].PlayerName;
            endPlayer1Score.text = playerList[0].Score.ToString();
            endPlayer2Name.text = playerList[1].PlayerName;
            endPlayer2Score.text = playerList[1].Score.ToString();
        }
    }

}