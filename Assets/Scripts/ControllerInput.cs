using Rewired;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerInput : MonoBehaviour {

    [Header("Player UI Reference")]
    PlayerUI playerUIController;

    [Header("Parameters")]
    [SerializeField] GameObject playerCharacter;
    [SerializeField] int playerID;
    [SerializeField] GameObject paddle;
    [SerializeField] float maxDeltaAngle = 30f;
    [SerializeField] PaddleData paddleData;
    [SerializeField] float speedBoostForce = 150000f;
    [SerializeField] float strengthBoostForce = 200f;
    [SerializeField] float stunTime = 1f;
    [SerializeField] float mudEffect = 0.5f;
    [SerializeField] float shortRumble = 0.5f;
    [SerializeField] float longRumble = 2.0f;
    [SerializeField] bool raft;
    float slowMod = 1f;

	[Header("Effects (Audio and Particles)")]
	[SerializeField] AudioSource splash;
	[SerializeField] AudioSource boatHit;
	[SerializeField] ParticleSystem splashForwardParticles;
	[SerializeField] ParticleSystem splashBackwardParticles;
	[SerializeField] float splashBackDelay = 0.5f;
	[SerializeField] float splashForwardDelay = 0.25f;

    //creating an selectable object.
    [SerializeField] GameObject attackDisplay;

    public bool stunned = false;

    Player player;
	GameObject boat;
	Boat boatInfo;

    float paddleRotationTimer = 0;

    // reference to the last player found within reach
    GameObject foundPlayer = null;

    /// <summary>
    /// Represents the left and right facing of the paddle. -1 for left, 1 for right.
    /// </summary>
    int previousPaddleSide;

    /// <summary>
    /// Represents the current moving direction of the paddle. -1 for backwards, 0 for stationary, 1 for forwards.
    /// </summary>
    int previousPaddleDirection;

    bool canPaddle = true;
    bool taunting = false;

	System.TimeSpan timeStartedRotation;
	List<int> quadrantsHit = new List<int>();

	// This is a dictionary that stores string keys and functions
	Dictionary<string, System.Action> powerupActions = new Dictionary<string, System.Action> ();

	void Awake () {
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
		Gizmos.DrawWireSphere (GetPaddlePosition(), paddleData.reach);
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
                HandleInput();
                RotatePaddle();
                CanAttack();
                Attack();
                Shove();

                if (playerCharacter)
                {
                    Animator playerAnimator = playerCharacter.GetComponent<Animator>();
                    if (playerAnimator)
                    {
                        playerAnimator.SetBool("Stunned", stunned || taunting);
                    }
                }                
            }
        }      
	}

    IEnumerator Taunt()
    {
        taunting = true;
        yield return new WaitForSeconds(1f);
        taunting = false;
    }

    /// <summary>
    /// Sets the current side the paddle is on.
    /// </summary>
    /// <param name="side">1 for the default side; right for the front player, left for the back player.</param>
    /// <returns>The previous side the paddle was on.</returns>
    void SetPaddleSide(int side)
    {
        previousPaddleSide = side;

        previousPaddleSide = side;

        if(playerCharacter)
        {
            Animator animator = playerCharacter.GetComponent<Animator>();
            if(animator)
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

    void MoveCanoe(int paddleSide, int paddleDirection)
    {
        // Add force to boat by the paddle
        Debug.Log("Adding Forward Force");
        canPaddle = false;

        Vector3 finalForwardForce = paddleDirection * paddleData.forwardForce * boat.transform.forward * slowMod;
        boat.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(finalForwardForce, boat.transform.position, ForceMode.Impulse);
        
        Vector3 finalHorizontalForce = -paddleSide * paddleData.torque * boat.transform.up;
        boat.transform.GetComponentInChildren<Rigidbody>().AddTorque(finalHorizontalForce, ForceMode.Impulse);

        previousPaddleSide = paddleSide;
        previousPaddleDirection = paddleDirection;

        if (playerCharacter)
        {
            Animator playerAnimator = playerCharacter.GetComponent<Animator>();

            if (playerAnimator)
            {
                playerAnimator.SetInteger("Paddle Side", previousPaddleSide);
                if(paddleDirection > 0)
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

    void HandleInput ()
    {
        if (canPaddle && !taunting)
        {
            if (player.GetButtonDown("+Right Paddle"))
            {
                //Go Forward
                if (raft) { MoveCanoe(1, 1); }
                else { MoveCanoe(1,-1); }
            }
            else if (player.GetButtonDown("-Right Paddle"))
            {
                // Go Backward
                if (raft) { MoveCanoe(1, -1); }
                else { MoveCanoe(1, 1); }

            }
            else if (player.GetButtonDown("+Left Paddle"))
            {
                //Go Forward
                if (raft) { MoveCanoe(-1, 1); }
                else { MoveCanoe(-1, -1); }
            }
            else if (player.GetButtonDown("-Left Paddle"))
            {
                // Go Backward
                if (raft) { MoveCanoe(-1, -1); }
                else { MoveCanoe(-1, 1); }
            }
            else
            {
                // Don't move
                previousPaddleDirection = 0;
            }
        }

        // Hit the powerup button && the boat has a powerup active
        if (player.GetButtonDown("Powerup") && boatInfo.hasPowerUp)
        {
            powerupActions[boatInfo.powerUpType](); // call the function that matches the string the boat has
        }

        if (player.GetButtonDown("Taunt") && !taunting)
        {
            StartCoroutine(Taunt());
        }

        float rightStickHorizontal = player.GetAxis("Horizontal 1");

        if (rightStickHorizontal > 0.34f)
        {
            SetPaddleSide(1);
        }
        else if (rightStickHorizontal < -0.34f)
        {
            SetPaddleSide(-1);
        }
        
        float leftStickVertical = player.GetAxis("Vertical");
        float leftStickHorizontal = player.GetAxis("Horizontal");

        float angle = Mathf.Atan2(leftStickVertical, leftStickHorizontal) * Mathf.Rad2Deg;
        if (angle < 0) { angle += 360; }
        
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

        //
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

            boat.transform.GetComponentInChildren<Rigidbody>().AddRelativeTorque(-paddleData.rotationForce * directionOfRotation * Vector3.up, ForceMode.Force);
        }
        
    }

    void CanAttack()
    {
        attackDisplay.SetActive(false);

        Collider[] hitColliders = Physics.OverlapSphere(GetPaddlePosition(), paddleData.reach);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject != this.gameObject && hitColliders[i].GetComponent<Boat>())
            {
                attackDisplay.SetActive(true);
                foundPlayer = hitColliders[i].gameObject;
                Debug.Log("Can Attack");
            }
        }

        // Check now to see if the attack notice is on or off - if it is on turn on the other players attack radius
		if (attackDisplay.activeSelf) {
			// If the current state is end(3) or off(0), activate
			if(foundPlayer != null && (foundPlayer.GetComponent<PlayerAttackUIController>().GetState() == 0 || foundPlayer.GetComponent<PlayerAttackUIController>().GetState() == 3)){
				foundPlayer.GetComponent<PlayerAttackUIController> ().ActivateRadius ();
				Debug.Log ("Activate The Attack UI");
			}
        }
        else {
			// If the current state is start(1) or pulse(2), end it
			if(foundPlayer != null && (foundPlayer.GetComponent<PlayerAttackUIController>().GetState() == 1 || foundPlayer.GetComponent<PlayerAttackUIController>().GetState() == 2)){
				foundPlayer.GetComponent<PlayerAttackUIController> ().DeactivateRadius ();
				Debug.Log ("Deactivate The Attack UI");
			}
        }

    }

    void Attack()
    {
        // Check if attacking
        if (player.GetButtonDown("Attack")) {

            Collider[] hitColliders = Physics.OverlapSphere(GetPaddlePosition(), paddleData.reach);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                Boat otherBoat = hitColliders[i].GetComponent<Boat>();
                if (hitColliders[i].gameObject != this.gameObject && otherBoat != null)
                {

                    if (otherBoat.invincible == false)
                    {
                        playerCharacter.GetComponent<Animator>().SetTrigger("Attacking");

                        Vector3 differenceVector = otherBoat.transform.position - GetPaddlePosition();

                    	hitColliders[i].GetComponent<Rigidbody>().AddForceAtPosition(paddleData.attackForce * Vector3.down, differenceVector, ForceMode.Impulse);
                    	Debug.Log("Attack force applied: "+ paddleData.attackForce);

			            // Removing strength powerup effect if we just used the strong attack
			            if (paddleData.attackForce > strengthBoostForce) 
			            {
                            paddleData.attackForce -= strengthBoostForce;
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

            Collider[] hitColliders = Physics.OverlapSphere(GetPaddlePosition(), paddleData.reach);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                Boat otherBoat = hitColliders[i].GetComponent<Boat>();
                if (hitColliders[i].gameObject != this.gameObject && otherBoat != null)
                {

                    if (otherBoat.invincible == false)
                    {
                        Vector3 forceVector = otherBoat.transform.position - transform.position;
                        forceVector.y = 0.0f;
                        forceVector.Normalize();

                        hitColliders[i].GetComponent<Rigidbody>().AddForceAtPosition(paddleData.shoveForce * forceVector, otherBoat.transform.position, ForceMode.Impulse);
                        Debug.Log("Shove force applied: " + paddleData.shoveForce);
                        
                    }
                }
            }
        }
    }

	// Adding force to the boat for the speed boost
	void speedBoost()
	{
		Debug.Log ("Adding " + speedBoostForce + " for speedboost!");
		gameObject.GetComponent<Rigidbody> ().AddForce (-transform.forward * speedBoostForce, ForceMode.Impulse);
		removePowerUp ();
	}

	// Add force for the next attack
	void strengthBoost()
	{
        paddleData.attackForce = (paddleData.attackForce > strengthBoostForce) ? paddleData.attackForce : paddleData.attackForce + strengthBoostForce;
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

    Vector3 GetPaddlePosition()
    {
        Bounds paddleBounds = paddle.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;

        return paddle.transform.position - paddle.transform.up.normalized * paddleBounds.extents.z * 0.33f;
    }

	IEnumerator PlaySplash(ParticleSystem splash, float delay){

		yield return new WaitForSeconds (delay);
		splash.Play ();
	}

}
