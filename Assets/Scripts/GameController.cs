using Rewired;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour {

	[SerializeField] GameObject team1Boat;
	[SerializeField] GameObject team2Boat;
	[SerializeField] Text winnerText;
	[SerializeField] Text endPromptText;

	Player firstPlayer;
	bool waitingForEndPrompt = false;
	int winningTeam = 0;


	void Awake () {

		firstPlayer = ReInput.players.GetPlayer (0);
	}

	void Update () {

		if (waitingForEndPrompt && firstPlayer.GetButtonDown("Attack")) {
			
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		}
	}

	public void Team1Wins () {

		winnerText.gameObject.SetActive (true);
		winningTeam = 1;
		winnerText.text = "Team 1 Wins!";
		StartCoroutine (DelayEndPromptToggle ());
	}

	public void Team2Wins () {

		winnerText.gameObject.SetActive (true);
		winningTeam = 2;
		winnerText.text = "Team 2 Wins!";
		StartCoroutine (DelayEndPromptToggle ());
	}

	IEnumerator DelayEndPromptToggle () {

		Time.timeScale = 0.4f;

		GameObject camera = Camera.main.gameObject;

		while (camera.transform.position.y > 20) {

			Vector3 cameraTarget = (winningTeam == 1) ? team1Boat.transform.position : team2Boat.transform.position;

			camera.transform.position = Vector3.MoveTowards (camera.transform.position, cameraTarget, 0.5f);
			//camera.transform.Translate (camera.transform.forward);
			Debug.Log ("camera: " + camera.transform.position);
			yield return null;
		}

		yield return new WaitForSeconds (1);

		endPromptText.gameObject.SetActive (true);
		waitingForEndPrompt = true;
	}
}

