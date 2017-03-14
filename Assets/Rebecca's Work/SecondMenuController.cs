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
    bool teamOneSelected = false;
    bool teamTwoSelected = false;

    [Header("Movement Markers")]
    [SerializeField] GameObject blockingWall;
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
        boardOneEndMarker = new Vector3(boardOneStartMarker.x, 240, boardOneStartMarker.z);
        boardTwoEndMarker = new Vector3(boardTwoStartMarker.x, 240, boardTwoStartMarker.z);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Green Canoe
            PlayerPrefs.SetString("teamOneBoat", "raft");
            // Red Raft
            PlayerPrefs.SetString("teamTwoBoat", "canoe");
        }

        if (teamOneSelected && teamTwoSelected && !canMove) {
            canMove = true;
            startTime = Time.time;
            journeyLength = Vector3.Distance(camStartMarker, camEndMarker);
            // Turn on wall to stop them from going back to the dock - wait first so they boost past it

            // Add a small force to the boats
            float forceValue = 0;
            if (PlayerPrefs.GetString("teamOneBoat") == "canoe") { forceValue = 5000; }
            else { forceValue = 25000; }
            boatOne.GetComponentInChildren<Rigidbody>().AddForce(boatOne.transform.forward * forceValue, ForceMode.Impulse);
            if (PlayerPrefs.GetString("teamTwoBoat") == "canoe") { forceValue = 5000; }
            else { forceValue = 25000; }
            boatTwo.GetComponentInChildren<Rigidbody>().AddForce(boatOne.transform.forward * forceValue, ForceMode.Impulse);
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
        boardOne.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(boardOneStartMarker, boardOneEndMarker, fracJourney * 2);
        boardTwo.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(boardTwoStartMarker, boardTwoEndMarker, fracJourney * 2);

        // Start The Instructions
        instructionPanel.GetComponent<InstructionPanel>().StartInstructions();

        if (fracJourney >= 0.9f) {
            // turn on blocking wall
            blockingWall.SetActive(true);
            ShowInstructions();
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

    /* IEnumerator StartLevelLoad(string level) {
        loadingPanel.SetActive(true);

        yield return new WaitForSeconds(3);

        // Call this function 3 times and load the dots

        SceneManager.LoadScene(level);
    }*/
}
