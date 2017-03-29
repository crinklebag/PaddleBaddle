using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackUIController : MonoBehaviour {

    public enum AttackState { OFF, START, PULSE, END };
    AttackState currentState = AttackState.OFF;
    
    [SerializeField] GameObject attackRadius;

    bool runAttackNotice = false;
    bool runAttackRadius = false;

    float startTime = 0;
    float journeyDistance = 0;

	// Use this for initialization
	void Start () {
        CheckState();
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            // get the other player and turn on the attack radius

            // Turn on your own Attack notice

        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {

        }
    }

    void CheckState() {
        switch (currentState) {
            case AttackState.OFF:
                // Set the transparency and size to 0
                break;
            case AttackState.START:
                // Set transparency to full - make bigger
                break;
            case AttackState.PULSE:
                break;
            case AttackState.END:
                break; 
        }
    }

    public void TurnOnAttackNotice() {

    }

    public void TurnOffAttackNotice() {

    }

    public void TunrOnAttackRadius() {
        // Grow and make visible
    }

    public void TurnOffAttackRadius() {
        // Shrink and make invisible
    }

    void RunAttackRadius() {

    }

    void RunAttackNotice() {

    }
}
