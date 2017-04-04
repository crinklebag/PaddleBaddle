using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyController : MonoBehaviour {

    MainMenuCamController mmCamController;

    public enum MainMenuState { INTRO, CHOOSE_PLAYER, READY, START_GAME };
    [SerializeField] MainMenuState currentState;

    [Header ("UI References:")]
    [SerializeField] GameObject startGameUI;
    [SerializeField] GameObject playerOneUI;
    [SerializeField] GameObject playerTwoUI;

    [Header("Intro References:")]
    [SerializeField] GameObject gameNameUI;

    [Header("Player Count References:")]
    [SerializeField] GameObject playerCountUI;
    [SerializeField] GameObject twoPlayerUI;
    [SerializeField] GameObject fourPlayerUI;
    int selectedPlayerCount;

    [Header("Choose Player References:")]
    [SerializeField] PlayerSelect playerOne;
    [SerializeField] PlayerSelect playerTwo;
    [SerializeField] PlayerSelect PlayerThree;
    [SerializeField] PlayerSelect PlayerFour;


    int playersIn = 0;
    bool canInput = true;
    bool startGame = false;
    bool settingUpReady = false;

    Player[] players = new Player[4];

    // UI Fade Variables
    float startTime;
    float journeyLength;
    float speed = 1; 

    // Use this for initialization
    void Start() {
        for (int i = 0; i < players.Length; i++) {
            players[i] = ReInput.players.GetPlayer(i);
        }

        // Init Game
        mmCamController = Camera.main.gameObject.GetComponent<MainMenuCamController>();
        currentState = MainMenuState.INTRO;
	}
    // Update is called once per frame
    void Update() {
        if (players[0].GetButton("Select")) {
            // Start Game
            Debug.Log("Start Game Early");
            SceneManager.LoadScene("Boat Selection");
        }

        if (playersIn == 2) {
            // Let the players start the game
            Debug.Log("Can start now");
            if (!settingUpReady) { SetUpReady(); }
        }
        else {
            if (settingUpReady) { ResetReady(); }
        }
 
        if (startGame) { 
			FadeInStartGame(); 
			StartCoroutine(StartGameDelay());
		}
        else { FadeOutStartGame(); }

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
            canInput = false;
            StartCoroutine("JoystickInputCountdown");
        }
    }

    void HandleAPress() {

        switch (currentState) {
            case MainMenuState.INTRO:
                SetUpChoosePlayer();
                ResetIntro();
                break;
            case MainMenuState.READY:
                StartGame();
                break;
        }
    }

    void HandleBPress() {

        switch (currentState) {
            case MainMenuState.CHOOSE_PLAYER:
                if ((!playerOne.IsInGame() && !playerTwo.IsInGame())){ 
                    SetUpIntro();
                    ResetChoosePlayer();
                }
                break;
            case MainMenuState.READY:
                SetUpChoosePlayer();
                break;
        }
    }

    void SetUpIntro() {
        currentState = MainMenuState.INTRO;
        StartCoroutine (LerpFade(0.06f , 1));
        // gameNameUI.SetActive(true);
        // playerOne.gameObject.SetActive(false);
        // playerTwo.gameObject.SetActive(false);
    }

    void SetUpChoosePlayer() {
        playerOne.gameObject.SetActive(true);
        playerTwo.gameObject.SetActive(true);
        // Set the current Menu State
        currentState = MainMenuState.CHOOSE_PLAYER;
        mmCamController.MoveToCharacterSelect();
        StartCoroutine(LerpFade(0, 1));
    }

    void SetUpReady() {
        settingUpReady = true;
        currentState = MainMenuState.READY;
        startGameUI.SetActive(true);
        playerOneUI.SetActive(false);
        playerTwoUI.SetActive(false);
        startTime = Time.time;
        journeyLength = Vector3.Distance(Vector3.one, Vector3.zero);
        startGame = true;
    }

    void ResetIntro() {
        // gameNameUI.SetActive(false);
    }

    void ResetPlayerCount() {
        playerCountUI.SetActive(false);
    }

    void ResetChoosePlayer() {
        mmCamController.MoveToMainMenu();

        // Go through any players that have readied and knock them out
        playerOne.GetComponent<PlayerSelect>().ExitGame();
        playerTwo.GetComponent<PlayerSelect>().ExitGame();
    }

    void ResetReady() {
        settingUpReady = false;
        currentState = MainMenuState.CHOOSE_PLAYER;
        startTime = Time.time;
        journeyLength = Vector3.Distance(Vector3.zero, Vector3.one);
        // startGameUI.SetActive(false);
        startGame = false;
    }

    void FadeInStartGame() {
        
		Debug.Log ("Fadin in");

        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        startGameUI.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.clear, Color.white, fracJourney);
        // startGameUI.transform.GetChild(0).GetComponent<Image>().color = Color.Lerp(Color.clear, Color.white, fracJourney);
        // startGameUI.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(Color.clear, Color.white, fracJourney);

    }

    void FadeOutStartGame() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        startGameUI.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.clear, fracJourney);
        // startGameUI.transform.GetChild(0).GetComponent<Image>().color = Color.Lerp(Color.white, Color.clear, fracJourney);
        // startGameUI.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(Color.white, Color.clear, fracJourney);

        if (fracJourney >= 1) {
            playerOneUI.SetActive(true);
            playerTwoUI.SetActive(true);
        }
    }

    public void StartGame() {
        SceneManager.LoadScene("Boat Selection");
    }

    public void AddPlayer() {
        playersIn++;
        Debug.Log(playersIn);
    }
    public void RemovePlayer() {
        if (playersIn > 0) { playersIn--; }
    }

    public void ToggleOffCharacters() {
        // if we are going to the main menu and the first player is still active, turn off the players
        Debug.Log("Toggle off Players");
        if (currentState == MainMenuState.INTRO && playerOne.gameObject.activeSelf) {
            playerOne.gameObject.SetActive(false);
            playerTwo.gameObject.SetActive(false);
        }
    }

    public bool CanInput() {
        return canInput;
    }

	IEnumerator StartGameDelay(){
		
		yield return new WaitForSeconds (2.5f);

		StartGame ();
	}

    IEnumerator JoystickInputCountdown() {
        yield return new WaitForSeconds(0.5f);

        canInput = true;
    }

    IEnumerator LerpFade(float endVal, float time)
    {
        //Determine how much to fade (per step):
        float startVal = RenderSettings.fogDensity;
        float currentTime = 0f;

        //Keep upping the value and updating the LERP until we've reached target time
        while (currentTime < time)
        {
            RenderSettings.fogDensity = Mathf.Lerp(startVal, endVal, currentTime / time);
            currentTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //Set the value explicitly (to ensure it stops at what we want)
        RenderSettings.fogDensity = endVal;
    }
}
