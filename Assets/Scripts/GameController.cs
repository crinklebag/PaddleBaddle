using Rewired;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

[DisallowMultipleComponent]
public class GameController : MonoSingleton<GameController> {

    [SerializeField] private GameObject team1Boat;
    [SerializeField] private GameObject team2Boat;

    [SerializeField] private GameObject team1WinBoard;
    [SerializeField] private GameObject team2WinBoard;

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
    private int roundEndTimer = 12;

    /// <summary>
    /// Returns whether the round is in "Hurry Up" mode: adds sudden death effects and timer effects.
    /// </summary>
    public bool RoundHurryUp { get { return roundEndTimer <= 10; } }

    /// <summary>
    /// Used for shaking and rotating the timer slightly.
    /// </summary>
    private float roundEndTimerShake = 0.0f;

    [SerializeField] private float shakeRate = 10.0f;

    [SerializeField] private float shakeScale = 0.1f;

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
    /// 
    /// </summary>
	public void Team1Wins ()
    {
        if (!RoundFinished)
        {
            roundEndTimerText.gameObject.SetActive(false);
            Debug.Log("Win");
            RoundFinished = true;
            StartCoroutine(DelayEndPromptToggle(team1WinBoard));
        }
	}

    /// <summary>
    /// 
    /// </summary>
	public void Team2Wins ()
    {
        if (!RoundFinished)
        {
            roundEndTimerText.gameObject.SetActive(false);
            Debug.Log("Win");
            RoundFinished = true;
            StartCoroutine(DelayEndPromptToggle(team2WinBoard));
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
            yield return new WaitForSeconds(1);
            roundBeginTimer -= 1;
        }

        roundBeginTimerText.text = "Start!";
        yield return new WaitForSeconds(0.5f);

        RoundStarted = true;
        roundBeginTimerText.gameObject.SetActive(false);
        roundEndTimerText.gameObject.SetActive(true);

        StartCoroutine(RoundSecondTick());
    }

    IEnumerator RoundSecondTick()
    {
        while (roundEndTimer >= 0 && !RoundFinished)
        {

            if (roundEndTimer <= 0)
            {
                RoundFinished = true;
            }

            TimeSpan t = TimeSpan.FromSeconds(roundEndTimer);

            roundEndTimerText.text = String.Format("{0}:{1:00}", t.Minutes, t.Seconds);

            yield return new WaitForSeconds(1);
            roundEndTimer -= 1;
        }
    }

	IEnumerator DelayEndPromptToggle (GameObject ToggleUI)
    {
		Time.timeScale = 0.4f;

		GameObject camera = Camera.main.gameObject;

		while (camera.transform.position.y > 35) {

			Vector3 cameraTarget = (winningTeam == 1) ? team1Boat.transform.position : team2Boat.transform.position;

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

