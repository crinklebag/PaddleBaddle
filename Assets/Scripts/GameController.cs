using Rewired;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour
{
    /// <summary>
    /// Array of currently active boats.
    /// </summary>
    [SerializeField] private GameObject[] teamBoats;
    [SerializeField] public GameObject pauseMenu;
    public float TeamOneScore;
    public float TeamTwoScore;
    [Obsolete("Please use teamBoats[0] instead")] GameObject team1Boat { get { return teamBoats[0]; } }
    [Obsolete("Please use teamBoats[1] instead")] GameObject team2Boat { get { return teamBoats[1]; } }

    /// <summary>
    /// The different winning screens for each team.
    /// </summary>
    [SerializeField] private GameObject[] teamWinBoards;

    [Obsolete("Please use teamWinBoards[0] instead")] GameObject team1WinBoard { get { return teamWinBoards[0]; } }
    [Obsolete("Please use teamWinBoards[1] instead")] GameObject team2WinBoard { get { return teamWinBoards[1]; } }

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
    [SerializeField] GameObject coinPrefab;
    [SerializeField] float spawnRate = 3f;
    SphereCollider respawnArea;

    /// <summary>
    /// The modes available to the controller
    /// Private by default
    /// Default mode is Flip
    /// </summary>
    [HideInInspector] public enum Modes { Flip, Pickup, Race };
    public Modes mode = Modes.Flip;

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

    /// <summary>
    /// bool to tell if a team has arrived at the race goal
    /// </summary>
    [HideInInspector]
    public bool raceOver;

    /// <summary>
    /// Reference to the race goal
    /// </summary>
    [SerializeField]
    private Transform raceGoal;

    /// <summary>
    /// MonoBehaviour Awake Event
    /// </summary>
    void Awake ()
    {
		firstPlayer = ReInput.players.GetPlayer(0);
	}

    /// <summary>
    /// MonoBehaviour Start Event
    /// </summary>
    void Start ()
    {
        respawnArea = GameObject.Find("Respawn Area").GetComponent<SphereCollider>();
        StartCoroutine(StartRound());
    }

	void Update ()
    {

        if (mode == Modes.Race && raceOver)
        { RaceWin(); }

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
        if (team >= 0 && team < teamBoats.Length && RoundStarted && !RoundFinished)
        {
            teamPoints[team] += points;
            TeamOneScore = teamPoints[0];
            TeamTwoScore = teamPoints[1];
            Text teamScoreDisplay = teamScoreBoards[team].GetComponentInChildren<Text>();
            teamScoreDisplay.text = teamPoints[team].ToString();
        }
    }

    void RaceWin()
    {
        float minDistance = float.MaxValue;

        for (int i = 0; i < teamBoats.Length; i++)
        {
            float thisDistance = Vector3.Distance(teamBoats[i].transform.position
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

        // Switch to perform mode specific setup
        // Flip doesn't need anything extra
        // Neither does race, now we're using an if
        if (mode == Modes.Pickup)
        {
                StartCoroutine(SpawnCoins());
        }

        roundStarted = true;
        roundBeginTimerText.gameObject.SetActive(false);
        roundEndTimerText.gameObject.SetActive(true);

        StartCoroutine(RoundSecondTick());
    }

    IEnumerator SpawnCoins()
    {
        // For simplicity we'll use Respawn area for now
       while (!RoundFinished)
        {
            Vector3 spawnPoint = UnityEngine.Random.insideUnitSphere;
            spawnPoint.Scale(new Vector3(respawnArea.radius, 0, respawnArea.radius));
            spawnPoint += respawnArea.transform.position;
            Instantiate(coinPrefab, spawnPoint, Quaternion.identity);

            yield return new WaitForSeconds(spawnRate);
        } 
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
        SceneManager.LoadScene("Credits");
        yield return new WaitForSecondsRealtime(2);

        if (mode == Modes.Race)
        {
            RaceWin();
            yield break;
        }

        if (teamPoints[0] > teamPoints[1])
        {
            Debug.Log("Team 1 won");
            TeamWin(0);
            teamBoats[1].SendMessage("FlipBoat");
        }
        else if (teamPoints[1] > teamPoints[0])
        {
            Debug.Log("Team 2 won");
            TeamWin(1);
            teamBoats[0].SendMessage("FlipBoat");
        }
        else
        {
            roundBeginTimerText.text = "Tie Game!";
            SceneManager.LoadScene("Credits");
            yield return new WaitForSecondsRealtime(3);
            SceneManager.LoadScene("Lobby Design", LoadSceneMode.Single);
        }
    }

	IEnumerator DelayEndPromptToggle(GameObject ToggleUI)
    {
		Time.timeScale = 0.4f;

		GameObject camera = Camera.main.gameObject;

		while (camera.transform.position.y > 35) {

			Vector3 cameraTarget = (winningTeam == 1) ? teamBoats[0].transform.position : teamBoats[1].transform.position;

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
}

