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
    [SerializeField] GameObject[] boatBody;
    [SerializeField] GameObject boatContents;
       
    [Header("Movement Variables")]
    [SerializeField] GameObject playerCharacter;
    [SerializeField] GameObject paddlePivot;
    [SerializeField] GameObject paddle;
    [SerializeField] int playerID;
    [SerializeField] float paddleForwardForce = 500;
    [SerializeField] float paddleTorque = 250;
    [SerializeField] float paddleRoationSpeed;

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
                if (instructionsPanel.gameObject.activeSelf) { instructionsPanel.PulledTrigger(playerID); }
                if (selectedBoat == "canoe") { MoveCanoe(1, -1); }
                else { MoveCanoe(1, 1); }
            }
            else if (player.GetButtonDown("-Right Paddle") && canPaddle) {
                if (instructionsPanel.gameObject.activeSelf) { instructionsPanel.PulledBumper(playerID); }
                if (selectedBoat == "canoe") { MoveCanoe(1, 1); }
                else { MoveCanoe(1, -1); }
            }
            else if (player.GetButtonDown("+Left Paddle") && canPaddle) {
                if (instructionsPanel.gameObject.activeSelf) { instructionsPanel.PulledTrigger(playerID); }
                if (selectedBoat == "canoe") { MoveCanoe(-1, -1); }
                else { MoveCanoe(-1, 1); }
            }
            else if (player.GetButtonDown("-Left Paddle") && canPaddle) {
                if (instructionsPanel.gameObject.activeSelf) { instructionsPanel.PulledBumper(playerID); }
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
        // Disallow Selection Input
        selectingBoat = false;
        // Set the UI
        boatSelectedUI.SetActive(true);
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
    }

    void MoveCanoe(int paddleSide, int paddleDirection)
    {
        // Add force to boat by the paddle
        // Debug.Log("Adding Forward Force");
        canPaddle = false;

        Vector3 finalForwardForce = paddleDirection * paddleForwardForce * currentBoatBody.transform.forward;
        currentBoatBody.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(finalForwardForce, currentBoatBody.transform.position, ForceMode.Impulse);

        Vector3 finalHorizontalForce = -paddleSide * paddleTorque * currentBoatBody.transform.up;
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
                }
                else if (paddleDirection < 0)
                {
                    playerAnimator.SetTrigger("Paddle Backward");
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
            paddle.transform.RotateAround(paddlePivot.transform.position, paddlePivot.transform.right * (dir * -1), paddleRoationSpeed * Time.deltaTime);
            paddle.transform.localRotation = initRot;
            paddleRotationTimer += paddleRoationSpeed * Time.deltaTime;
            if (paddleRotationTimer >= 360)
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
}
