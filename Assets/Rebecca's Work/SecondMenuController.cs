using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondMenuController : MonoBehaviour {

    [SerializeField] GameObject Team1Player2;
    [SerializeField] GameObject Team1Paddle;
    [SerializeField] GameObject Team2Player4;
    [SerializeField] GameObject Team2Paddle;

    bool canMove = false;
    bool fourPlayers = false;

    bool teamOneSelected = false;
    bool teamTwoSelected = false;

	// Use this for initialization
	void Start () {
        // Check if 2 or 4 player game
        if (PlayerPrefs.GetInt("numPlayers") == 4) { fourPlayers = true; }
        else {
            fourPlayers = false;
            SetUpTwoPlayers();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (teamOneSelected && teamTwoSelected) {
            canMove = true;
        }
	}

    void SetUpTwoPlayers() {
        Team1Player2.SetActive(false);
        Team1Paddle.SetActive(false);
        Team2Paddle.SetActive(false);
        Team2Player4.SetActive(false);
    }

    public void TeamOneSelect(string boatSelection) {
        teamOneSelected = true;
        PlayerPrefs.SetString("teamOneBoat", boatSelection);
    }

    public void TeamOneDeselect() {
        teamOneSelected = false;
        PlayerPrefs.SetString("teamOneBoat", "");
    }

    public void TeamTwoSelect(string boatSelection) {
        teamTwoSelected = true;
        PlayerPrefs.SetString("teamOneBoat", boatSelection);
    }

    public void TeamTwoDeselect() {
        teamTwoSelected = false;
        PlayerPrefs.SetString("teamOneBoat", "");
    }

    public bool CanMove() {
        return canMove;
    }

    public bool IsFourPlayer() {
        return fourPlayers;
    }
}
