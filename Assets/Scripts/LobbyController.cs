using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyController : MonoBehaviour {

    MainMenuCamController mmCamController;

    public enum MainMenuState { INTRO, PLAYER_COUNT, CHOOSE_PLAYER, READY, START_GAME };
    [SerializeField] MainMenuState currentState;

    [SerializeField] GameObject startGame;
	[SerializeField] string targetName = "Tutorialish";

    [Header("Intro References:")]
    [SerializeField] GameObject gameNameUI;

    [Header("Player Count References:")]
    [SerializeField] GameObject playerCountUI;
    [SerializeField] GameObject twoPlayerUI;
    [SerializeField] GameObject fourPlayerUI;
    [SerializeField] Sprite selectedBanner;
    [SerializeField] Sprite deselectedBanner;
    int selectedPlayerCount;

    [Header("Choose Player References:")]
    [SerializeField] GameObject teamOneSmall;
    [SerializeField] GameObject teamOneLarge;
    [SerializeField] GameObject teamTwoSmall;
    [SerializeField] GameObject teamTwoLarge;
    [SerializeField] PlayerSelect teamOnePlayer1;
    [SerializeField] PlayerSelect teamOnePlayer2;
    [SerializeField] PlayerSelect teamOnePlayer3;
    [SerializeField] PlayerSelect teamTwoPlayer1;
    [SerializeField] PlayerSelect teamTwoPlayer2;
    [SerializeField] PlayerSelect teamTwoPlayer3;

    int playersIn = 0;
    bool canSelectCharacter = false;
    bool canInput = true;

    Player[] players = new Player[4];

    // Use this for initialization
    void Start() {
        startGame.SetActive(false);
        for (int i = 0; i < players.Length; i++) {
            players[i] = ReInput.players.GetPlayer(i);
        }

        // Init Game
        mmCamController = Camera.main.gameObject.GetComponent<MainMenuCamController>();
        currentState = MainMenuState.INTRO;
	}
	// Update is called once per frame
	void Update () {      
        if (players[0].GetButton("Select")) {
            // Start Game
            Debug.Log("Start Game Early");
			StartGame ();
        }

        if ((playersIn == 4 && selectedPlayerCount == 4) || (playersIn == 2 && selectedPlayerCount == 2)) {
            // Let the players start the game
            Debug.Log("Can start now");
            SetUpReady();
        }

        HandleInput();
    }

    void HandleInput() {
        if (players[0].GetButtonDown("Attack") || players[1].GetButtonDown("Attack") || players[2].GetButtonDown("Attack") || players[3].GetButtonDown("Attack")) {

            HandleAPress();
            StartCoroutine("JoystickInputCountdown");
        }
        else if (players[0].GetButtonDown("Shove") || players[1].GetButtonDown("Shove") || players[2].GetButtonDown("Shove") || players[3].GetButtonDown("Shove")) {

            HandleBPress();
            StartCoroutine("JoystickInputCountdown");
        }

        if ((players[0].GetAxis("Horizontal") != 0 || players[1].GetAxis("Horizontal") != 0 || players[2].GetAxis("Horizontal") != 0 || players[3].GetAxis("Horizontal") != 0 ||
            players[0].GetAxis("Horizontal 1") != 0 || players[1].GetAxis("Horizontal 1") != 0 || players[2].GetAxis("Horizontal 1") != 0 || players[3].GetAxis("Horizontal 1") != 0) && canInput) {
            // Moved the joystick
            HandleJoystickMovement();
            canInput = false;
            StartCoroutine("JoystickInputCountdown");
        }
    }

    void HandleAPress() {

        switch (currentState) {
            case MainMenuState.INTRO:
                SetUpPlayerCount();
                ResetIntro();
                break;
            case MainMenuState.PLAYER_COUNT:
                SetUpChoosePlayer();
                ResetPlayerCount();
                break;
            case MainMenuState.READY:
                StartGame();
                break;
        }
        // Intro: Advance to player count
        // Player count: Set the numPlayers(playerPref), Advance to Choose Player
        // Choose Player: Advance to Ready
        // Ready: Start Game
    }

    void HandleBPress() {

        switch (currentState) {
            case MainMenuState.PLAYER_COUNT:
                SetUpIntro();
                ResetPlayerCount();
                break;
            case MainMenuState.CHOOSE_PLAYER:
                if ((selectedPlayerCount == 2 && !teamOnePlayer3.IsInGame() && !teamTwoPlayer3.IsInGame()) ||
                    (selectedPlayerCount == 4 && !teamOnePlayer1.IsInGame() && !teamOnePlayer2.IsInGame() && !teamTwoPlayer1.IsInGame() && !teamTwoPlayer2.IsInGame())) { 
                    SetUpPlayerCount();
                    ResetChoosePlayer();
                }
                break;
            case MainMenuState.READY:
                SetUpChoosePlayer();
                break;
        }
        // Intro: No Action.
        // Player count: Go back to intro
        // Choose Player: go back to player count
        // Ready: remove all players from the game, go back to choose player
    }

    void HandleJoystickMovement() {
        switch (currentState) {
            case MainMenuState.PLAYER_COUNT:
                // toggle between 2 and 4 players
                ToggleSelectedPlayerCount();
                break;
            case MainMenuState.CHOOSE_PLAYER:
                // Change the colours of the outfit the player is wearing
                break;
        }
    }

    void SetUpIntro() {
        currentState = MainMenuState.INTRO;
        gameNameUI.SetActive(true);
    }

    void SetUpPlayerCount() {
        currentState = MainMenuState.PLAYER_COUNT;
        playerCountUI.SetActive(true);
        selectedPlayerCount = 2;
        twoPlayerUI.GetComponent<SpriteRenderer>().sprite = selectedBanner;
        fourPlayerUI.GetComponent<SpriteRenderer>().sprite = deselectedBanner;
    }

    void SetUpChoosePlayer() {
        // Set thenumber of players chosen
        if (selectedPlayerCount == 2) {
            PlayerPrefs.SetInt("numPlayers", 2);
            selectedPlayerCount = 2;
            Debug.Log("Choose 2 players");
            teamOneSmall.SetActive(true);
            teamOneLarge.SetActive(false);
            teamTwoSmall.SetActive(true);
            teamTwoLarge.SetActive(false);
        } else {
            PlayerPrefs.SetInt("numPlayers", 4);
            selectedPlayerCount = 4;
            Debug.Log("Choose 4 players");
            teamOneSmall.SetActive(false);
            teamOneLarge.SetActive(true);
            teamTwoSmall.SetActive(false);
            teamTwoLarge.SetActive(true);
        }
        
        // Set the current Menu State
        currentState = MainMenuState.CHOOSE_PLAYER;
        mmCamController.MoveToCharacterSelect();
        canSelectCharacter = true;
    }

    void SetUpReady() {
        currentState = MainMenuState.READY;
        startGame.SetActive(true);
    }

    void ResetIntro() {
        gameNameUI.SetActive(false);
    }

    void ResetPlayerCount() {
        playerCountUI.SetActive(false);
    }

    void ResetChoosePlayer() {
        mmCamController.MoveToMainMenu();
        canSelectCharacter = false;

        // Go through any players that have readied and knock them out
        if (selectedPlayerCount == 2) {
            if (teamOnePlayer3.GetComponent<PlayerSelect>().IsInGame()) { teamOnePlayer3.GetComponent<PlayerSelect>().ExitGame(); }
            if (teamTwoPlayer3.GetComponent<PlayerSelect>().IsInGame()) { teamTwoPlayer3.GetComponent<PlayerSelect>().ExitGame(); }
        } else {
            if (teamOnePlayer1.GetComponent<PlayerSelect>().IsInGame()) { teamOnePlayer1.GetComponent<PlayerSelect>().ExitGame(); }
            if (teamTwoPlayer1.GetComponent<PlayerSelect>().IsInGame()) { teamTwoPlayer1.GetComponent<PlayerSelect>().ExitGame(); }
            if (teamOnePlayer2.GetComponent<PlayerSelect>().IsInGame()) { teamOnePlayer2.GetComponent<PlayerSelect>().ExitGame(); }
            if (teamTwoPlayer2.GetComponent<PlayerSelect>().IsInGame()) { teamTwoPlayer2.GetComponent<PlayerSelect>().ExitGame(); }
        }
    }

    void ResetReady() {
        currentState = MainMenuState.CHOOSE_PLAYER;
        startGame.SetActive(false);
    }

    void ToggleSelectedPlayerCount() {
        if (selectedPlayerCount == 2) {
            twoPlayerUI.GetComponent<SpriteRenderer>().sprite = deselectedBanner;
            fourPlayerUI.GetComponent<SpriteRenderer>().sprite = selectedBanner;
            selectedPlayerCount = 4;
        } else {
            twoPlayerUI.GetComponent<SpriteRenderer>().sprite = selectedBanner;
            fourPlayerUI.GetComponent<SpriteRenderer>().sprite = deselectedBanner;
            selectedPlayerCount = 2;
        }
    }

    public void StartGame() {
        SceneManager.LoadScene(targetName);
    }

    public void AddPlayer() {
        playersIn++;
        Debug.Log(playersIn);
    }
    public void RemovePlayer() {
        startGame.SetActive(false);
        playersIn--;
    }

    public bool CanSelectCharacter() {
        return canSelectCharacter;
    }

    public bool CanInput() {
        return canInput;
    }

    IEnumerator JoystickInputCountdown() {
        yield return new WaitForSeconds(0.5f);

        canInput = true;
    }

}
