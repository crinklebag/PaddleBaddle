using Rewired;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerInput : MonoBehaviour {

    [Header("Player UI Reference")]
    PlayerUI playerUIController;

	[Header("Parameters")]
	[SerializeField] int playerID;
	[SerializeField] GameObject paddle;
    [SerializeField] Transform pivot;
	[SerializeField] float maxDeltaAngle = 30;
	[SerializeField] float paddleRotationForce = 10000;
    [SerializeField] float paddleForwardForce;
	[SerializeField] float attackForce = 15;
    [SerializeField] float paddleRoationSpeed;

	Player player;
	GameObject boat;
    Animator paddleAnimator;
	float angleTraversed = 0;
	float previousAngle = -1;
    float paddleRotationTimer = 0;
    float lastPaddleAngle = 0;
    int dir = 0;
    bool canPaddle = true;
    bool taunting = false;
	System.TimeSpan timeStartedRotation;
	List<int> quadrantsHit = new List<int>();
    Quaternion initRot;

    Vector3 startPosPaddle;
    Vector3 startRotPaddle;
    Vector3 startAnimPaddlePos;
    Quaternion startAnimPaddleRot;

	void Awake () {
        // FindUI();
        player = ReInput.players.GetPlayer (playerID);
		boat = this.GetComponentInParent<Boat> ().gameObject;
        initRot = paddle.transform.localRotation;
        
        // Initialize Paddle Variables
        startPosPaddle = paddle.transform.localPosition;
        paddleAnimator = paddle.transform.GetComponentInParent<Animator>();
        startAnimPaddlePos = paddle.transform.GetComponentInParent<Transform>().localPosition;
        startAnimPaddleRot = paddle.transform.GetComponentInParent<Transform>().localRotation;
    }

	void OnDrawGizmosSelected() {
        
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (paddle.transform.position, 1);
	}

    void FindUI() {
        switch (playerID) {
            case 0:
                playerUIController = GameObject.FindGameObjectWithTag("Player 1 UI").GetComponent<PlayerUI>();
                Debug.Log("Hii " + playerUIController);
                break;
            case 1:
                playerUIController = GameObject.FindGameObjectWithTag("Player 2 UI").GetComponent<PlayerUI>();
                break;
            case 2:
                playerUIController = GameObject.FindGameObjectWithTag("Player 3 UI").GetComponent<PlayerUI>();
                break;
            case 3:
                playerUIController = GameObject.FindGameObjectWithTag("Player 4 UI").GetComponent<PlayerUI>();
                break;
        }
    }

	// Update is called once per frame
	void Update () {

        if (player.GetButtonDown("Increase Speed") && canPaddle && !taunting) {
            //Go Forward
            dir = 1;
            MoveCanoe();
        }

        if (player.GetButtonDown("Decrease Speed") && canPaddle && !taunting) {
            // Go Backward
            dir = -1;
            MoveCanoe();
        }

        if (player.GetButtonDown("Taunt") && !taunting) {
            taunting = true;
            paddleAnimator.SetBool("taunting", true);
            StartCoroutine(StopTauntAnim());
        }

        CheckForJoystickRotation();
        RotatePaddle();
        Attack();

        /// REMEMBER TO REMOVE THIS AND REPLACE WITH MORE ROBUST VERSION AFTER EPM!!!!
        /// 
        if (player.GetButtonDown("Attack"))
        {
            GameObject endPrompt = GameObject.Find("End Prompt");
            if (endPrompt != null)
            {
                endPrompt.SendMessage("PlayAgain", null, SendMessageOptions.DontRequireReceiver);
            }
        }
	}

    IEnumerator StopTauntAnim() {
        yield return new WaitForSeconds(1f);
        taunting = false;
        paddleAnimator.SetBool("taunting", false);
        paddle.transform.GetComponentInParent<Transform>().localPosition = startAnimPaddlePos;
        paddle.transform.GetComponentInParent<Transform>().localRotation = startAnimPaddleRot;
    }

    void RotatePaddle() {
        if (!canPaddle) {
            paddle.transform.RotateAround(pivot.position, pivot.transform.right * (dir * -1), paddleRoationSpeed * Time.deltaTime);
            paddle.transform.localRotation = initRot;
            paddleRotationTimer += paddleRoationSpeed * Time.deltaTime;
            if (paddleRotationTimer >= 360) {
                canPaddle = true;
                paddleRotationTimer = 0;
            }
        }
    }

    void MoveCanoe() {
        // Add force to boat by the paddle
        Debug.Log("Adding Forward Force");
        canPaddle = false;
        float horizontalDirection = Mathf.Sign(paddle.transform.localPosition.x);
        Vector3 forceVector = (dir * paddleForwardForce * boat.transform.up - .85f * horizontalDirection * boat.transform.right);
        boat.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(forceVector, boat.transform.position, ForceMode.Impulse);
       
    }

    void CheckForJoystickRotation () {

        float verticalValue = player.GetAxis("Vertical");
        float horizontalValue = player.GetAxis("Horizontal");
        float angle = Mathf.Atan2(verticalValue, horizontalValue) * Mathf.Rad2Deg;
        if (angle < 0) { angle += 360; }

        // Debug.Log("Angle" + angle);

        // Find what quadrant has been hit and apply a force accordingly
        if (angle >= 0 && angle < 90) {
            if (!quadrantsHit.Contains(1)) {
                quadrantsHit.Add(1);
            }
        } else if (angle < 180) {
            if (!quadrantsHit.Contains(2)) {
                quadrantsHit.Add(2);
            }
        } else if (angle < 270) {
            if (!quadrantsHit.Contains(3)) {
                quadrantsHit.Add(3);
            }
        } else if (angle < 360) {
            if (!quadrantsHit.Contains(4)) {
                quadrantsHit.Add(4);
            }
        }

        if (quadrantsHit.Contains(1) && quadrantsHit.Contains(2) &&
               quadrantsHit.Contains(3) && quadrantsHit.Contains(4))
        {

            float directionOfRotation = 0;

            int increasing = 0;
            int decreasing = 0;
            // Check if rotated clockwise or counter clockwise
            for(int i = 0; i < quadrantsHit.Count - 1; i++)
            {
                if(quadrantsHit[i] > quadrantsHit[i+1])
                {
                    decreasing++;
                }
                else
                {
                    increasing++;
                }
            }

            if(increasing > decreasing)
            {
                directionOfRotation = 1;
            }
            else
            {
                directionOfRotation = -1;
            }

            Debug.Log("Direction of Rotation: " + directionOfRotation);

            // Reset quandrant tracker
            quadrantsHit = new List<int>();

            boat.transform.GetComponentInChildren<Rigidbody>().AddRelativeTorque(-paddleRotationForce * directionOfRotation * Vector3.forward, ForceMode.Force);
        }

        lastPaddleAngle = angle;
    }

    void Attack() {
        // Check if attacking
        if (player.GetButtonDown("Attack")) {

            Collider[] hitColliders = Physics.OverlapSphere(paddle.transform.position, 5);

            for (int i = 0; i < hitColliders.Length; i++) {

                if (hitColliders[i].gameObject != this.gameObject && hitColliders[i].GetComponent<Boat>()) {

                    Vector3 differenceVector = hitColliders[i].transform.position - paddle.transform.position;

                    hitColliders[i].GetComponent<Rigidbody>().AddForceAtPosition(attackForce * Vector3.down, differenceVector, ForceMode.Impulse);
                    Debug.Log("Force applied");
                }
            }
        }
    }
}
