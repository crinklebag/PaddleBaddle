using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;

public class CreditSceneLooper : MonoBehaviour {
    [SerializeField] int playerID;

    Player player;

    // Use this for initialization
    void Awake () {
        player = ReInput.players.GetPlayer(playerID);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (player.GetButtonDown("Attack"))
        {
            Debug.Log("FOOP");
            SceneManager.LoadScene("Lobby Design");
        }
	}
}
