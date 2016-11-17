using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Boat : MonoBehaviour {

	[SerializeField] bool isTeam1;
	[SerializeField] GameObject player1;
	[SerializeField] GameObject player2;
	[SerializeField] Transform flipCheck;

	bool isFlipped = false;


	// Use this for initialization
	void Start () {
	
	}

	void Update () {

		if (flipCheck.position.y < this.transform.position.y && !isFlipped) {

			isFlipped = true;

			// Turn off controller input
			ControllerInput[] controllers = this.GetComponentsInChildren<ControllerInput> ();
			foreach (ControllerInput controller in controllers) {

				controller.enabled = false;
			}

			// Detach players for the funnies
			if(player1) player1.transform.SetParent (null);
			if(player2) player2.transform.SetParent (null);

			// Send data to game controller
			GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			if (isTeam1) {

				gameController.Team1Wins ();
			}
			else {

				gameController.Team2Wins ();
			}
		}
	}
}
