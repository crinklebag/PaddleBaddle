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
    [SerializeField] float shoveForce = 900;
    [SerializeField] float paddleRoationSpeed;
    [SerializeField] float attackRadius;
    [SerializeField] float stunTime = 1f;
    [SerializeField] float mudEffect = 0.5f;
    [SerializeField] float shortRumble = 0.5f;
    [SerializeField] float longRumble = 2.0f;
    float slowMod = 1f;

    //creating an selectable object.
    public GameObject attackDisplay;

    public bool stunned = false;

    Player player;
	GameObject boat;
	Boat boatInfo;

    float paddleRotationTimer = 0;

    int dir = 0;

    bool canPaddle = true;
    bool taunting = false;

	System.TimeSpan timeStartedRotation;
	List<int> quadrantsHit = new List<int>();

	// This is a dictionary that stores string keys and functions
	Dictionary<string, System.Action> powerupActions = new Dictionary<string, System.Action> ();

	void Awake () {
        // FindUI();
        player = ReInput.players.GetPlayer (playerID);
		boat = this.GetComponentInParent<Boat> ().gameObject;
		boatInfo = boat.GetComponent<Boat> ();       

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
        //Debug.Log((gameControllerObject != null && stunned != true) + " reported from " + gameObject.name);
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
                    StartCoroutine(StopTauntAnim());
                }

                float rightAxis = player.GetAxis("Horizontal 1");

                if(rightAxis > 0.34f)
                {
                    SetPaddleSide(1);
                }
                else if(rightAxis < -0.34f)
                {
                    SetPaddleSide(-1);
                }

                CheckForJoystickRotation();
                RotatePaddle();
                CanAttack();
                Attack();
                Shove();

            }
        }      
	}

    IEnumerator StopTauntAnim()
    {
        yield return new WaitForSeconds(1f);
        taunting = false;
    }

    /// <summary>
    /// Sets the current side the paddle is on.
    /// </summary>
    /// <param name="side">1 for the default side; right for the front player, left for the back player.</param>
    /// <returns>The previous side the paddle was on.</returns>
    int SetPaddleSide(int side)
    {
        int oldSide = Mathf.RoundToInt(paddlePivot.transform.localScale.x);

        paddlePivot.transform.localScale = new Vector3(side, 1, 1);

        return oldSide;
    }

    void RotatePaddle()
    {
        if (!canPaddle)
        {
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

        Vector3 finalForwardForce = dir * paddleForwardForce * boat.transform.forward * slowMod;
        boat.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(finalForwardForce, boat.transform.position, ForceMode.Impulse);

        float horizontalDirection = Mathf.Sign(dir * paddle.transform.localPosition.x * paddlePivot.transform.localScale.x);
        Vector3 finalHorizontalForce = horizontalDirection * paddleTorque * boat.transform.up;
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

            boat.transform.GetComponentInChildren<Rigidbody>().AddRelativeTorque(-paddleRotationForce * directionOfRotation * Vector3.up, ForceMode.Force);
        }
        
    }

    void CanAttack()
    {
        // Debug.Log("Can Attack");
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

    void Attack()
    {
        // Check if attacking
        if (player.GetButtonDown("Attack")) {

            Collider[] hitColliders = Physics.OverlapSphere(paddle.transform.position, attackRadius);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                Boat otherBoat = hitColliders[i].GetComponent<Boat>();
                if (hitColliders[i].gameObject != this.gameObject && otherBoat != null)
                {

                    if (otherBoat.Invincible == false)
                    {
                        Vector3 differenceVector = otherBoat.transform.position - paddle.transform.position;

                    	hitColliders[i].GetComponent<Rigidbody>().AddForceAtPosition(attackForce * Vector3.down, differenceVector, ForceMode.Impulse);
                    	Debug.Log("Attack force applied: "+ attackForce);

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

    void Shove()
    {
        if (player.GetButtonDown("Shove"))
        {

            Collider[] hitColliders = Physics.OverlapSphere(paddle.transform.position, attackRadius);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                Boat otherBoat = hitColliders[i].GetComponent<Boat>();
                if (hitColliders[i].gameObject != this.gameObject && otherBoat != null)
                {

                    if (otherBoat.Invincible == false)
                    {
                        Vector3 forceVector = otherBoat.transform.position - transform.position;
                        forceVector.y = 0.0f;
                        forceVector.Normalize();

                        hitColliders[i].GetComponent<Rigidbody>().AddForceAtPosition(shoveForce * forceVector, otherBoat.transform.position, ForceMode.Impulse);
                        Debug.Log("Shove force applied: " + shoveForce);
                        
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
        } else if (other.CompareTag("Mud"))
        {
            slowMod = mudEffect; // lower the mod to the mud effect
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mud"))
            slowMod = 1f; // Just set it back to full
    }

    IEnumerator Stunning()
    {
        stunned = true;
        yield return new WaitForSeconds(stunTime);
        stunned = false;
    }


    // Controller Rumble functions

	// Variable length full-intensity rumble function
    public IEnumerator Rumble(float duration)
    {
        foreach (Joystick j in player.controllers.Joysticks)
        {
            if (!j.supportsVibration) continue;
            j.SetVibration(1.0f, 1.0f);
        }
        yield return new WaitForSeconds(duration);
        foreach (Joystick j in player.controllers.Joysticks)
        {
            j.StopVibration();
        }
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

	// Variable direction half-second rumble function 
	public IEnumerator DirectionalRumble(float leftIntensity, float rightIntensity)
	{
		foreach (Joystick j in player.controllers.Joysticks)
		{
			if (!j.supportsVibration) continue;
			j.SetVibration(leftIntensity, rightIntensity);
		}
		yield return new WaitForSeconds(0.5f);
		foreach (Joystick j in player.controllers.Joysticks)
		{
			j.StopVibration();
		}
	}


}
