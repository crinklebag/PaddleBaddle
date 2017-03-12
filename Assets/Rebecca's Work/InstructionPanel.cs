using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPanel : MonoBehaviour {

    [SerializeField] GameObject triggerPrompts;
    [SerializeField] GameObject bumperPrompts;

    bool playerOneTrigger = false;
    bool playerTwoTrigger = false;
    bool playerOneBumper = false;
    bool playerTwoBumper = false;

    // Use this for initialization
    void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

    void CheckForNextInstrction() {
        if (playerOneTrigger && playerTwoTrigger) {
            // Fade these in and out
            bumperPrompts.SetActive(true);
            triggerPrompts.SetActive(false);
        }
        if (playerOneBumper && playerTwoBumper) {
            bumperPrompts.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }

    // Call from player - check if this object is active or not first
    public void PulledTrigger(int playerID) {
        switch (playerID) {
            case 0:
                playerOneTrigger = true;
                break;
            case 1:
                playerTwoTrigger = true;
                break;
        }
        CheckForNextInstrction();
    }

    public void PulledBumper(int playerID) {
        if (playerOneTrigger && playerTwoTrigger) {
            switch (playerID)
            {
                case 0:
                    playerOneBumper = true;
                    break;
                case 1:
                    playerTwoBumper = true;
                    break;
            }
            CheckForNextInstrction();
        }
    }
}
