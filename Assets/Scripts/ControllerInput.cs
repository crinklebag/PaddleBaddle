using Rewired;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerInput : MonoBehaviour {

    [Header("Player")]
    PlayerUI playerUIController;

    [Header("Parameters")]
    [SerializeField] GameObject playerCharacter;
    [SerializeField] int playerID;

    [Header("Paddle")]
    [SerializeField] GameObject paddleObject;
    [SerializeField] PaddleData paddleData;

    [Header("Powerup")]
    [SerializeField] float speedBoostForce = 150000f;
    [SerializeField] float strengthBoostForce = 200f;
    [SerializeField] float stunTime = 1f;
    [SerializeField] float mudEffect = 0.5f;
    public bool stunned = false;
    float slowMod = 1f;

    [Header("Rumble")]
    [SerializeField] float shortRumble = 0.5f;
    [SerializeField] float longRumble = 2.0f;

    //creating an selectable object.
    public GameObject attackDisplay;

    Player player;
	GameObject boat;
	Boat boatInfo;

    float paddleRotationTimer = 0;

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

	void Awake ()
    {
        // FindUI();
        player = ReInput.players.GetPlayer (playerID);
		boat = this.GetComponentInParent<Boat> ().gameObject;
		boatInfo = boat.GetComponent<Boat> ();       

		// Manually adding all of the functions to the dictionary
		powerupActions.Add ("speed", SpeedBoost);
		powerupActions.Add ("strength", StrengthBoost);
		powerupActions.Add ("", OnMissingAction);
    }

	void OnDrawGizmosSelected()
    {
        
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (GetPaddlePosition(), paddleData.reach);
	}

    void FindUI()
    {
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
	void Update()
    {
        GameObject gameControllerObject = GameObject.Find("GameController");
        //Debug.Log((gameControllerObject != null && stunned != true) + " reported from " + gameObject.name);
        if (gameControllerObject != null && stunned != true)
        {
            GameController gameController = gameControllerObject.GetComponent<GameController>();

            if (gameController != null && gameController.RoundStarted)
            {                                            
                HandleInput();
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paddleSide"></param>
    /// <param name="paddleDirection"></param>
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

        StartCoroutine(RotatePaddle());

        if (playerCharacter)
        {
            Animator playerAnimator = playerCharacter.GetComponent<Animator>();

            if (playerAnimator)
            {
                playerAnimator.SetInteger("Paddle Side", previousPaddleSide);
                if(paddleDirection > 0)
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

    /// <summary>
    /// 
    /// </summary>
    void HandleInput ()
    {
        if (canPaddle && !taunting)
        {
            if (player.GetButtonDown("+Right Paddle"))
            {
                //Go Forward
                MoveCanoe(1,1);
            }
            else if (player.GetButtonDown("-Right Paddle"))
            {
                // Go Backward
                MoveCanoe(1,-1);

            }
            else if (player.GetButtonDown("+Left Paddle"))
            {
                //Go Forward
                MoveCanoe(-1,1);
            }
            else if (player.GetButtonDown("-Left Paddle"))
            {
                // Go Backward
                MoveCanoe(-1,-1);
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

        //

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

    /// <summary>
    /// 
    /// </summary>
    void CanAttack()
    {
        // Debug.Log("Can Attack");
        attackDisplay.SetActive(false);

        Collider[] hitColliders = Physics.OverlapSphere(GetPaddlePosition(), paddleData.reach);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject != this.gameObject && hitColliders[i].GetComponent<Boat>())
            {
                attackDisplay.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// Adding force to the boat for the speed boost
    /// </summary>
    void SpeedBoost()
	{
		Debug.Log ("Adding " + speedBoostForce + " for speedboost!");
		gameObject.GetComponent<Rigidbody> ().AddForce (transform.up * speedBoostForce, ForceMode.Impulse);
		RemovePowerUp();
	}

    /// <summary>
	/// Add force for the next attack
    /// </summary>
	void StrengthBoost()
	{
        paddleData.attackForce = (paddleData.attackForce > strengthBoostForce) ? paddleData.attackForce : paddleData.attackForce + strengthBoostForce;
		RemovePowerUp();
	}

    /// <summary>
    /// Remove the powerup from the boat
    /// </summary>
    void RemovePowerUp()
	{
		boatInfo.hasPowerUp = false;
		Destroy (transform.GetChild (transform.childCount - 1).gameObject);
	}

    /// <summary>
    /// Something went wrong
    /// </summary>
    void OnMissingAction()
	{
		Debug.Log ("A powerup with an empty string got called?");
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Poop"))
        {
            StartCoroutine(Stun());
            Destroy(other.gameObject);
        } else if (other.CompareTag("Mud"))
        {
            slowMod = mudEffect; // lower the mod to the mud effect
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mud"))
            slowMod = 1f; // Just set it back to full
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Vector3 GetPaddlePosition()
    {
        Bounds paddleBounds = paddleObject.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;

        return paddleObject.transform.position - paddleObject.transform.up.normalized * paddleBounds.extents.z * 0.33f;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator Taunt()
    {
        taunting = true;
        yield return new WaitForSeconds(1f);
        taunting = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator Stun()
    {
        stunned = true;
        yield return new WaitForSeconds(stunTime);
        stunned = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator RotatePaddle()
    {
        canPaddle = false;
        yield return new WaitForSeconds(paddleData.rotationTime);
        canPaddle = true;
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
