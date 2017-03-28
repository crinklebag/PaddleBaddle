using Rewired;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Team
{
    public GameObject boat;
    public int score = 0;

    public Team()
    {
        // nothing here yet
    }
}

[DisallowMultipleComponent]
public class GameController : MonoBehaviour
{
    /// <summary>
    /// Array of currently active boats.
    /// </summary>
    //[SerializeField] private GameObject[] teamBoats;
    [SerializeField] private Team[] Teams = { new Team(), new Team()};
    [SerializeField] public GameObject pauseMenu;
    [HideInInspector] public float TeamOneScore;
    [HideInInspector] public float TeamTwoScore;

    /// <summary>
    /// The different winning screens for each team.
    /// </summary>
    [SerializeField] private GameObject[] teamWinBoards;

    /// <summary>
    /// In-game HUD score boards for each team.
    /// </summary>
    [SerializeField] private GameObject[] teamScoreBoards;

    /// <summary>
    /// Round begin timer text. Large and in the center.
    /// </summary>
    [SerializeField] private Text roundBeginTimerText;

    /// <summary>
    /// Round end timer text. Small and at the top.
    /// </summary>
    [SerializeField] private Text roundEndTimerText;

    /// <summary>
    /// Game end prompt.
    /// </summary>
    [SerializeField] private GameObject endPromptText;

    /// <summary>
    /// Serialized vars for spawning coins
    /// </summary>
    public GameObject coinPrefab;
    public float spawnRate = 3f;
    public GameObject respawnArea;

    /// <summary>
    /// The modes available to the controller
    /// Default mode is Flip
    /// </summary>
    [HideInInspector] public enum Modes { Flip, Pickup, Race };
    public Dictionary<Modes, GameMode> game;
    public Modes mode = Modes.Flip;

    /// <summary>
    /// Reference to the race goal
    /// </summary>
    [SerializeField]
    private Transform raceGoal;

    /// <summary>
    /// Will be true when the entire end prompt has been displayed (including the "Press 'A' to Continue" notification)
    /// </summary>
    private bool waitingForEndPrompt = false;

    /// <summary>
    /// Current team points. Whoever has the most at the end of the round wins
    /// </summary>
    private int[] teamPoints = { 0, 0 };

    /// <summary>
    /// First player component. Used to get input to restart the game.
    /// </summary>
    private Player firstPlayer;

    /// <summary>
    /// Returns if the round is currently in action. This prevents the players from moving, hazards from spawning, and time from counting down.
    /// </summary>
    public bool RoundStarted { get { return roundStarted; } }
    private bool roundStarted = false;

    /// <summary>
    /// Will be true when the round is completed.
    /// </summary>
    public bool RoundFinished { get { return roundFinished; } }
    private bool roundFinished = false;

    /// <summary>
    /// Winning team integer. -1 is no team (or tie), 0 is team 1, 1 is team 2, etc.
    /// </summary>
    public int WinningTeam { get { return winningTeam; } }
    private int winningTeam = -1;

    /// <summary>
    /// Time before round begins.
    /// </summary>
    public int TimeUntilRoundStart { get { return timeUntilRoundStart; } }
    [SerializeField] private int timeUntilRoundStart = 3;

    /// <summary>
    /// Time before round ends.
    /// </summary>
    public int TimeUntilRoundEnd { get { return timeUntilRoundEnd; } }
    [SerializeField] private int timeUntilRoundEnd = 80;

    /// <summary>
    /// Returns whether the round is in "Hurry Up" mode: adds sudden death effects and timer effects.
    /// </summary>
    public bool RoundHurryUp { get { return timeUntilRoundEnd <= hurryUpTime; } }

    /// <summary>
    /// Time at which the round is in "Hurry Up" mode.
    /// </summary>
    [SerializeField] private int hurryUpTime = 10;

    /// <summary>
    /// Used for shaking and rotating the timer slightly.
    /// </summary>
    private float roundEndTimerShake = 0.0f;

    /// <summary>
    /// How fast the timer text shakes.
    /// </summary>
    private float shakeRate = 12.0f;

    /// <summary>
    /// How big the scale variance is when the screen shakes.
    /// </summary>
    private float shakeScale = 0.034f;

    /* TEMP - I'M SORRY THIS IS TERRIBLE PLEASE FORGIVE ME*/
    [SerializeField] GameObject greenTeamCanoe;
    [SerializeField] GameObject greenTeamRaft;
    [SerializeField] GameObject redTeamCanoe;
    [SerializeField] GameObject redTeamRaft;

    /// <summary>
    /// MonoBehaviour Awake Event
    /// </summary>
    void Awake ()
    {
        SetUpTeams();    
		firstPlayer = ReInput.players.GetPlayer(0);
	}

