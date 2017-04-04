using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycloneZoneController : MonoBehaviour {

    [SerializeField] FlowObject whirlpool;
    [SerializeField] SpriteRenderer[] clockwiseUI;
    [SerializeField] SpriteRenderer[] counterClockwiseUI;

    SpriteRenderer currentUIOne;
    SpriteRenderer currentUITwo;
    bool clockwise = false;
    bool flashUI = false;
    bool fadingIn = false;

    // UI Fade Variables
    float startTime;
    float journeyLength;
    float speed = 2;

    // Use this for initialization
    void Start () {
        currentUIOne = counterClockwiseUI[0];
        currentUITwo = counterClockwiseUI[1];
        flashUI = true;
        StartCoroutine(FlashUI());
        StartCoroutine(ChangeDirection());
	}
	
	// Update is called once per frame
	void Update () {

        if (fadingIn) {
            Debug.Log("Fading In");
            FadeIn();
        } else {
            Debug.Log("Fading Out");
            FadeOut();
        }
	}
    
    void FadeIn() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        currentUIOne.color = Color.Lerp(Color.clear, Color.white, fracJourney);
        currentUITwo.color = Color.Lerp(Color.clear, Color.white, fracJourney);
        // If it is finished fading in
        if (fracJourney >= 1) {
            startTime = Time.time;
            journeyLength = Vector3.Distance(Vector3.one, Vector3.zero);
            fadingIn = false;
        }
    }

    void FadeOut() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        currentUIOne.color = Color.Lerp(Color.white, Color.clear, fracJourney);
        currentUITwo.color = Color.Lerp(Color.white, Color.clear, fracJourney);
        // If it is finished fading in
        if (fracJourney >= 1) {
            if (flashUI) {
                startTime = Time.time;
                journeyLength = Vector3.Distance(Vector3.one, Vector3.zero);
                fadingIn = true;
            }
        }
    }

    IEnumerator ChangeDirection() {
        yield return new WaitForSeconds(20);

        // Change Direction
        whirlpool.ChangeCurrentDirection();
        if (whirlpool.GetCurrentDirection() > 1) {
            clockwise = false;
            currentUIOne = counterClockwiseUI[0];
            currentUITwo = counterClockwiseUI[1];
        } else {
            clockwise = true;
            currentUIOne = clockwiseUI[0];
            currentUITwo = clockwiseUI[1];
        }

        startTime = Time.time;
        journeyLength = Vector3.Distance(Vector3.one, Vector3.zero);
        flashUI = true;
        StartCoroutine(FlashUI());
        StartCoroutine(ChangeDirection());
    }

    IEnumerator FlashUI() {
        yield return new WaitForSeconds(5);
        flashUI = false;
    }
}
