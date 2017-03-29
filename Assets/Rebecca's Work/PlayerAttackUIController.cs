using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

    ATTATCH A TRIGGER TO THE BOAT, ADD THIS TO IT AND CALL THESE FUNCTIONS WHEN THE OTHER BOAT IS NEAR 
     
*/

public class PlayerAttackUIController : MonoBehaviour {

    [SerializeField] GameObject attackNotice;
    [SerializeField] GameObject attackRadius;

    bool runAttackNotice = false;
    bool runAttackRadius = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (runAttackNotice) { RunAttackNotice(); }
        if (runAttackRadius) { RunAttackRadius(); }
	}

    private void OnTriggerEnter(Collider other) {

    }

    private void OnTriggerExit(Collider other)
    {

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
