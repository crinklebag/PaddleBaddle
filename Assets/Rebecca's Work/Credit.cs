using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credit : MonoBehaviour {

    CreditsController creditsController;

	// Use this for initialization
	void Start () {
        creditsController = GameObject.FindGameObjectWithTag("GameController").GetComponent<CreditsController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Finish")) {
            creditsController.IncreaseDestructionCount();
            Destroy(this.gameObject);
        }
    }
}
