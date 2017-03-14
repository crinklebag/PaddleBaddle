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
    [SerializeField] GameObject greenTeamBadgeUI;
    [SerializeField] GameObject redTeamBadgeUI;

    [SerializeField] Transform  winningTeamPos;
    [SerializeField] Transform losingTeamPos;
    [SerializeField] RectTransform winningUIpos;
    [SerializeField] RectTransform losingUIPos;

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
        // PlayerPrefs.SetInt("teamOneScore", 5);
        // Red Score
        // PlayerPrefs.SetInt("teamTwoScore", 5);
        CheckScores();
    }

    // Update is called once per frame
    void Update() {
        if (canInput) {
            // Player One Input
            if (playerOne.GetButtonDown("Attack")) {
                ReadyPlayer(greenTeamBadgeUI, 1);
            }
            if (playerOne.GetButtonDown("Shove")) {
                UnreadyPlayer(greenTeamBadgeUI, 1);
            }
            // Player Two Input
            if (playerTwo.GetButtonDown("Attack")) {
                ReadyPlayer(redTeamBadgeUI, 2);
            }
            if (playerTwo.GetButtonDown("Shove")) {
                UnreadyPlayer(redTeamBadgeUI, 2);
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

    void ReadyPlayer(GameObject playerUI, int playerNumber) {
        playerUI.transform.GetChild(1).GetComponent<Text>().text = "Done.";
        if (playerNumber == 1) { playerOneReady = true; }
        else { playerTwoReady = true; }
        // state = true;
    }

    void UnreadyPlayer(GameObject playerUI, int playerNumber) {
        if (playerNumber == 1) {
            playerUI.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetInt("teamOneScore").ToString();
            playerOneReady = false;
        }
        else {
            playerUI.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetInt("teamTwoScore").ToString();
            playerTwoReady = false;
        }
        // state = false;
    }

    void SetUpWin(GameObject player, GameObject playerUI) {
        Debug.Log("Setting up win");
        playerUI.GetComponent<RectTransform>().localPosition = winningUIpos.localPosition;
        playerUI.transform.GetChild(1).GetComponent<Text>().text = winningScore.ToString();
        player.transform.position = new Vector3(winningTeamPos.position.x, player.transform.position.y, winningTeamPos.position.z);
        player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, -197, player.transform.rotation.z);
        // SET THE ANIM BOOL IDIOT
        player.GetComponent<Animator>().SetTrigger("Win");
    }

    void SetUpLoss(GameObject player, GameObject playerUI) {
        Debug.Log("Setting up lose");
        playerUI.GetComponent<RectTransform>().localPosition = losingUIPos.localPosition;
        playerUI.transform.GetChild(1).GetComponent<Text>().text = losingScore.ToString();
        player.transform.position = new Vector3(losingTeamPos.position.x, player.transform.position.y, winningTeamPos.position.z);
        // SET THE ANIM BOOL IDIOT
        player.GetComponent<Animator>().SetTrigger("Lose");
    }

    void TieGame() {
        // Everything stays where it is 
        // Set the players score
        redTeamBadgeUI.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetInt("teamOneScore").ToString();
        greenTeamBadgeUI.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetInt("teamTwoScore").ToString();
        // Set the animators to win state
        greenTeam.GetComponent<Animator>().SetTrigger("Win");
        redTeam.GetComponent<Animator>().SetTrigger("Win");
    }
}
