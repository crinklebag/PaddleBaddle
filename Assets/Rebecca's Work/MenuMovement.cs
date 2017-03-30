using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class MenuMovement : MonoBehaviour {

    public enum BoatType { CANOE, RAFT };
    [SerializeField] BoatType currentBoat;

    public enum TeamNumber { TeamOne, TeamTwo, TeamThree, TeamFour };
    [SerializeField] TeamNumber teamNumber;

    [SerializeField] SecondMenuController menuController;
    [SerializeField] InstructionPanel instructionsPanel;

    [Header("Boat Selection Variables")]
    [SerializeField] Image[] boatUI;
    [SerializeField] GameObject boatSelectedUI;
    [SerializeField] GameObject pressAUI;
    [SerializeField] GameObject pressBUI;
    [SerializeField] GameObject[] boatBody;
    [SerializeField] GameObject boatContents;
       
    [Header("Movement Variables")]
    [SerializeField] GameObject playerCharacter;
    [SerializeField] GameObject paddlePivot;
    [SerializeField] GameObject paddle;
    [SerializeField] int playerID;
    [SerializeField] PaddleData paddleData;
    float paddleForwardForce = 400;
    float paddleTorque = 200;
    float paddleRoationSpeed = 2400;

	[Header("Effects (Audio and Particles)")]
	[SerializeField] AudioSource splash;
	[SerializeField] AudioSource boatHit;
	[SerializeField] ParticleSystem splashForwardParticles;
	[SerializeField] ParticleSystem splashBackwardParticles;
	[SerializeField] float splashBackDelay = 0.5f;
	[SerializeField] float splashForwardDelay = 0.25f;

    Player player;
    GameObject boat;
    GameObject currentBoatBody;
    Quaternion initRot;
    string selectedBoat = "canoe";
    int dir = 0;
    int previousPaddleSide;
    int previousPaddleDirection;
    float paddleRotationTimer = 0;
    bool canPaddle = true;
    bool canInput = true;
    bool selectingBoat = true;

    // Use this for initialization
    void Awake () {
        player = ReInput.players.GetPlayer(playerID);
        boat = this.gameObject;
        currentBoat = BoatType.CANOE;
        currentBoatBody = boatBody[(int)currentBoat];
        if(paddle != null) initRot = paddle.transform.localRotation;
    }
	
	// Update is called once per frame
	void Update () {
        if (menuController.CanMove())
        {
            if (player.GetButtonDown("+Right Paddle") && canPaddle) {
                // Debug.Log("+Right Paddle");
                if (selectedBoat == "canoe") { MoveCanoe(1, -1); }
                else { MoveCanoe(1, 1); }
            }
            else if (player.GetButtonDown("-Right Paddle") && canPaddle) {
                if (selectedBoat == "canoe") { MoveCanoe(1, 1); }
                else { MoveCanoe(1, -1); }
            }
            else if (player.GetButtonDown("+Left Paddle") && canPaddle) {
                if (selectedBoat == "canoe") { MoveCanoe(-1, -1); }
                else { MoveCanoe(-1, 1); }
            }
            else if (player.GetButtonDown("-Left Paddle") && canPaddle) {
                if (selectedBoat == "canoe") { MoveCanoe(-1, 1); }
                else { MoveCanoe(-1, -1); }
            }
            else {
                // Don't move
                previousPaddleDirection = 0;
            }
        }
        else {
            // Swap Boats
            if ((player.GetAxis("Horizontal") > 0.34f || player.GetAxis("Horizontal") < -0.34f) && canInput && selectingBoat) {
                // Get Next Boat
                if (currentBoat < BoatType.RAFT) {
                    currentBoat = BoatType.RAFT;
                    boatUI[(int)currentBoat].gameObject.SetActive(true);
                    boatUI[(int)currentBoat - 1].gameObject.SetActive(false);
                    boatBody[(int)currentBoat].SetActive(true);
                    boatBody[(int)currentBoat - 1].SetActive(false);
                    boatContents.transform.parent = boatBody[(int)currentBoat].transform;
                    currentBoatBody = boatBody[(int)currentBoat];
                    selectedBoat = "raft";
                    // Set the forces for raft
                } else {
                    currentBoat = BoatType.CANOE;
                    boatUI[(int)currentBoat].gameObject.SetActive(true);
                    boatUI[(int)currentBoat + 1].gameObject.SetActive(false);
                    boatBody[(int)currentBoat].SetActive(true);
                    boatBody[(int)currentBoat + 1].SetActive(false);
                    boatContents.transform.parent = boatBody[(int)currentBoat].transform;
                    currentBoatBody = boatBody[(int)currentBoat];
                    selectedBoat = "canoe";
                }

                StartCoroutine(WaitForInput());
            }
            // 'A' press
            if (player.GetButtonDown("Attack")) {
                SelectBoat();
            }
            // 'B' press
            if (player.GetButtonDown("Shove")) {
                DeselectBoat();
            }
        }

        RotatePaddle();
    }
		
    void SelectBoat() {
        // Select Boat
        if (teamNumber == TeamNumber.TeamOne) {
            menuController.TeamOneSelect(selectedBoat);
        }
        else {
            menuController.TeamTwoSelect(selectedBoat);
        }

        if (selectedBoat == "canoe") {
            paddleForwardForce = 400;
            paddleTorque = 200;
        } else {
            paddleForwardForce = 600;
            paddleTorque = 150;
        }

        // Disallow Selection Input
        selectingBoat = false;
        // Set the UI
        boatSelectedUI.SetActive(true);
        pressBUI.SetActive(true);
        pressAUI.SetActive(false);
    }

    void DeselectBoat() {
        if (teamNumber == TeamNumber.TeamOne) {
            menuController.TeamOneDeselect();
        }
        else {
            menuController.TeamTwoDeselect();
        }
        selectingBoat = true;
        // Reset the UI
        boatSelectedUI.SetActive(false);
        pressBUI.SetActive(false);
        pressAUI.SetActive(true);
    }

    void MoveCanoe(int paddleSide, int paddleDirection)
    {
        // Add force to boat by the paddle
        // Debug.Log("Adding Forward Force");
        canPaddle = false;

        Vector3 finalForwardForce = paddleDirection * paddleData.forwardForce * currentBoatBody.transform.forward;
        currentBoatBody.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(finalForwardForce, currentBoatBody.transform.position, ForceMode.Impulse);

        Vector3 finalHorizontalForce = -paddleSide * paddleData.torque * currentBoatBody.transform.up;
        currentBoatBody.transform.GetComponentInChildren<Rigidbody>().AddTorque(finalHorizontalForce, ForceMode.Impulse);

        previousPaddleSide = paddleSide;
        previousPaddleDirection = paddleDirection;

        if (playerCharacter)
        {
            // Debug.Log("Found Player");
            Animator playerAnimator = playerCharacter.GetComponent<Animator>();

            if (playerAnimator)
            {
                playerAnimator.SetInteger("Paddle Side", previousPaddleSide);
                if (paddleDirection > 0)
                {
                    playerAnimator.SetTrigger("Paddle Forward");
					// Play Sound Effect
					splash.Play();
					// Play Splash Effect
					StartCoroutine(PlaySplash(splashBackwardParticles, splashBackDelay));

                }
                else if (paddleDirection < 0)
                {
                    playerAnimator.SetTrigger("Paddle Backward");
					// Play Sound Effect
					splash.Play();
					// Play Splash Effect
					StartCoroutine(PlaySplash(splashForwardParticles, splashForwardDelay));
                }
            }
        }
    }

    void SetPaddleSide(int side)
    {
        previousPaddleSide = side;

        previousPaddleSide = side;

        if (playerCharacter)
        {
            Animator animator = playerCharacter.GetComponent<Animator>();
            if (animator)
            {
                animator.SetInteger("Paddle Side", side);
            }
        }
    }

    void RotatePaddle()
    {
        if (!canPaddle)
        {
            paddleRotationTimer += Time.deltaTime;
            if (paddleRotationTimer >= paddleData.rotationTime)
            {
                canPaddle = true;
                paddleRotationTimer = 0;
            }
        }
    }

    IEnumerator WaitForInput() {

        canInput = false;

        yield return new WaitForSeconds(0.3f);

        canInput = true;

    }

    public int GetPlayerID() {
        return playerID;
    }

	IEnumerator PlaySplash(ParticleSystem splash, float delay){

		yield return new WaitForSeconds (delay);
		splash.Play ();
	}
		
	public void RumbleControllers(){
		StartCoroutine (Bump (0.1f));
	}

	// Variable length low-intensity bump function
	public IEnumerator Bump(float duration)
	{
		foreach (Joystick j in player.controllers.Joysticks)
		{
			if (!j.supportsVibration) continue;
			j.SetVibration(0.25f, 0.25f);
		}
		yield return new WaitForSeconds(duration);
		foreach (Joystick j in player.controllers.Joysticks)
		{
			j.StopVibration();
		}
	}

}
