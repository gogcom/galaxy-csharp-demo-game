using UnityEngine;
using Galaxy.Api;
using System.Collections.Generic;

public class GalaxyManager : MonoBehaviour
{
    #region Variables

    // Project specific data (ClientID and ClientSecret can be obtained from devportal.gog.com's game page)
    // ClientID and ClientSecret are merged into initParams for later use with Init(InitParams initParams) method
    private readonly string clientID = "50225266424144145";
    private readonly string clientSecret = "45955f1104f99b625a5733fa1848479b43d63bdb98f0929e37c9affaf900e99f";

    // Declaration of GalaxyManager static Instance so that it is easily available from anywhere in the code
    public static GalaxyManager Instance;
    // Declaration of Galaxy features classes for easier access
    public StatsAndAchievements StatsAndAchievements;
    public Leaderboards Leaderboards;
    public Friends Friends;
    public Networking Networking;
    public Matchmaking Matchmaking;
    public Invitations Invitations;
    public Storage Storage;
    private static GalaxyID myGalaxyID;
    public GalaxyID MyGalaxyID { get { return myGalaxyID; } }
    // Galaxy initiation result
    private bool galaxyFullyInitialized;
    public bool GalaxyFullyInitialized { get { return galaxyFullyInitialized; } }

    // List of all in-game achievements api keys
    public string[] achievementsList = new string[14] {
        "launchTheGame",
        "putBlack",
        "putBlue",
        "putBrown",
        "putGreen",
        "putPink",
        "putRed",
        "putYellow",
        "winSPRound",
        "win2PRound",
        "draw2PRound",
        "winMPRound",
        "drawMPRound",
        "loseMPRound"
    };

    // Dictionary of all in-game statistics api keys, with their full display names
    public Dictionary<string, string> statisticsFloatList = new Dictionary<string, string>() 
    {
        { "lastScore", "Last Score" },
        { "highestScore", "Highest Score"}
    };

    // Dictionary of all in-game statistics api keys with their full display names
    public Dictionary<string, string> statisticsIntList = new Dictionary<string, string> 
    {
        { "fouls", "Fouls" },
        { "shotsTaken", "Shots Taken" }
    };
    
    public Dictionary<string, string> leaderboardNames = new Dictionary<string, string>()
    {
        {"Fouls", "fouls"},
        {"Highest Score", "highestScore"},
        {"Shots Taken", "shotsTaken"}
    };

    //  Authentication listener
    public AuthenticationListener authListener;
    public GogServicesConnectionStateListener gogServicesConnectionStateListener;

    #endregion

