using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SecondMenuController : MonoBehaviour {

    [Header("UI boards")]
    [SerializeField] GameObject boardOne;
    [SerializeField] GameObject boardTwo;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject greenPlayerLevelBoard;
    [SerializeField] GameObject redPlayerLevelBoard;
    [SerializeField] GameObject instructionPanel;

    [Header("Game Boats")]
    [SerializeField] GameObject boatOne;
    [SerializeField] GameObject boatTwo;

    bool canMove = false;

    [SerializeField] GameObject blockingWall;

    bool teamOneSelected = false;
    bool teamTwoSelected = false;
    bool instructionsStarted = false;
    bool raiseBoards = false;

    [Header("Movement Markers")]
    [SerializeField] Vector3 camStartMarker;
    [SerializeField] Vector3 boardOneStartMarker;
    [SerializeField] Vector3 boardTwoStartMarker;
    [SerializeField] Vector3 camEndMarker;
    [SerializeField] float speed = 5;

    float startTime = 0;
    float journeyLength = 0;

    Vector3 boardOneEndMarker;
    Vector3 boardTwoEndMarker;

    // PLayer Data
    int playersIn = 0;
    int maxPlayers = 2;
    // Players Level Choices
    string playerOneChoice = " ";
    string playerTwoChoice = " ";

    // Use this for initialization
    void Start () {
        boardOneStartMarker = boardOne.GetComponent<RectTransform>().anchoredPosition;
        boardTwoStartMarker = boardTwo.GetComponent<RectTransform>().anchoredPosition;
        boardOneEndMarker = new Vector3(boardOne.GetComponent<RectTransform>().anchoredPosition.x, boardOne.GetComponent<RectTransform>().anchoredPosition.y + 500, 0);
        boardTwoEndMarker = new Vector3(boardTwo.GetComponent<RectTransform>().anchoredPosition.x, boardTwo.GetComponent<RectTransform>().anchoredPosition.y + 500, 0);
    }
	
	// Update is called once per frame
	void Update () {
       /* if (Input.GetKeyDown(KeyCode.Space)) {
            // Green Canoe
            PlayerPrefs.SetString("teamOneBoat", "raft");
            // Red Raft
            PlayerPrefs.SetString("teamTwoBoat", "canoe");
        } */

        if (teamOneSelected && teamTwoSelected && !canMove) {
            canMove = true;
            startTime = Time.time;
            journeyLength = Vector3.Distance(camStartMarker, camEndMarker);
            // Turn on wall to stop them from going back to the dock - wait first so they boost past it

            // Add a small force to the boats
            float teamOneForceValue = 5000;
            float teamTwoForceValue = 5000;
            if (PlayerPrefs.GetString("teamOneBoat") == "canoe") { 
				// Debug.Log ("Team One Canoe Force");
				teamOneForceValue = 7000; 
			}
            else {
				// Debug.Log ("Team One Raft Force");
				teamOneForceValue = 8000; 
			}
            if (PlayerPrefs.GetString("teamTwoBoat") == "canoe") { teamTwoForceValue = 7000; }
            else { teamTwoForceValue = 8000; }
            boatOne.GetComponentInChildren<Rigidbody>().AddForce(boatOne.transform.forward * teamOneForceValue, ForceMode.Impulse);
            boatTwo.GetComponentInChildren<Rigidbody>().AddForce(boatOne.transform.forward * teamTwoForceValue, ForceMode.Impulse);

            boatOne.GetComponent<MenuMovement>().AllowAttack();
            boatTwo.GetComponent<MenuMovement>().AllowAttack();
        }

        if (canMove) {
            SetUpLevelSelect();
        }
	}

    /* Instructions */
    void ShowInstructions() {
        instructionPanel.SetActive(true);
    }

    /* Level Selection */
    void SetUpLevelSelect() {
        // Debug.Log("Setting Up Level");
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        // Move up the Camera
        Camera.main.transform.position = Vector3.Lerp(camStartMarker, camEndMarker, fracJourney);
        // Move up the boards
        boardOne.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(boardOneStartMarker, boardOneEndMarker, fracJourney * 1.5f);
        boardTwo.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(boardTwoStartMarker, boardTwoEndMarker, fracJourney * 1.5f);

        // Start The Instructions
        if (!instructionsStarted) {
            instructionPanel.GetComponent<InstructionPanel>().StartInstructions();
            instructionsStarted = true;
            // Debug.Log("Starting Instructions in Menu Controller");
        }

        if (fracJourney >= 0.9f) {
            // turn on blocking wall
            blockingWall.SetActive(true);
            ShowInstructions();
            // canMove = false;
        }
    }

    void SetPlayerChoice(int playerID, string level) {
        if (playersIn < maxPlayers) { playersIn++; }

        switch (playerID) {
            case 0:
                playerOneChoice = level;
                greenPlayerLevelBoard.SetActive(true);
                greenPlayerLevelBoard.GetComponent<LevelSelectBoard>().SetText(level);
                // Lower it down - Call a function on it?
                greenPlayerLevelBoard.GetComponent<LevelSelectBoard>().Activate();
                break;
            case 1:
                playerTwoChoice = level;
                redPlayerLevelBoard.SetActive(true);
                redPlayerLevelBoard.GetComponent<LevelSelectBoard>().SetText(level);
                redPlayerLevelBoard.GetComponent<LevelSelectBoard>().Activate();
                break;
        }
    }

    public void ClearPlayerChoice(int playerID) {
        if (playersIn > 0) { playersIn--; }

        switch (playerID) {
            case 0:
                playerOneChoice = " ";
                greenPlayerLevelBoard.GetComponent<LevelSelectBoard>().Deactivate();
                break;
            case 1:
                playerTwoChoice = " ";
                redPlayerLevelBoard.GetComponent<LevelSelectBoard>().Deactivate();
                break;
        }
    }

    public void LoadLevel(int playerID, string level) {
        SetPlayerChoice(playerID, level);
        if ((playersIn == maxPlayers) && (playerOneChoice == playerTwoChoice)) {
            // StartCoroutine(StartLevelLoad(level));
            loadingPanel.GetComponent<LevelSelectLoadPanel>().StartLevelLoad(level);
        }
    }
    
    /* Boat Selection */
    public void TeamOneSelect(string boatSelection) {
        teamOneSelected = true;
        PlayerPrefs.SetString("teamOneBoat", boatSelection);
        // Debug.Log("Team Two Boat: " + PlayerPrefs.GetString("teamTwoBoat"));
    }

    public void TeamOneDeselect() {
        teamOneSelected = false;
        PlayerPrefs.SetString("teamOneBoat", "");
    }

    public void TeamTwoSelect(string boatSelection) {
        teamTwoSelected = true;
        PlayerPrefs.SetString("teamTwoBoat", boatSelection);
        // Debug.Log("Team Two Boat: " + PlayerPrefs.GetString("teamTwoBoat"));
    }

    public void TeamTwoDeselect() {
        teamTwoSelected = false;
        PlayerPrefs.SetString("teamTwoBoat", "");
    }

    public bool CanMove() {
        return canMove;
    }
}
