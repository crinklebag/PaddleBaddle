using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackUIController : MonoBehaviour {

    public enum AttackState { OFF, START, PULSE, END };
    public AttackState currentState = AttackState.OFF;
    
    [SerializeField] GameObject attackRadius;
	[SerializeField] float speed = 10;

	[Header("Size Varients")]
	[SerializeField] Vector3 smallSize;
	[SerializeField] Vector3 stretchSize;
	Vector3 startMarker = Vector3.zero;
	Vector3 endMarker = Vector3.zero;

	bool grow = false;

	// Pulse Variables
	float pulseStartTime = 0;
	float pulseJourneyDistance = 0;

	// Activate and Deactivate Variables
	float startTime = 0;
	float journeyDistance = 0;

	// Use this for initialization
	void Start () {
        // CheckState();

	}
	
	// Update is called once per frame
	void Update () {
		CheckState ();
		attackRadius.transform.rotation = Quaternion.Euler(90,0,0);
	}

    void CheckState() {
        switch (currentState) {
            case AttackState.OFF:
                break;
            case AttackState.START:
                // Set transparency to full - make bigger
				TurnOnAttackRadius();	
                break;
            case AttackState.PULSE:
				// pulse and spin attack radius
				RunAttackRadius();
                break;
            case AttackState.END:
				// Do nothing?
				TurnOffAttackRadius();	
                break; 
        }
    }
		
    public void ActivateRadius() {
		startTime = Time.time;
		startMarker = this.transform.localScale;
		endMarker = smallSize;
		journeyDistance = Vector3.Distance (startMarker, endMarker);
		currentState = AttackState.START;
    }

    public void DeactivateRadius() {
		startTime = Time.time;
		startMarker = this.transform.localScale;
		endMarker = Vector3.zero;
		journeyDistance = Vector3.Distance (startMarker, endMarker);
		currentState = AttackState.END;
	}

	void TurnOnAttackRadius(){

		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyDistance;

		attackRadius.transform.localScale = Vector3.Lerp (startMarker, endMarker, fracJourney);

		// Toggle on the Run Attack Radius When this is finished - set state to RUN
		if(fracJourney >= 1){

			startMarker = smallSize;
			endMarker = stretchSize;
			startTime = Time.time;
			journeyDistance = Vector3.Distance (startMarker, endMarker);
			grow = true;
			currentState = AttackState.PULSE;
		}
	}

    void RunAttackRadius() {

		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyDistance;

		attackRadius.transform.localScale = Vector3.Lerp (startMarker, endMarker, fracJourney);

		if (fracJourney >= 1) {
			
			// Switch to shrink
			if (grow) {
				
				startMarker = stretchSize;
				endMarker = smallSize;
				grow = false;
			} else {
				
				startMarker = smallSize;
				endMarker = stretchSize;
				grow = true;
			}

			startTime = Time.time;
			journeyDistance = Vector3.Distance (startMarker, endMarker);
		}
    }

	void TurnOffAttackRadius(){

		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyDistance;

		attackRadius.transform.localScale = Vector3.Lerp (startMarker, endMarker, fracJourney);

		// Toggle on the Run Attack Radius When this is finished - set state to RUN
		if(fracJourney >= 1){

			currentState = AttackState.OFF;
		}
	}

	public int GetState(){
		return (int)currentState;
	}
}
