using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectLoadPanel : MonoBehaviour {

    [SerializeField] Text loadingText;

    float startTime = 0;
    float journeyLength = 0;
    float speed = 100;
    int secondsToWait = 3;
    int secondsPast = 0;
    bool lowerBoard = false;
    string levelToLoad = "";

    Vector3 startMarker;
    Vector3 endMarker;

    // -45

    // Use this for initialization
    void Start () {
        startMarker = gameObject.GetComponent<RectTransform>().anchoredPosition;
        endMarker = new Vector3(startMarker.x, startMarker.y - 200, startMarker.z);
    }
	
	// Update is called once per frame
	void Update () {
        /* if (Input.GetKeyDown(KeyCode.Space)) {
            StartLevelLoad("Fake Level");
        }

        if (lowerBoard) {
            LowerBoard();
        } */
	}

    void LowerBoard() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startMarker, endMarker, fracJourney);
        if (fracJourney >= 1) {
            lowerBoard = false;
            StartCoroutine(Countdown());
        }
    }

    IEnumerator Countdown() {
        switch (secondsPast) {
            case 0:
                loadingText.text = "Loading.";
                break;
            case 1:
                loadingText.text = "Loading..";
                break;
            case 2:
                loadingText.text = "Loading...";
                break;
        }

        yield return new WaitForSeconds(1f);

        secondsPast++;
        if (secondsPast == secondsToWait) {
            // Load level
            SceneManager.LoadScene(levelToLoad);
        }
        else { StartCoroutine(Countdown()); }
    }

    public void StartLevelLoad(string level) {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker, endMarker);
        lowerBoard = true;
        levelToLoad = level;
    }
}
