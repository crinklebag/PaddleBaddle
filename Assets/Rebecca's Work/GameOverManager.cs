using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameOverManager : MonoBehaviour {

    [SerializeField] GameObject greenTeam;
    [SerializeField] GameObject redTeam;
    [SerializeField] GameObject greenTeamBadgeUI;
    [SerializeField] GameObject redTeamBadgeUI;

    [SerializeField] Transform  winningTeamPos;
    [SerializeField] Transform losingTeamSpot;
    [SerializeField] RectTransform winningUIpos;
    [SerializeField] RectTransform losingUIPos;

    int winningScore;
    int losingScore;

	// Use this for initialization
	void Start () {
        // Green Score
        PlayerPrefs.SetInt("teamOneScore", 10);
        // Red Score
        PlayerPrefs.SetInt("teamTwoScore", 5);
        CheckScores();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CheckScores() {
        if (PlayerPrefs.GetInt("teamOneScore") > PlayerPrefs.GetInt("teamTwoScore")) {
            // Green Team Wins
            winningScore = PlayerPrefs.GetInt("teamOneScore");
            losingScore = PlayerPrefs.GetInt("teamTwoScore");
            SetUpWin(greenTeam, greenTeamBadgeUI);
            SetUpLoss(redTeam, redTeamBadgeUI);
        }
        else if (PlayerPrefs.GetInt("teamOneScore") < PlayerPrefs.GetInt("teamTwoScore")) {
            // Red team wins
            winningScore = PlayerPrefs.GetInt("teamTwoScore");
            losingScore = PlayerPrefs.GetInt("teamOneScore");
            SetUpWin(redTeam, redTeamBadgeUI);
            SetUpLoss(greenTeam, greenTeamBadgeUI);
        } else {
            // Tie Game - Everybody wins!
            TieGame();
        }
    }

    void SetUpWin(GameObject player, GameObject playerUI) {
        playerUI.GetComponent<RectTransform>().localPosition = winningUIpos.localPosition;
        playerUI.transform.GetChild(1).GetComponent<Text>().text = winningScore.ToString();
        player.transform.position = new Vector3(winningTeamPos.position.x, player.transform.position.y, winningTeamPos.position.z);
        // SET THE ANIM BOOL IDIOT
    }

    void SetUpLoss(GameObject player, GameObject playerUI) {

    }

    void TieGame() {
        // Everything stays where it is 
        // Set the players score
        // Set the animators to win state
    }
}
