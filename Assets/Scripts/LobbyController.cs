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
        startGame.SetActive(false);
        for (int i = 0; i < players.Length; i++) {
            players[i] = ReInput.players.GetPlayer(i);
        }
	}
	// Update is called once per frame
	void Update () {      
        if ((players[0].GetButton("Select")))
        {
            // Start Game
            Debug.Log("Start Game Early");
            SceneManager.LoadScene("Tutorialish");
        }
        if (playersIn >= 4)
        {
            // Let teh players start the game
            Debug.Log("Can start now");
            startGame.SetActive(true);

            if (players[0].GetButton("Select"))
            {
                startGame.SetActive(true);
            }
        }
    }
    public void AddPlayer() {
        playersIn++;
        Debug.Log(playersIn);
    }
    public void RemovePlayer()
    {
        startGame.SetActive(false);
        playersIn--;
    }
}
