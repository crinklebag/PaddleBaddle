using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour {

    [SerializeField] GameObject startGame;
    int playersIn = 0;

    Player[] players = new Player[4];

    // Use this for initialization
    void Start() {
        for (int i = 0; i < players.Length; i++) {
            players[i] = ReInput.players.GetPlayer(i);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if ((players[0].GetButton("Attack") || players[1].GetButton("Attack") || players[2].GetButton("Attack") || players[3].GetButton("Attack")) && startGame.activeSelf) {
            // Start Game
            Debug.Log("Start Game");
            SceneManager.LoadScene("TestScene");
        }
	}

    public void AddPlayer() {
        playersIn++;
        if (playersIn == 4) {
            // Let teh players start the game
            Debug.Log("Donezo");
            startGame.SetActive(true);
        }
    }
}
