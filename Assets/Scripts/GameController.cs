using Rewired;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour {

	[SerializeField] GameObject team1Boat;
	[SerializeField] GameObject team2Boat;
	[SerializeField] GameObject endPromptText;
    [SerializeField] GameObject team1WinBoard;
    [SerializeField] GameObject team2WinBoard;
    
	Player firstPlayer;
	bool waitingForEndPrompt = false;
    bool roundFinished = false;
	int winningTeam = 0;


	void Awake () {

		firstPlayer = ReInput.players.GetPlayer (0);
	}

	void Update () {

		if (waitingForEndPrompt && firstPlayer.GetButtonDown("Attack")) {

            Time.timeScale = 1.0f;
			SceneManager.LoadScene ("Character Select",LoadSceneMode.Single);
		}
	}

	public void Team1Wins () {
        if (!roundFinished) {
            Debug.Log("Win");
            roundFinished = true;
            StartCoroutine(DelayEndPromptToggle(team1WinBoard));
        }
	}

	public void Team2Wins () {
        if (!roundFinished) {
            Debug.Log("Win");
            roundFinished = true;
            StartCoroutine(DelayEndPromptToggle(team2WinBoard));
        }
	}

	IEnumerator DelayEndPromptToggle (GameObject ToggleUI) {

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

