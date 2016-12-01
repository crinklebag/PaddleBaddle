using Rewired;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour
{

    [SerializeField] private GameObject[] teamBoats;

    [SerializeField] private GameObject[] teamWinBoards;

    [SerializeField] private GameObject[] teamScoreBoards;

    [SerializeField] private Text roundBeginTimerText;
    [SerializeField] private Text roundEndTimerText;

    [SerializeField] private GameObject endPromptText;

    /// <summary>
    /// First player component. Used to get input to restart the game.
    /// </summary>
    private Player firstPlayer;

    /// <summary>
    /// Will be true when the entire end prompt has been displayed (including the "Press 'A' to Continue" notification)
    /// </summary>
	private bool waitingForEndPrompt = false;
    
    /// <summary>
    /// Winning team integer. -1 is no team (or tie), 0 is team 1, 1 is team 2, etc.
    /// </summary>
	private int winningTeam = -1;

    /// <summary>
    /// Current team points. Whoever has the most at the end of the round wins
    /// </summary>
    private int[] teamPoints = { 0, 0 };

    /// <summary>
    /// Returns if the round is currently in action. This prevents the players from moving, hazards from spawning, and time from counting down.
    /// </summary>
    public bool RoundStarted { get; private set; }

    /// <summary>
    /// Will be true when the round is completed.
    /// </summary>
    public bool RoundFinished { get; private set; }

    /// <summary>
    /// Time before round begins.
    /// </summary>
    private int roundBeginTimer = 3;

    /// <summary>
    /// Time before round ends.
    /// </summary>
    public int roundEndTimer = 80;

    /// <summary>
    /// Returns whether the round is in "Hurry Up" mode: adds sudden death effects and timer effects.
    /// </summary>
    public bool RoundHurryUp { get { return roundEndTimer <= 10; } }

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
        StartCoroutine(StartRound());
    }

    /// <summary>
    /// 
    /// </summary>
	void Update ()
    {

		if (waitingForEndPrompt && firstPlayer.GetButtonDown("Attack"))
        {
            Time.timeScale = 1.0f;
			SceneManager.LoadScene ("Character Select", LoadSceneMode.Single);
		}

        if (RoundHurryUp)
        {
            roundEndTimerShake += (Time.deltaTime * shakeRate);
            float shakeVal = Mathf.Cos(roundEndTimerShake);

            roundEndTimerText.color = new Color(1, shakeVal, shakeVal);

            roundEndTimerText.gameObject.transform.localScale += new Vector3(shakeScale,shakeScale,0) * shakeVal;
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
            RoundFinished = true;
            StartCoroutine(DelayEndPromptToggle(teamWinBoards[team]));
        }
    }

	[Obsolete("Please use TeamWin(0) instead")] public void Team1Wins ()
    {
        if (!RoundFinished)
        {
            roundEndTimerText.gameObject.SetActive(false);
            Debug.Log("Win");
            RoundFinished = true;
            StartCoroutine(DelayEndPromptToggle(teamWinBoards[0]));
        }
	}

	[Obsolete("Please use TeamWin(1) instead")] public void Team2Wins ()
    {
        if (!RoundFinished)
        {
            roundEndTimerText.gameObject.SetActive(false);
            Debug.Log("Win");
            RoundFinished = true;
            StartCoroutine(DelayEndPromptToggle(teamWinBoards[1]));
        }
	}

    /// <summary>
    /// Adds a point to a team. The team with the most points at round end wins.
    /// </summary>
    /// <param name="team">The given team; 0 for team 1, 1 for team 2, etc.</param>
    /// <param name="points">The number of points to award.</param>
    public void AddTeamPoint (int team, int points)
    {
        if (team >= 0 && team < teamBoats.Length)
        {
            teamPoints[team] += points;

            Text teamScoreDisplay = teamScoreBoards[team].GetComponentInChildren<Text>();
            teamScoreDisplay.text = teamPoints[team].ToString();
        }
    }

    IEnumerator StartRound()
    {
        RoundStarted = false;
        roundBeginTimerText.gameObject.SetActive(true);

        while (roundBeginTimer > 0)
        {
            //
            roundBeginTimerText.text = roundBeginTimer.ToString() + "...";
            //
            yield return new WaitForSecondsRealtime(1);
            roundBeginTimer -= 1;
        }

        roundBeginTimerText.text = "Start!";
        yield return new WaitForSecondsRealtime(1);

        RoundStarted = true;
        roundBeginTimerText.gameObject.SetActive(false);
        roundEndTimerText.gameObject.SetActive(true);

        StartCoroutine(RoundSecondTick());
    }

    IEnumerator RoundSecondTick()
    {
        while (roundEndTimer >= 0 && !RoundFinished)
        {
            TimeSpan t = TimeSpan.FromSeconds(roundEndTimer);

            roundEndTimerText.text = String.Format("{0}:{1:00}", t.Minutes, t.Seconds);

            yield return new WaitForSecondsRealtime(1);
            roundEndTimer -= 1;
        }

        roundEndTimerText.gameObject.SetActive(false);
        roundBeginTimerText.gameObject.SetActive(true);
        roundBeginTimerText.text = "Finished!";

        yield return new WaitForSecondsRealtime(2);

        roundBeginTimerText.gameObject.SetActive(false);

        if (teamPoints[0] > teamPoints[1])
        {
            Debug.Log("Team 1 won");
            TeamWin(0);
        }
        else if (teamPoints[1] > teamPoints[0])
        {
            Debug.Log("Team 2 won");
            TeamWin(1);
        }
        else
        {
            roundBeginTimerText.text = "Tie Game!";
            yield return new WaitForSecondsRealtime(3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
    }

	IEnumerator DelayEndPromptToggle (GameObject ToggleUI)
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

        yield return new WaitForSeconds(0.5f);

        ToggleUI.SetActive(true);

		yield return new WaitForSeconds (1);

		endPromptText.gameObject.SetActive (true);
		waitingForEndPrompt = true;
	}
}

