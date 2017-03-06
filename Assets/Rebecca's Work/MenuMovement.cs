using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class MenuMovement : MonoBehaviour {

    public enum BoatType { CANOE, RAFT };
    [SerializeField] BoatType currentBoat; 

    [SerializeField] SecondMenuController menuController;

    [Header("Boat Selection Variables")]
    [SerializeField] Image[] boatUI;
    [SerializeField] GameObject[] boatBody;
    [SerializeField] GameObject boatContents;
       
    [Header("Movement Variables")]
    [SerializeField] GameObject paddlePivot;
    [SerializeField] GameObject paddle;
    [SerializeField] int playerID;
    [SerializeField] float paddleForwardForce = 500;
    [SerializeField] float paddleTorque = 250;
    [SerializeField] float paddleRoationSpeed;

    Player player;
    GameObject boat;
    Quaternion initRot;
    int dir = 0;
    float paddleRotationTimer = 0;
    bool canPaddle = true;
    bool canInput = true;
    bool inGame = true;

    // Use this for initialization
    void Awake () {
        if (playerID == 1 || playerID == 3) {
            Debug.Log("Checking");
            inGame = menuController.IsFourPlayer();
        }
        player = ReInput.players.GetPlayer(playerID);
        boat = this.gameObject;
        currentBoat = BoatType.CANOE;
        if(paddle != null) initRot = paddle.transform.localRotation;
    }
	
	// Update is called once per frame
	void Update () {
        if (menuController.CanMove())
        {
            if (player.GetButtonDown("+Right Paddle") && canPaddle)
            {
                paddlePivot.transform.localScale = new Vector3(1, 1, 1);
                //Go Forward
                dir = 1;
                MoveCanoe();
            }
            else if (player.GetButtonDown("-Right Paddle") && canPaddle)
            {
                paddlePivot.transform.localScale = new Vector3(1, 1, 1);
                // Go Backward
                dir = -1;
                MoveCanoe();

            }
            else if (player.GetButtonDown("+Left Paddle") && canPaddle)
            {
                paddlePivot.transform.localScale = new Vector3(-1, 1, 1);
                //Go Forward
                dir = 1;
                MoveCanoe();
            }
            else if (player.GetButtonDown("-Left Paddle") && canPaddle)
            {
                paddlePivot.transform.localScale = new Vector3(-1, 1, 1);
                // Go Backward
                dir = -1;
                MoveCanoe();
            }
        }
        else {
            // Swap Boats
            if ((player.GetAxis("Horizontal 1") > 0.34f || player.GetAxis("Horizontal 1") < -0.34f) && canInput) {
                // Get Next Boat
                if (currentBoat < BoatType.RAFT) {
                    currentBoat = BoatType.RAFT;
                    boatUI[(int)currentBoat].gameObject.SetActive(true);
                    boatUI[(int)currentBoat - 1].gameObject.SetActive(false);
                    boatBody[(int)currentBoat].SetActive(true);
                    boatBody[(int)currentBoat - 1].SetActive(false);
                    boatContents.transform.parent = boatBody[(int)currentBoat].transform;
                } else {
                    currentBoat = BoatType.CANOE;
                    boatUI[(int)currentBoat].gameObject.SetActive(true);
                    boatUI[(int)currentBoat + 1].gameObject.SetActive(false);
                    boatBody[(int)currentBoat].SetActive(true);
                    boatBody[(int)currentBoat + 1].SetActive(false);
                    boatContents.transform.parent = boatBody[(int)currentBoat].transform;
                }

                StartCoroutine(WaitForInput());
            }
        }

        RotatePaddle();
    }

    void MoveCanoe()
    {
        // Add force to boat by the paddle
        Debug.Log("Adding Forward Force");
        canPaddle = false;

        Vector3 finalForwardForce = dir * paddleForwardForce * boat.transform.forward;
        boat.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(finalForwardForce, boat.transform.position, ForceMode.Impulse);

        float horizontalDirection = Mathf.Sign(dir * paddle.transform.localPosition.x * paddlePivot.transform.localScale.x);
        Vector3 finalHorizontalForce = horizontalDirection * paddleTorque * boat.transform.up;
        boat.transform.GetComponentInChildren<Rigidbody>().AddTorque(finalHorizontalForce, ForceMode.Impulse);

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
}
