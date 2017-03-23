using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionPanel : MonoBehaviour {
    

    [SerializeField] GameObject triggerPrompts;
    [SerializeField] GameObject bumperPrompts;
    GameObject currentPrompt;

    bool fadeInstructions = false;
    bool fadeIn = true;
    bool showTriggers = true;

    // UI Fade Variables
    float startTime;
    float journeyLength;
    float speed = 1;

    // Use this for initialization
    void Start () {
		
	}

	// Update is called once per frame
	void Update () {
        /* if (Input.GetKeyDown(KeyCode.Space)) {
            // StartInstructions();

        } */

        if (fadeInstructions && showTriggers) {
            if (fadeIn) {
                FadeIn(triggerPrompts);
            } else {
                FadeOut(triggerPrompts);
            }
        }

        if (fadeInstructions && !showTriggers) {
            if (fadeIn) {
                FadeIn(bumperPrompts);
            } else {
                FadeOut(bumperPrompts);
            }
        }
	}

    void FadeIn(GameObject UIPiece) {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        UIPiece.GetComponent<Image>().color = Color.Lerp(Color.clear, Color.white, fracJourney);
        // If it is finished fading in
        if (fracJourney >= 1) {
            StartCoroutine(FadeDelay());
            fadeInstructions = false;
            fadeIn = false;
        }
    }

    void FadeOut(GameObject UIPiece) {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        UIPiece.GetComponent<Image>().color = Color.Lerp(Color.white, Color.clear, fracJourney);
        // If it is finished fading in
        if (fracJourney >= 1) {
            // StartCoroutine(FadeDelay());
            startTime = Time.time;
            journeyLength = Vector3.Distance(Vector3.one, Vector3.zero);
            fadeIn = true;
            if (showTriggers) { showTriggers = false; }
            else { fadeInstructions = false; }
        }
    }

    // Show this from the menu controller after both players have selected a boat
    public void StartInstructions() {
        Debug.Log("Starting Instructions in Instruction Panel");
        fadeInstructions = true;
        startTime = Time.time;
        journeyLength = Vector3.Distance(Vector3.one, Vector3.zero);
    }

    IEnumerator FadeDelay() {
        yield return new WaitForSeconds(4);

        startTime = Time.time;
        journeyLength = Vector3.Distance(Vector3.one, Vector3.zero);
        fadeInstructions = true;
    }
}