    #region Behaviors

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(this);
            return;
        }
    }

    void OnEnable()
    {
        Init();
        ListenersInit();
        SignIn();
    }

    void Update()
    {
        /* Makes the GalaxyPeer process data. 
        NOTE: This is required for any listener to work, and has little overhead. */
        GalaxyInstance.ProcessData();
    }

    void OnDisable()
    {
        ShutdownAllFeatureClasses();
        ListenersDispose();
    }

    void OnDestroy()
    {
        /* Shuts down the working instance of GalaxyPeer. 
        NOTE: Shutdown should be the last method called, and all listeners should be closed before that. */
        GalaxyInstance.Shutdown(true);
        Instance = null;
        Destroy(this);
    }

    #endregion

    #region Listeners methods

    private void ListenersInit()
    {
        if (authListener == null) authListener = new AuthenticationListener();
        if (gogServicesConnectionStateListener == null) gogServicesConnectionStateListener = new GogServicesConnectionStateListener();
    }

    void ListenersDispose()
    {
        if (authListener != null) authListener.Dispose();
        if (gogServicesConnectionStateListener != null) gogServicesConnectionStateListener.Dispose();
    }

    #endregion

    #region Methods

    /* Following methods are used to start and shutdown each and every GalaxyManager feature class separately.
    Note: Each class closes its own listeners whyn disabled */
    public void StartStatsAndAchievements()
    {
        if (StatsAndAchievements == null) StatsAndAchievements = gameObject.AddComponent<StatsAndAchievements>();
    }

    public void ShutdownStatsAndAchievements()
    {
        if (StatsAndAchievements != null) Destroy(StatsAndAchievements);
    }

    public void StartFriends()
    {
        if (Friends == null) Friends = gameObject.AddComponent<Friends>();
    }

    public void ShutdownFriends()
    {
        if (Friends != null) Destroy(Friends);
    }

    public void StartLeaderboards()
    {
        if (Leaderboards == null) Leaderboards = gameObject.AddComponent<Leaderboards>();
    }

    public void ShutdownLeaderboards()
    {
        if (Leaderboards != null) Destroy(Leaderboards);
    }

    public void StartInvitations()
    {
        if (Invitations == null) Invitations = gameObject.AddComponent<Invitations>();
    }

    public void ShutdownInvitations()
    {
        if (Invitations != null) Destroy(Invitations);
    }

    public void StartMatchmaking()
    {
        if (Matchmaking == null) Matchmaking = gameObject.AddComponent<Matchmaking>();
    }

    public void ShutdownMatchmaking()
    {
        if (Matchmaking != null) Destroy(Matchmaking);
    }

    public void StartNetworking()
    {
        if (Networking == null) Networking = gameObject.AddComponent<Networking>();
    }

    public void ShutdownNetworking()
    {
        if (Networking != null) Destroy(Networking);
    }

    public void StartStorage()
    {
        if (Storage == null) Storage = gameObject.AddComponent<Storage>();
    }

    public void ShutdownStorage()
    {
        if (Storage != null) Destroy(Storage);
    }

    public void ShutdownAllFeatureClasses()
    {
        ShutdownStatsAndAchievements();
        ShutdownLeaderboards();
        ShutdownFriends();
        ShutdownNetworking();
        ShutdownInvitations();
        ShutdownMatchmaking();
        ShutdownStorage();
    }

    /* Initializes GalaxyPeer instance
    NOTE: Even if Init will throw exepction Apps interface will still be available. 
    If you want to use Apps interface in your game make sure that shutdown is NOT called when Init throws an exception.*/
    private void Init()
    {
        InitParams initParams = new InitParams(clientID, clientSecret);
        Debug.Log("Initializing GalaxyPeer instance...");
        try
        {
            GalaxyInstance.Init(initParams);
            galaxyFullyInitialized = true;
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Init failed for reason " + e);
            galaxyFullyInitialized = false;
        }
    }

    /* Signs the current user in to Galaxy services
    NOTE: This call is asynchronus. Sign in result is received by AuthListener. */
    public void SignIn()
    {
        Debug.Log("Signing user in...");
        try
        {
            GalaxyInstance.User().SignInGalaxy();
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("SignIn failed for reason " + e);
        }
    }

    /* Signs the current user out from Galaxy services */
    public void SignOut()
    {
        Debug.Log("Singing user out...");
        try
        {
            GalaxyInstance.User().SignOut();
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("SignOut failed for reason " + e);
        }
    }

    /* Checks current user singed in status
    NOTE: Signed in means that the user is signed in to GOG Galaxy client (he does not have to have working internet connection). */
    public bool IsSignedIn(bool silent = false)
    {
        bool signedIn = false;
        if (!silent) Debug.Log("Checking SignedIn status...");
        try
        {
            signedIn = GalaxyInstance.User().SignedIn();
            if (!silent) Debug.Log("User SignedIn: " + signedIn);
        }
        catch (GalaxyInstance.Error e)
        {
            if (!silent) Debug.LogWarning("Could not check user signed in status for reason " + e);
        }
        return signedIn;

    }

    /* Checks if user is logged on
    NOTE: Logged on means that the user is signed in to GOG Galaxy client and he does have working internet connection */
    public bool IsLoggedOn(bool silent = false)
    {
        bool isLoggedOn = false;

        if (!silent) Debug.Log("Checking LoggedOn status...");

        try
        {
            isLoggedOn = GalaxyInstance.User().IsLoggedOn();
            if (!silent) Debug.Log("User logged on: " + isLoggedOn);
        }
        catch (GalaxyInstance.Error e)
        {
            if (!silent) Debug.LogWarning("Could not check user logged on status for reason " + e);
        }

        return isLoggedOn;

    }

    // Checks if DLC specified by productID is installed on users machine
    public bool IsDlcInstalled(ulong productID)
    {
        bool isDlcInstalled = false;

        Debug.Log("Checking is DLC " + productID + " installed");

        try
        {
            isDlcInstalled = GalaxyInstance.Apps().IsDlcInstalled(productID);
            Debug.Log("DLC " + productID + " installed " + isDlcInstalled);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not check is DLC " + productID + " installed for reason " + e);
        }

        return isDlcInstalled;

    }

    public string GetCurrentGameLanguage ()
    {
        string gameLanguage = null;
        Debug.Log("Checking current game language");
        try
        {
            gameLanguage = GalaxyInstance.Apps().GetCurrentGameLanguage();
            Debug.Log("Current game language is " + gameLanguage);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.Log("Could not check current game language for reason " + e);
        }
        return gameLanguage;
    }

    public void ShowOverlayWithWebPage (string url)
    {
        Debug.Log("Opening overlay with web page " + url);
        try
        {
            GalaxyInstance.Utils().ShowOverlayWithWebPage(url);
            Debug.Log("Opened overlay with web page " + url);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.Log("Could not open overlay with web page " + url + " for reason " + e);
        }
    }

    #endregion

    #region Listeners

    /* Informs about the event of authenticating user
    Callback for method:
    SignIn() */
    public class AuthenticationListener : GlobalAuthListener
    {
        public override void OnAuthSuccess()
        {
            Debug.Log("Successfully signed in");

            myGalaxyID = GalaxyInstance.User().GetGalaxyID();

            GalaxyManager.Instance.StartStatsAndAchievements();
            GalaxyManager.Instance.StartFriends();

            if (GalaxyManager.Instance.IsLoggedOn())
            {
                GalaxyManager.Instance.StartLeaderboards();
                GalaxyManager.Instance.StartInvitations();
                GalaxyManager.Instance.StartStorage();
            }

        }

        public override void OnAuthFailure(FailureReason failureReason)
        {
            Debug.LogWarning("Failed to sign in for reason " + failureReason);
            if (GameObject.Find("MainMenu")!= null) GameObject.Find("MainMenu").GetComponent<MainMenuController>().GalaxyCheck();
        }

        public override void OnAuthLost()
        {
            Debug.LogWarning("Authorization lost");
            if (GameObject.Find("MainMenu") != null) GameObject.Find("MainMenu").GetComponent<MainMenuController>().GalaxyCheck();
        }

    }

    public class GogServicesConnectionStateListener : GlobalGogServicesConnectionStateListener
    {
        public override void OnConnectionStateChange(GogServicesConnectionState connected)
        {
            Debug.Log("Connection state to GOG services changed to " + connected);
        }
    }

    #endregion

}