    /* TEMP "SPAWN" functionality */
    void SetUpTeams() {
        
        // Player One Boat
        if (PlayerPrefs.GetString("teamOneBoat") == "canoe") {
            greenTeamRaft.SetActive(false);
            greenTeamCanoe.SetActive(true);
        }
        if (PlayerPrefs.GetString("teamOneBoat") == "raft") {
            greenTeamRaft.SetActive(true);
            greenTeamCanoe.SetActive(false);
        }

        // Player Two Boat
        if (PlayerPrefs.GetString("teamTwoBoat") == "canoe") {
            redTeamRaft.SetActive(false);
            redTeamCanoe.SetActive(true);
        }
        if (PlayerPrefs.GetString("teamTwoBoat") == "raft") {
            redTeamRaft.SetActive(true);
            redTeamCanoe.SetActive(false);
        }
    }

    /// <summary>
    /// MonoBehaviour Start Event
    /// </summary>
    void Start ()
    {
        GameObject[] teamBoats = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < teamBoats.Length; i++)
        {
            Teams[i].boat = teamBoats[i];
        }

        //respawnArea = GameObject.Find("Respawn Area").GetComponent<SphereCollider>();
        respawnArea = GameObject.Find("Respawn Area");
        StartCoroutine(StartRound());

        if (raceGoal == null)
            raceGoal = transform;

