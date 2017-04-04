using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

[DisallowMultipleComponent]
public class GameOverManager : MonoBehaviour {

    [SerializeField] LevelSelectLoadPanel loadPanel;

    [SerializeField] GameObject greenTeam;
    [SerializeField] GameObject redTeam;
    [SerializeField] GameObject greenTeamSign;
    [SerializeField] GameObject redTeamSign;

    [SerializeField] Text redTeamPointUI;
    [SerializeField] Text greenTeamPointUI;

    [SerializeField] Transform  winningTeamPos;
    [SerializeField] Transform losingTeamPos;
    [SerializeField] Transform winningUIpos;
    [SerializeField] Transform losingUIPos;

    int winningScore;
    int losingScore;

    bool playerOneReady = false;
    bool playerTwoReady = false;
    bool canInput = true;

    Player playerOne;
    Player playerTwo;

    // Use this for initialization
    void Start () {
        playerOne = ReInput.players.GetPlayer(0);
        playerTwo = ReInput.players.GetPlayer(1);

        // Green Score
        // PlayerPrefs.SetInt("teamOneScore", 10);
        // Red Score
        // PlayerPrefs.SetInt("teamTwoScore", 20);
        CheckScores();
    }

    // Update is called once per frame
    void Update() {
        if (canInput) {
            // Player One Input
            if (playerOne.GetButtonDown("Attack")) {
                ReadyPlayer(greenTeamPointUI, 1);
            }
            if (playerOne.GetButtonDown("Shove")) {
                UnreadyPlayer(greenTeamPointUI, 1);
            }
            // Player Two Input
            if (playerTwo.GetButtonDown("Attack")) {
                ReadyPlayer(redTeamPointUI, 2);
            }
            if (playerTwo.GetButtonDown("Shove")) {
                UnreadyPlayer(redTeamPointUI, 2);
            }
        }

        if (playerOneReady && playerTwoReady && canInput) {
            canInput = false;
            Debug.Log("Everybody in");
            loadPanel.StartLevelLoad("Lobby");
        }

	}

    void CheckScores() {
        if (PlayerPrefs.GetInt("teamOneScore") > PlayerPrefs.GetInt("teamTwoScore")) {
            // Green Team Wins
            winningScore = PlayerPrefs.GetInt("teamOneScore");
            losingScore = PlayerPrefs.GetInt("teamTwoScore");
            SetUpWin(greenTeam, greenTeamSign, greenTeamPointUI);
            SetUpLoss(redTeam, redTeamSign, redTeamPointUI);
        }
        else if (PlayerPrefs.GetInt("teamOneScore") < PlayerPrefs.GetInt("teamTwoScore")) {
            // Red team wins
            winningScore = PlayerPrefs.GetInt("teamTwoScore");
            losingScore = PlayerPrefs.GetInt("teamOneScore");
            SetUpWin(redTeam, redTeamSign, redTeamPointUI);
            SetUpLoss(greenTeam, greenTeamSign, greenTeamPointUI);
        } else {
            // Tie Game - Everybody wins!
            TieGame();
        }
    }

    void ReadyPlayer(Text playerUI, int playerNumber) {
        playerUI.text = "Done.";
        if (playerNumber == 1) { playerOneReady = true; }
        else { playerTwoReady = true; }
        // state = true;
    }

    void UnreadyPlayer(Text playerUI, int playerNumber) {
        if (playerNumber == 1) {
            playerUI.text = PlayerPrefs.GetInt("teamOneScore").ToString();
            playerOneReady = false;
        }
        else {
            playerUI.text = PlayerPrefs.GetInt("teamTwoScore").ToString();
            playerTwoReady = false;
        }
        // state = false;
    }

    void SetUpWin(GameObject player, GameObject playerUI, Text pointUI) {
        Debug.Log("Setting up win");
        playerUI.transform.localPosition = winningUIpos.position;
        playerUI.transform.rotation = Quaternion.Euler(0, 150, 0);
        pointUI.text = winningScore.ToString();
        player.transform.position = new Vector3(winningTeamPos.position.x, player.transform.position.y, winningTeamPos.position.z);
        player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, -197, player.transform.rotation.z);
        // SET THE ANIM BOOL IDIOT
        player.GetComponent<Animator>().SetTrigger("Win");
    }

    void SetUpLoss(GameObject player, GameObject playerUI, Text pointUI) {
        Debug.Log("Setting up lose");
        playerUI.transform.localPosition = losingUIPos.position;
        playerUI.transform.rotation = Quaternion.Euler(0, 205, 0);
        pointUI.text = losingScore.ToString();
        player.transform.position = new Vector3(losingTeamPos.position.x, player.transform.position.y, losingTeamPos.position.z);
        // SET THE ANIM BOOL IDIOT
        player.GetComponent<Animator>().SetTrigger("Lose");
    }

    void TieGame() {
        // Everything stays where it is 
        // Set the players score
        redTeamPointUI.text = PlayerPrefs.GetInt("teamOneScore").ToString();
        greenTeamPointUI.text = PlayerPrefs.GetInt("teamTwoScore").ToString();
        // Set the animators to win state
        greenTeam.GetComponent<Animator>().SetTrigger("Win");
        redTeam.GetComponent<Animator>().SetTrigger("Win");
    }
}
