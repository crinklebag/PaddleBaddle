using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycloneZoneController : MonoBehaviour {

    [SerializeField] FlowObject whirlpool;
    [SerializeField] PickupFlowObject pickupWhirlpool;
    [SerializeField] SpriteRenderer[] clockwiseUI;
    [SerializeField] SpriteRenderer[] counterClockwiseUI;
    [SerializeField] SpriteRenderer whirlpoolImage;
    [SerializeField] Sprite clockwiseSprite;
    [SerializeField] Sprite counterClockwiseSprite;
    [SerializeField] ParticleSystem[] counterClockwiseWaves;
    [SerializeField] ParticleSystem[] clockwiseWaves;

    SpriteRenderer currentUIOne;
    SpriteRenderer currentUITwo;
    bool clockwise = false;
    bool flashUI = false;
    bool fadingIn = false;
    bool startGame = false;

    // UI Fade Variables
    float startTime;
    float journeyLength;
    float speed = 2;

    // Whirlpool Variables
    float whirlpoolStartTime = 0;
    float whirlpoolJourneyLength = 0;
    float whirlpoolSpeed = 3;
    bool shrinkWhirlpool = false;

    // Use this for initialization
    void Start () {
        // Initialize as counter clockwise
        currentUIOne = counterClockwiseUI[0];
        currentUITwo = counterClockwiseUI[1];
        flashUI = true;
        clockwise = false;
        StartCoroutine(FlashUI());
        StartCoroutine(ChangeDirection());

        // Whirlpool Stuff - whirlpool starts hidden, reveal it on start
        shrinkWhirlpool = false;
        whirlpoolStartTime = Time.time;
        whirlpoolJourneyLength = Vector3.Distance(new Vector3(2, 2, 2), Vector3.zero);
        ToggleWaves();
    }
	
	// Update is called once per frame
	void Update () {

        if (fadingIn) {
            // Debug.Log("Fading In");
            FadeIn();
        } else {
            // Debug.Log("Fading Out");
            FadeOut();
        }

        if (shrinkWhirlpool) {
            ShrinkWhirlpool();
        } else {
            GrowWhirlpool();
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
        yield return new WaitForSeconds(10);

        Debug.Log("Changing Direction");

        // Change Direction
        whirlpool.ChangeCurrentDirection();
        pickupWhirlpool.ChangeCurrentDirection();
        if (whirlpool.GetCurrentDirection() > 1) {
            clockwise = false;
            currentUIOne = counterClockwiseUI[0];
            currentUITwo = counterClockwiseUI[1];
        } else {
            clockwise = true;
            currentUIOne = clockwiseUI[0];
            currentUITwo = clockwiseUI[1];
        }

        Debug.Log("Changing Whirlpool Direction");
        whirlpoolStartTime = Time.time;
        whirlpoolJourneyLength = Vector3.Distance(Vector3.zero, new Vector3(2, 2, 2));
        shrinkWhirlpool = true;
        
        ToggleWaves();

        // Start Shrinking the Whirlpool - set start variables
        /* if (startGame)
        {
            
        }
        else { startGame = true; } */

        startTime = Time.time;
        journeyLength = Vector3.Distance(Vector3.one, Vector3.zero);
        flashUI = true;

        StartCoroutine(FlashUI());
        StartCoroutine(ChangeDirection());
    }

    void ShrinkWhirlpool() {
        float distCovered = (Time.time - whirlpoolStartTime) * whirlpoolSpeed;
        float fracJourney = distCovered / whirlpoolJourneyLength;

        // Shrink the whirlpool image
        whirlpoolImage.transform.localScale = Vector3.Lerp(new Vector3(2, 2, 2), Vector3.zero, fracJourney);

        if (fracJourney >= 1) {
            shrinkWhirlpool = false;
            whirlpoolStartTime = Time.time;
            whirlpoolJourneyLength = Vector3.Distance(new Vector3(2, 2, 2), Vector3.zero);
            whirlpoolImage.GetComponent<RotateClouds>().ChangeDirection();
            // Check what direction and change the sprite
            if (!clockwise) { whirlpoolImage.sprite = clockwiseSprite; }
            else { whirlpoolImage.sprite = counterClockwiseSprite; }
        }
    }

    void GrowWhirlpool() {
        float distCovered = (Time.time - whirlpoolStartTime) * whirlpoolSpeed;
        float fracJourney = distCovered / whirlpoolJourneyLength;
        
        // Shrink the whirlpool image
        whirlpoolImage.transform.localScale = Vector3.Lerp( Vector3.zero, new Vector3(2, 2, 2), fracJourney);
        
    }

    void ToggleWaves() {
        if (clockwise)
        {
            // turn on clockwise guys
            for (int i = 0; i < clockwiseWaves.Length; i++) {
                clockwiseWaves[i].Play();
            }

            // turn off counter clockwise guys
            for (int i = 0; i < counterClockwiseWaves.Length; i++) {
                counterClockwiseWaves[i].Stop();
            }
        }
        else {
            // turn on clockwise guys
            for (int i = 0; i < clockwiseWaves.Length; i++) {
                clockwiseWaves[i].Stop();
            }

            // turn off counter clockwise guys
            for (int i = 0; i < counterClockwiseWaves.Length; i++) {
                counterClockwiseWaves[i].Play();
            }
        }
    }

    IEnumerator FlashUI() {
        yield return new WaitForSeconds(5);
        flashUI = false;
    }
}