        game = new Dictionary<Modes, GameMode>();
        game.Add(Modes.Flip, new FlipMode());
        game.Add(Modes.Pickup, new PickupMode());
        game.Add(Modes.Race, new RaceMode());
    }

    void Update ()
    {
        // Early win condition met
        if (game[mode].winCon)
        {
            //TeamWin(game[mode].getWinner(teamBoats, raceGoal));
            TeamWin(game[mode].getWinner(Teams, raceGoal));
            AddTeamPoint(0, 0);
        }

        if (waitingForEndPrompt && firstPlayer.GetButtonDown("Attack"))
        {
            Time.timeScale = 1.0f;
			SceneManager.LoadScene ("Character Select", LoadSceneMode.Single);
		}

        if (RoundHurryUp)
        {
            roundEndTimerShake = (Time.timeSinceLevelLoad * shakeRate);
            float shakeVal = Mathf.Cos(roundEndTimerShake);

            roundEndTimerText.color = new Color(1, shakeVal, shakeVal);

            roundEndTimerText.gameObject.transform.localScale += new Vector3(shakeScale,shakeScale,0) * shakeVal;
        }
        if (firstPlayer.GetButtonDown("Select"))
        {
            Time.timeScale = 0.0f;
            pauseMenu.SetActive(true);
        }
    }

    private void endRound()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Designates that a team has won.
    /// </summary>
    /// <param name="team">The team that won.</param>
    public void TeamWin (int team)
    {
        if(!RoundFinished)
        {
            roundEndTimerText.gameObject.SetActive(false);
            Debug.Log("Win");
            roundFinished = true;
            StartCoroutine(DelayEndPromptToggle(teamWinBoards[team]));
        }
    }

	[Obsolete("Please use TeamWin(0) instead")] public void Team1Wins ()
    {
        if (!RoundFinished)
        {
            roundEndTimerText.gameObject.SetActive(false);
            Debug.Log("Win");
            roundFinished = true;
            StartCoroutine(DelayEndPromptToggle(teamWinBoards[0]));
        }
	}

	[Obsolete("Please use TeamWin(1) instead")] public void Team2Wins ()
    {
        if (!RoundFinished)
        {
            roundEndTimerText.gameObject.SetActive(false);
            Debug.Log("Win");
            roundFinished = true;
            StartCoroutine(DelayEndPromptToggle(teamWinBoards[1]));
        }
	}

    /// <summary>
    /// Adds a point to a team. The team with the most points at round end wins.
    /// </summary>
    /// <param name="team">The given team; 0 for team 1, 1 for team 2, etc.</param>
    /// <param name="points">The number of points to award.</param>
    /// 

    public void AddTeamPoint (int team, int points)
    {
        //if (team >= 0 && team < teamBoats.Length && RoundStarted && !RoundFinished)
        if (team >= 0 && team < Teams.Length && RoundStarted && !RoundFinished)
        {
            //teamPoints[team] += points;
            //TeamOneScore = teamPoints[0];
            //TeamTwoScore = teamPoints[1];
            //Text teamScoreDisplay = teamScoreBoards[team].GetComponentInChildren<Text>();
            //teamScoreDisplay.text = teamPoints[team].ToString();

            Teams[team].score += points;
            TeamOneScore = Teams[0].score;
            TeamTwoScore = Teams[1].score;
            Text teamScoreDisplay = teamScoreBoards[team].GetComponentInChildren<Text>();
            teamScoreDisplay.text = Teams[team].score.ToString();
        }
    }


    [Obsolete("Use game[mode].getWinner instead")]void RaceWin()
    {
        float minDistance = float.MaxValue;

        //for (int i = 0; i < teamBoats.Length; i++)
        for (int i = 0; i < Teams.Length; i++)
        {
            //float thisDistance = Vector3.Distance(teamBoats[i].transform.position
            float thisDistance = Vector3.Distance(Teams[i].boat.transform.position
                , raceGoal.position);

            // closest is winner
            if (thisDistance < minDistance)
            {
                minDistance = thisDistance;
                winningTeam = i;
            }
        }

        timeUntilRoundEnd = 0;
        if (teamPoints[winningTeam] == 0)
        { AddTeamPoint(winningTeam, 1); }
        StartCoroutine(DelayEndPromptToggle(teamWinBoards[winningTeam]));
    }

    IEnumerator StartRound()
    {
        roundStarted = false;
        roundBeginTimerText.gameObject.SetActive(true);

        while (timeUntilRoundStart > 0)
        {
            //
            roundBeginTimerText.text = timeUntilRoundStart.ToString() + "...";
            //
            yield return new WaitForSecondsRealtime(1);
            timeUntilRoundStart -= 1;
        }

        roundBeginTimerText.text = "Start!";
        yield return new WaitForSecondsRealtime(1);

        // Run mode specific init
        game[mode].init(this);

        roundStarted = true;
        roundBeginTimerText.gameObject.SetActive(false);
        roundEndTimerText.gameObject.SetActive(true);

        StartCoroutine(RoundSecondTick());
    }
    IEnumerator RoundSecondTick()
    {
        while (timeUntilRoundEnd >= 0 && !RoundFinished)
        {
            TimeSpan t = TimeSpan.FromSeconds(timeUntilRoundEnd);

            roundEndTimerText.text = String.Format("{0}:{1:00}", t.Minutes, t.Seconds);

            yield return new WaitForSecondsRealtime(1);
            timeUntilRoundEnd -= 1;
        }

        roundEndTimerText.gameObject.SetActive(false);
        roundBeginTimerText.gameObject.SetActive(true);
        roundBeginTimerText.text = "Finished!";
        PlayerPrefs.SetInt("teamOneScore", Teams[0].score);
        PlayerPrefs.SetInt("teamTwoScore", Teams[1].score);
        SceneManager.LoadScene("GameOver");
        yield return new WaitForSecondsRealtime(2);

        winningTeam = game[mode].getWinner(Teams, raceGoal);

        if (winningTeam == -1)
        {
            roundBeginTimerText.text = "Tie Game!";
            SceneManager.LoadScene("GameOver");
            // yield return new WaitForSecondsRealtime(3);
            // SceneManager.LoadScene("Lobby Design", LoadSceneMode.Single);
        }
        else
        {
            TeamWin(winningTeam);
            Teams[(winningTeam + 1) % Teams.Length].boat
                .SendMessage("FlipBoat");
        }
    }

	IEnumerator DelayEndPromptToggle(GameObject ToggleUI)
    {
		Time.timeScale = 0.4f;

		GameObject camera = Camera.main.gameObject;

		while (camera.transform.position.y > 35) {

            //Vector3 cameraTarget = (winningTeam == 1) ? teamBoats[0].transform.position : teamBoats[1].transform.position;
            Vector3 cameraTarget = (winningTeam == 1) ? Teams[0].boat.transform.position : Teams[1].boat.transform.position;

            camera.transform.position = Vector3.MoveTowards (camera.transform.position, cameraTarget, 0.5f);
			//camera.transform.Translate (camera.transform.forward);
			Debug.Log ("camera: " + camera.transform.position);
			yield return null;
		}

        yield return new WaitForSecondsRealtime(2);
        //ToggleUI.SetActive(true);

        //GameObject.Find("End Score Team 1").GetComponent<Text>().text = teamPoints[0] + " Points";
        //GameObject.Find("End Score Team 2").GetComponent<Text>().text = teamPoints[1] + " Points";

    yield return new WaitForSecondsRealtime(2);

		endPromptText.gameObject.SetActive (true);
		waitingForEndPrompt = true;
	}

    public void runCR(IEnumerator CR)
    {
        StartCoroutine(CR);
    }

    //public void AltWin()
    //{
    //    Debug.Log("AltWin called");
    //    game[mode].winCon = true;
    //}
}

