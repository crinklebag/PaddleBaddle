using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour {

    MainMenuCamController mmCamController;

    public enum MainMenuState { INTRO, PLAYER_COUNT, CHOOSE_PLAYER, READY, START_GAME };
    [SerializeField] MainMenuState currentState;

    [SerializeField] GameObject startGame;

    [Header("Intro References:")]
    [SerializeField] GameObject gameNameUI;

    [Header("Player Count References:")]
    [SerializeField] GameObject playerCountUI;
    [SerializeField] GameObject twoPlayerUI;
    [SerializeField] GameObject fourPlayerUI;
    [SerializeField] Sprite selectedBanner;
    [SerializeField] Sprite deselectedBanner;
    GameObject selectedPlayerCount;

    [Header("Choose Player References:")]
    [SerializeField] GameObject team1Player1;
    [SerializeField] GameObject team1Player2;
    [SerializeField] GameObject team2Player1;
    [SerializeField] GameObject team2Player2;

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
            SceneManager.LoadScene("Tutorialish");
        }

        if ((playersIn >= 4 && PlayerPrefs.GetInt("numPlayers") >= 4) || (playersIn >= 2 && PlayerPrefs.GetInt("numPlayers") >= 2))
        {
            // Let the players start the game
            Debug.Log("Can start now");
            startGame.SetActive(true);

            if ((players[0].GetButton("Attack") || players[1].GetButton("Attack") || players[2].GetButton("Attack") || players[3].GetButton("Attack"))) {
                startGame.SetActive(true);
            }
        }

        HandleInput();
    }

    void HandleInput() {
        if (players[0].GetButtonDown("Attack") || players[1].GetButtonDown("Attack") || players[2].GetButtonDown("Attack") || players[3].GetButtonDown("Attack")) {

            HandleAPress();
        }
        else if (players[0].GetButtonDown("Shove") || players[1].GetButtonDown("Shove") || players[2].GetButtonDown("Shove") || players[3].GetButtonDown("Shove")) {

            HandleBPress();
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
            case MainMenuState.CHOOSE_PLAYER:
                SetUpReady();
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
                SetUpPlayerCount();
                ResetChoosePlayer();
                break;
            case MainMenuState.READY:
                SetUpChoosePlayer();
                ResetReady();
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
        selectedPlayerCount = twoPlayerUI;
        twoPlayerUI.GetComponent<SpriteRenderer>().sprite = selectedBanner;
        fourPlayerUI.GetComponent<SpriteRenderer>().sprite = deselectedBanner;
    }

    void SetUpChoosePlayer() {
        // Set thenumber of players chosen
        if (selectedPlayerCount == twoPlayerUI) {
            PlayerPrefs.SetInt("numPlayers", 2);
            Debug.Log("Choose 2 players");
            team1Player2.SetActive(true);
            team2Player1.SetActive(true);
            team1Player1.SetActive(false);
            team2Player2.SetActive(false);
        } else {
            PlayerPrefs.SetInt("numPlayers", 4);
            Debug.Log("Choose 4 players");
            team1Player1.SetActive(true);
            team2Player1.SetActive(true);
            team1Player2.SetActive(true);
            team2Player2.SetActive(true);
        }
        
        // Set the current Menu State
        currentState = MainMenuState.CHOOSE_PLAYER;
        mmCamController.MoveToCharacterSelect();
        canSelectCharacter = true;
    }

    void SetUpReady() {
        currentState = MainMenuState.READY;
    }

    void ResetIntro() {
        gameNameUI.SetActive(false);
    }

    void ResetPlayerCount() {
        selectedPlayerCount = twoPlayerUI;
        twoPlayerUI.GetComponent<SpriteRenderer>().sprite = deselectedBanner;
        fourPlayerUI.GetComponent<SpriteRenderer>().sprite = selectedBanner;
        playerCountUI.SetActive(false);
    }

    void ResetChoosePlayer() {
        mmCamController.MoveToMainMenu();
        canSelectCharacter = false;

        // Go through any players that have readied and knock them out
        if (selectedPlayerCount == twoPlayerUI) {

        } else {

        }
    }

    void ResetReady() {

    }

    void ToggleSelectedPlayerCount() {
        if (selectedPlayerCount == twoPlayerUI) {
            twoPlayerUI.GetComponent<SpriteRenderer>().sprite = deselectedBanner;
            fourPlayerUI.GetComponent<SpriteRenderer>().sprite = selectedBanner;
            selectedPlayerCount = fourPlayerUI;
        } else {
            twoPlayerUI.GetComponent<SpriteRenderer>().sprite = selectedBanner;
            fourPlayerUI.GetComponent<SpriteRenderer>().sprite = deselectedBanner;
            selectedPlayerCount = twoPlayerUI;
        }
    }

    public void StartGame() {
        SceneManager.LoadScene("Tutorialish");
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

    IEnumerator JoystickInputCountdown() {
        yield return new WaitForSeconds(0.5f);

        canInput = true;
    }

}
