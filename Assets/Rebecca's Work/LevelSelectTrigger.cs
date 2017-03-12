using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectTrigger : MonoBehaviour {

    SecondMenuController menuController;

    [SerializeField] string levelToLoad;

	// Use this for initialization
	void Start () {
        menuController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SecondMenuController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Found Player");
            // Tell the menu controller to add a readied player and which level 
            int playerID = other.GetComponentInParent<MenuMovement>().GetPlayerID();
            menuController.LoadLevel(playerID, levelToLoad);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            int playerID = other.GetComponentInParent<MenuMovement>().GetPlayerID();
            menuController.ClearPlayerChoice(playerID);
        }
    }
}
