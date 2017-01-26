using Rewired;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerInput : MonoBehaviour {

    enum PaddleSide { Left, Right };

    [Header("Player UI Reference")]
    PlayerUI playerUIController;

    [Header("Parameters")]
    [SerializeField] int playerID;
    [SerializeField] GameObject paddle;
    [SerializeField] GameObject paddlePivot;
    [SerializeField] float maxDeltaAngle = 30;
    [SerializeField] float paddleRotationForce = 10000;
    [SerializeField] float paddleTorque = 250;
    [SerializeField] float paddleForwardForce = 500;
    [SerializeField] float speedBoostForce = 30000;
    [SerializeField] float strengthBoostForce = 40;
    [SerializeField] float attackForce = 15;
    [SerializeField] float paddleRoationSpeed;
    [SerializeField] float attackRadius;
    [SerializeField] float stunTime = 1f;

    //creating an selectable object.
    public GameObject attackDisplay;

    public bool stunned = false;

    Player player;
	GameObject boat;
	Boat boatInfo;
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
	// This is a dictionary that stores string keys and functions
	Dictionary<string, System.Action> powerupActions = new Dictionary<string, System.Action> ();

    Vector3 startPosPaddle;
    Vector3 startRotPaddle;
    Vector3 startAnimPaddlePos;
    Quaternion startAnimPaddleRot;

	void Awake () {
        // FindUI();
        player = ReInput.players.GetPlayer (playerID);
		boat = this.GetComponentInParent<Boat> ().gameObject;
		boatInfo = boat.GetComponent<Boat> ();
        initRot = paddle.transform.localRotation;
        
        // Initialize Paddle Variables
        startPosPaddle = paddle.transform.localPosition;
        paddleAnimator = paddle.transform.GetComponentInParent<Animator>();
        startAnimPaddlePos = paddle.transform.GetComponentInParent<Transform>().localPosition;
        startAnimPaddleRot = paddle.transform.GetComponentInParent<Transform>().localRotation;

		// Manually adding all of the functions to the dictionary
		powerupActions.Add ("speed", speedBoost);
		powerupActions.Add ("strength", strengthBoost);
		powerupActions.Add ("", missingAction);
    }

	void OnDrawGizmosSelected() {
        
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (paddle.transform.position, attackRadius);
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
        GameObject gameControllerObject = GameObject.Find("GameController");
        Debug.Log((gameControllerObject != null && stunned != true) + " reported from " + gameObject.name);
        if (gameControllerObject != null && stunned != true)
        {
            GameController gameController = gameControllerObject.GetComponent<GameController>();

            if (gameController != null && gameController.RoundStarted)
            {                                            
                if (canPaddle && !taunting)
                {
                    if (player.GetButtonDown("+Right Paddle"))
                    {
                        paddlePivot.transform.localScale = new Vector3(1, 1, 1);
                        //Go Forward
                        dir = 1;
                        MoveCanoe();
                    }
                    else if (player.GetButtonDown("-Right Paddle"))
                    {
                        paddlePivot.transform.localScale = new Vector3(1, 1, 1);
                        // Go Backward
                        dir = -1;
                        MoveCanoe();

                    }
                    else if (player.GetButtonDown("+Left Paddle"))
                    {
                        paddlePivot.transform.localScale = new Vector3(-1, 1, 1);
                        //Go Forward
                        dir = 1;
                        MoveCanoe();
                    }
                    else if (player.GetButtonDown("-Left Paddle"))
                    {
                        paddlePivot.transform.localScale = new Vector3(-1, 1, 1);
                        // Go Backward
                        dir = -1;
                        MoveCanoe();
                    }
                }
                                
		        // Hit the powerup button && the boat has a powerup active
		        if (player.GetButtonDown ("Powerup") && boatInfo.hasPowerUp) 
		        {
		        	powerupActions [boatInfo.powerUpType] (); // call the function that matches the string the boat has
		        }
                                
                if (player.GetButtonDown("Taunt") && !taunting)
                {
                    taunting = true;
                    paddleAnimator.SetBool("taunting", true);
                    StartCoroutine(StopTauntAnim());
                }

                CheckForJoystickRotation();
                RotatePaddle();
                CanAttack();
                Attack();

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

    void MoveCanoe() {
        // Add force to boat by the paddle
        Debug.Log("Adding Forward Force");
        canPaddle = false;

        Vector3 finalForwardForce = dir * paddleForwardForce * boat.transform.up;
        boat.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(finalForwardForce, boat.transform.position, ForceMode.Impulse);


        float horizontalDirection = Mathf.Sign(dir * paddle.transform.localPosition.x * paddlePivot.transform.localScale.x);
        Vector3 finalHorizontalForce = horizontalDirection * paddleTorque * boat.transform.forward;
        boat.transform.GetComponentInChildren<Rigidbody>().AddTorque(finalHorizontalForce, ForceMode.Impulse);

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

    void CanAttack()
    {

        attackDisplay.SetActive(false);

        Collider[] hitColliders = Physics.OverlapSphere(paddle.transform.position, attackRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject != this.gameObject && hitColliders[i].GetComponent<Boat>())
            {
                attackDisplay.SetActive(true);
            }
        }
    }

    void Attack() {
        // Check if attacking
        if (player.GetButtonDown("Attack")) {

            Collider[] hitColliders = Physics.OverlapSphere(paddle.transform.position, attackRadius);

            for (int i = 0; i < hitColliders.Length; i++) {

                if (hitColliders[i].gameObject != this.gameObject && hitColliders[i].GetComponent<Boat>()) {

                    if (hitColliders[i].GetComponent<Boat>().Invincible == false)
                    {
                        Vector3 differenceVector = hitColliders[i].transform.position - paddle.transform.position;

                   	hitColliders[i].GetComponent<Rigidbody>().AddForceAtPosition(attackForce * Vector3.down, differenceVector, ForceMode.Impulse);
                   	Debug.Log("Force applied: "+ attackForce);

			// Removing strength powerup effect if we just used the strong attack
			if (attackForce > strengthBoostForce) 
			{
				attackForce -= strengthBoostForce;
			}
                    }
                }
            }
        }
    }

	// Adding force to the boat for the speed boost
	void speedBoost()
	{
		Debug.Log ("Adding " + speedBoostForce + " for speedboost!");
		gameObject.GetComponent<Rigidbody> ().AddForce (transform.up * speedBoostForce, ForceMode.Impulse);
		removePowerUp ();
	}

	// Add force for the next attack
	void strengthBoost()
	{
		attackForce = (attackForce > strengthBoostForce) ? attackForce : attackForce + strengthBoostForce;
		removePowerUp ();
	}

	// Remove the powerup from the boat
	void removePowerUp()
	{
		boatInfo.hasPowerUp = false;
		Destroy (transform.GetChild (transform.childCount - 1).gameObject);
	}

	// Something went wrong
	void missingAction()
	{
		Debug.Log ("A powerup with an empty string got called?");
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Poop"))
        {
            StartCoroutine(Stunning());
            Destroy(other.gameObject);
        }
    }

    IEnumerator Stunning()
    {
        stunned = true;
        yield return new WaitForSeconds(stunTime);
        stunned = false;
    }
}
