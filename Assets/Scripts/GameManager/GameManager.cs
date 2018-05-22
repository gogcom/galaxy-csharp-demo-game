using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	#region variables
	// Game manager
	public static GameManager Instance;

    // Galaxy Manager
    protected bool galaxyManagerActive;
    protected int shotsTaken;
    protected int fouls;

    // Enumerators
    public enum BallColorEnum 
	{
		White, 
		Red, 
		Yellow, 
		Brown, 
		Green, 
		Blue, 
		Pink, 
		Black, 
		Color 
	};
	public enum PlayerState 
	{
		ActiveClear, 
		ActiveWIH, 
		Passive, 
	};
	public enum FoulEnum 
	{
		Clear, 
		NoHit,
		HitWrong, 
		PutWrong, 
		WhiteInHole 
	};

	// Creator
	public GameObject playerPrefab;
	public Vector3 offset;

	// UI
	//	Player stats
	public Text playerScoreText;
	public Text ballOnText;
	public Text activePlayerText;
	public Slider thrustSlider;
	//	player message
	public GameObject message;
	public Text messageScore;
	public Text messageFoul;
	//	Game end message
	public GameObject endMessage;
	private bool gameFinished;
	public bool GameFinished
	{
		get
		{
			return gameFinished;
		}
		set
		{
			if (value) MouseController.ChangeMouseLockMode(CursorLockMode.None);
			gameFinished = value;
		}
	}
	public Text endTitle;
	public Text endPlayer1Name;
	public Text endPlayer1Score;
	// 	Escape menu
	public GameObject escMenu;
	// 	Help message
	public GameObject help;
	public GameObject hint;

	// Variables
	public BallColorEnum ballOn;
	public FoulEnum foul;
	public int shotScore;
	public float threshold;
	protected Player[] playerList;
	protected GameObject[] allBalls;
	protected GameObject[] allSpots;

	protected List<BallColorEnum> shotBallsOnTable;
	public List<BallColorEnum> shotBallsHit;
	public List<BallColorEnum> shotBallsPut;

	protected Player activePlayer;
	public virtual Player ActivePlayer 
	{
		get
		{
			return activePlayer;
		}
		set
		{
			value.Controller.enabled = true;
			activePlayer = value;
			Debug.Log("activePlayer set to " + value);
		}
	}
	protected Player passivePlayer;
	public virtual Player PassivePlayer
		{
		get
		{
			return passivePlayer;
		}
		set
		{
			value.Controller.enabled = false;
			passivePlayer = value;
			Debug.Log("passivePlayer set to " + value);
		}
	}
	protected Player tempPlayer;
	protected bool inMenu;
	public virtual bool InMenu
    {
        get 
		{
			return inMenu;
		}
        set
        {
            if (value) Time.timeScale = 0f;
            else Time.timeScale = 1f;
			GameManager.Instance.escMenu.SetActive(value);
            MouseController.InMenu = value;
            inMenu = value;
        }
    }
	protected GameCameraController.CameraPositionType currentCameraPositionType;
	public virtual GameCameraController.CameraPositionType CurrentCameraPositionType { 
        get
        { 
            return currentCameraPositionType; 
        } 
        set 
        {
            switch(value)
            {
                case GameCameraController.CameraPositionType.Player:
                    if (ActivePlayer.PlayerState == GameManager.PlayerState.ActiveWIH) 
                        Camera.main.GetComponent<GameCameraController>().CurrentCameraPositionObject = GameObject.Find("CameraPositionWIHGround");
                    else
                        Camera.main.GetComponent<GameCameraController>().CurrentCameraPositionObject = ActivePlayer.transform.GetChild(1).gameObject;
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

	#region Creator

	// Creates players, to be called once on game start
	// GAMETYPE SPECIFIC
	protected virtual void CreatePlayers () 
	{
		GameObject currentObject;
		for ( int i = 0; i < playerList.Length; i++ ) 
		{
			currentObject = Instantiate (playerPrefab, GameObject.Find ("WhiteBall").transform.position + offset, Quaternion.identity, GameObject.Find ("Players").transform);
			playerList[i] = currentObject.AddComponent<Player>();
			playerList[i].Controller = currentObject.AddComponent<PlayerController>();
			playerList[i].Score = 0;
			playerList[i].Model = currentObject;
			playerList[i].Model.name = "Player" + ( i + 1 );
			playerList[i].PlayerName = "Player " + ( i + 1 );
			playerList[i].Cue = playerList [i].Model.transform.GetChild (0).gameObject;
		}
	}

	protected virtual void RemovePlayers ()
	{
		foreach (Player player in playerList) 
		{
			Destroy(player.gameObject);
		}
	}

	protected virtual void ResetBalls () 
	{
		Rigidbody ballRB;
		Vector3 position;
		foreach (GameObject ball in allBalls)
		{
			ball.SetActive(true);
			ballRB = ball.GetComponent<Rigidbody>();
			position = ball.GetComponent<BallsCollisions>().MyPosition;
			ballRB.MovePosition(position);
		}
	}

	#endregion

	#region Game controller

	/* Checks if any of the active balls are in motion
	If the game runs at 60FPS this method shouldn't take longer than 5 seconds thanks to the threshold*/
	// GLOBAL
	public IEnumerator ShotStart () 
	{
		bool startedMotion = false;
		bool stoppedMotion = false;
		int i = 0;
		ActivePlayer.PlayerState = PlayerState.Passive;
		for (;;) 
		{
			if (!startedMotion) 
			{
				foreach (GameObject ball in allBalls) 
				{
					if (ball.GetComponent<Rigidbody> ().velocity.magnitude > threshold) 
					{
						startedMotion = true;
						Debug.Log("Balls started motion");
						break;
					}
				}
				if (i > 150) 
				{
					startedMotion = true;
					Debug.Log("Threshold for start reached");
				}
				i++;
			}
			if (startedMotion)
			{
				foreach (GameObject ball in allBalls) 
				{
					if (ball.GetComponent<Rigidbody> ().velocity.magnitude < threshold) 
					{
						stoppedMotion = true;
						continue;
					}
					else
					{
						stoppedMotion = false;
						break;
					}
				}
				if (i > 300) 
				{
					foreach(GameObject ball in allBalls)
					{
						ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
						ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
					}
					stoppedMotion = true;
					Debug.Log("Threshold for end reached");
				}
				i++;
			}

			if (startedMotion && stoppedMotion) 
			{
				Time.timeScale = 1.0f;
				Debug.Log("Balls stopped, shot finished");
				ShotLong();
				break;
			}

			yield return new WaitForFixedUpdate();
		}
	}

	// Will be called when balls are stopped after a shot. When playing multiplayer this will be activated only by the active player. Passive player will call ShotShort method
	// GLOBAL
	protected virtual void ShotLong () 
	{
		if (galaxyManagerActive) 
		{
			shotsTaken++;
			GalaxyManager.Instance.StatsAndAchievements.SetStatInt ("shotsTaken", shotsTaken);
		}
		CountBallsOnTable();
		shotScore = ScoreCounter();
		foul = FoulDecider ();
		ballOn = BallOnDecider ();
		Respot ();
		ShotDecider();
		StartCoroutine (DisplayShotStats ());
		PlayingNext ();
		GameEnd();
		ShotCleanUp();
	}

	// This is used only in multiplayer, when the shot finishes on the passive player side.
	// GAMETYPE SPECIFIC
	public virtual void ShotShort () {}

	// Counts the score for the current shot, to be called at the end of the shot method
	// GLOBAL
	protected void CountBallsOnTable () 
	{
		// create a list of colors of all active balls
		foreach ( GameObject ball in allBalls ) 
		{
			if ( ball.activeInHierarchy ) 
			{
				shotBallsOnTable.Add ( ball.GetComponent<BallsCollisions> ().myColor );
			}
		}
	}

	// Counts the score for the current shot, to be called at the end of the shot method
	// GLOBAL
	protected int ScoreCounter() {
		int shotScore = 0;
		foreach (BallColorEnum ball in shotBallsPut) {
			switch (ball) {
			case BallColorEnum.Red:
				shotScore += 1;
				if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement ("putRed");
				break;
			case BallColorEnum.Yellow:
				shotScore += 2;
				if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement ("putYellow");
				break;
			case BallColorEnum.Green:
				shotScore += 3;
				if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement ("putGreen");
				break;
			case BallColorEnum.Brown:
				shotScore += 4;
				if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement ("putBrown");
				break;
			case BallColorEnum.Blue:
				shotScore += 5;
				if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement ("putBlue");
				break;
			case BallColorEnum.Pink:
				shotScore += 6;
				if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement ("putPink");
				break;
			case BallColorEnum.Black:
				shotScore += 7;
				if (galaxyManagerActive) GalaxyManager.Instance.StatsAndAchievements.SetAchievement ("putBlack");
				break;
			}
		}
		return shotScore;
	}

	// To be called at the end of each shot, in shot method, decides wheter foul was committed during the shot
	// GLOBAL
	protected FoulEnum FoulDecider () 
	{
		FoulEnum foul = FoulEnum.Clear;
		// check if any ball was hit...
		if (shotBallsHit.Count == 0) 
		{
			foul = FoulEnum.NoHit;
			Debug.Log ("Foul, balls hit: No hit");
		// if not, no wrong ball was hit first
		} 
		else if (shotBallsHit[0] != ballOn && !(ballOn == BallColorEnum.Color && shotBallsHit[0] != BallColorEnum.Red)) 
		{
			foul = FoulEnum.HitWrong;
			Debug.Log ("Foul, balls hit: Hit wrong");			
		}
		// check if all balls where put according to the ballOn
		if (shotBallsPut.Count != 0) 
		{
			foreach (BallColorEnum ball in shotBallsPut) 
			{
				if (ball != ballOn && !(ballOn == BallColorEnum.Color && ball != BallColorEnum.Red)) 
				{
					foul = FoulEnum.PutWrong;
					Debug.Log ("Foul, balls put: Put wrong");						
				}
			}
		}

		// check if white ended up in hole
		if (shotBallsPut.Contains (BallColorEnum.White)) 
		{
			foul = FoulEnum.WhiteInHole;
			Debug.Log ("Foul: white in hole");
		}

		return foul;

	}

	// Decides what will be the ball on for the next shot, to be called at the end of the shot method
	// GLOBAL
	protected BallColorEnum BallOnDecider() {
		// if there are still reds on the table...
		if (shotBallsOnTable.Contains (BallColorEnum.Red)) {
			// ...and there's a foul
			if (foul != FoulEnum.Clear) {
				// return red
				return BallColorEnum.Red;
			// ...and shot was clear, and we put a ball...
			} else if (shotBallsPut.Count != 0) {
				// ...if it was red
				if (shotBallsPut [0] == BallColorEnum.Red) {
					// return color
					return BallColorEnum.Color;
				// ...if it was color
				} else {
					// return red
					return BallColorEnum.Red;
				}
			} else {
				return BallColorEnum.Red;
			}
		// if there are no reds left on the table...
		} else {
			// ...check...
			if (shotBallsOnTable.Contains (BallColorEnum.Yellow)) {
				// ...if yellow is on the table return yellow
				return BallColorEnum.Yellow;
			} else if (shotBallsOnTable.Contains (BallColorEnum.Green)) {
				// ...if green is on the table return green
				return BallColorEnum.Green;
			} else if (shotBallsOnTable.Contains (BallColorEnum.Brown)) {
				// ...if brown is on the table return brown
				return BallColorEnum.Brown;
			} else if (shotBallsOnTable.Contains (BallColorEnum.Blue)) {
				// ...if blue is on the table return blue
				return BallColorEnum.Blue;
			} else if (shotBallsOnTable.Contains (BallColorEnum.Pink)) {
				// ...if pink is on the table case return pink
				return BallColorEnum.Pink;
			} else if (shotBallsOnTable.Contains (BallColorEnum.Black)) {
				// ...if black is on the table return black
				return BallColorEnum.Black;
			} else {
				Debug.Log ("BallOnDecider couldn't resolve this case"); 
				return BallColorEnum.Red;
			}
		}
	}

	// Respot putted balls according to snooker rules, to be called at the end of each shot
	// GLOBAL
	protected void Respot () {
		if (shotBallsOnTable.Contains (BallColorEnum.Red)) {
			foreach (GameObject ball in allBalls) {
				if (ball.name != "RedBall" && !ball.activeInHierarchy) {
					switch (ball.name) {
					case "YellowBall":
						RespotSub (ball, Array.IndexOf(allSpots, GameObject.Find("YellowSpot")));
						break;
					case "BrownBall":
						RespotSub (ball, Array.IndexOf(allSpots, GameObject.Find("BrownSpot")));
						break;
					case "GreenBall":
						RespotSub (ball, Array.IndexOf(allSpots, GameObject.Find("GreenSpot")));
						break;
					case "BlueBall":
						RespotSub (ball, Array.IndexOf(allSpots, GameObject.Find("BlueSpot")));
						break;
					case "PinkBall":
						RespotSub (ball, Array.IndexOf(allSpots, GameObject.Find("PinkSpot")));
						break;
					case "BlackBall":
						RespotSub (ball, Array.IndexOf(allSpots, GameObject.Find("BlackSpot")));
						break;
					case "WhiteBall":
						break;
					default:
						Debug.Log ("Respot failed - switch case not handeled");
						return;
					}
				}
			}
		}
	}

	// Used in respot method
	// GLOBAL
	protected void RespotSub (GameObject target, int i) {
		for (int j = 0; j < allSpots.Length; j++) {
			if (allSpots [i].GetComponent<SpotTrigger> ().available) {
				target.GetComponent<Rigidbody>().velocity = Vector3.zero;
				target.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				target.transform.position = allSpots[i].transform.position;
				target.SetActive (true);
				Debug.Log ("Reset ball " + target + " at spot " + i);
				return;
			} else if (i <= allSpots.Length) {
				i++;
			} else {
				i = 0;
			}
		}
	}

	// Adds score for the current shot to the approprite player.
	// GLOBAL
	protected void ShotDecider() 
	{
		switch (foul) 
		{
			case FoulEnum.Clear:
				// add shot score to current player score
				ActivePlayer.Score += shotScore;
				// set foul displayed in the message
				messageFoul.text = "Clear";
				break;
			case FoulEnum.NoHit:
				// add shot score to opponents score
				if (playerList.Length != 1) PassivePlayer.Score += shotScore;
				// set foul displayed in the message
				messageFoul.text = "No hit";
				// add 1 to foul stat in GOG database
				if (galaxyManagerActive) {
					fouls++;
					GalaxyManager.Instance.StatsAndAchievements.SetStatInt ("fouls", fouls);
				}
				break;
			case FoulEnum.HitWrong:
				// add shot score to opponents score
				if (playerList.Length != 1) PassivePlayer.Score += shotScore;
				// set foul displayed in the message
				messageFoul.text = "Hit wrong";
				// add 1 to foul stat in GOG database
				if (galaxyManagerActive) {
					fouls++;
                    GalaxyManager.Instance.StatsAndAchievements.SetStatInt ("fouls", fouls);
				}
				break;
			case FoulEnum.PutWrong:
				// add shot score to opponents score
				if (playerList.Length != 1) PassivePlayer.Score += shotScore;
				// set foul displayed in the message
				messageFoul.text = "Put wrong";
				// add 1 to foul stat in GOG database
				if (galaxyManagerActive) {
					fouls++;
                    GalaxyManager.Instance.StatsAndAchievements.SetStatInt ("fouls", fouls);
				}
				break;
			case FoulEnum.WhiteInHole:
				// add shot score to opponents score
				if (playerList.Length != 1) PassivePlayer.Score += shotScore;
				// set foul displayed in the message
				messageFoul.text = "White in hole";
				// add 1 to foul stat in GOG database
				if (galaxyManagerActive) {
					fouls++;
                    GalaxyManager.Instance.StatsAndAchievements.SetStatInt ("fouls", fouls);
				}
				break;
		}
	}

	// Cleans up all data leftovers after a shot is finished
	// GLOBAL
	protected void ShotCleanUp () {
		// clean up
		shotBallsPut.Clear ();
		shotBallsPut.TrimExcess ();
		shotBallsHit.Clear ();
		shotBallsHit.TrimExcess ();
		shotBallsOnTable.Clear ();
		shotBallsOnTable.TrimExcess ();
		shotScore = 0;
		foul = FoulEnum.Clear;
	}

	// Decides which player plays next
	// GAMETYPE SPECIFIC
	protected virtual void PlayingNext () {}

	// Should be called at the end of each shot to check if the game is supposed to end
	// GAMETYPE SPECIFIC
	public virtual void GameEnd() {}

	#endregion

	#region UI
	
	// should be called every frame to updated data displayed on the UI
	// GAMETYPE SPECIFIC
	protected virtual void UiFrameUpdate () 
	{
		activePlayerText.text = ActivePlayer.PlayerName;
		playerScoreText.text = "" + ActivePlayer.Score;
		ballOnText.text = "" + ballOn;
		thrustSlider.value = ActivePlayer.Controller.Thrust;
		thrustSlider.value = ActivePlayer.Controller.Thrust;
	}

	// Displays message at the end of each shot, to be called on the end of each shot
	// GAMETYPE SPECIFIC
	protected virtual IEnumerator DisplayShotStats () 
	{
		// set score displayed in the message
		messageScore.text = Convert.ToString (shotScore);
		message.SetActive (true);
		yield return new WaitForSeconds (4f);
		message.SetActive (false);
	}

	#endregion

}