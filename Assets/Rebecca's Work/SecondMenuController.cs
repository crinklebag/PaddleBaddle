using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SecondMenuController : MonoBehaviour {

    [Header("UI boards")]
    [SerializeField] GameObject boardOne;
    [SerializeField] GameObject boardTwo;

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

    // Movement Variables

    // Use this for initialization
    void Start () {
        boardOneEndMarker = new Vector3(boardOneStartMarker.x, 240, boardOneStartMarker.z);
        boardTwoEndMarker = new Vector3(boardTwoStartMarker.x, 240, boardTwoStartMarker.z);
    }
	
	// Update is called once per frame
	void Update () {
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

    void SetUpLevelSelect() {
        Debug.Log("Setting Up Level");
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        // Move up the Camera
        Camera.main.transform.position = Vector3.Lerp(camStartMarker, camEndMarker, fracJourney);
        // Move up the boards
        boardOne.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(boardOneStartMarker, boardOneEndMarker, fracJourney * 2);
        boardTwo.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(boardTwoStartMarker, boardTwoEndMarker, fracJourney * 2);

        if (fracJourney >= 0.9f) {
            // turn on blocking wall
            blockingWall.SetActive(true);
        }
    }

    public void TeamOneSelect(string boatSelection) {
        teamOneSelected = true;
        PlayerPrefs.SetString("teamOneBoat", boatSelection);
        Debug.Log("Team Two Boat: " + PlayerPrefs.GetString("teamTwoBoat"));
    }

    public void TeamOneDeselect() {
        teamOneSelected = false;
        PlayerPrefs.SetString("teamOneBoat", "");
    }

    public void TeamTwoSelect(string boatSelection) {
        teamTwoSelected = true;
        PlayerPrefs.SetString("teamTwoBoat", boatSelection);
        Debug.Log("Team Two Boat: " + PlayerPrefs.GetString("teamTwoBoat"));
    }

    public void TeamTwoDeselect() {
        teamTwoSelected = false;
        PlayerPrefs.SetString("teamTwoBoat", "");
    }

    public bool CanMove() {
        return canMove;
    }
}
