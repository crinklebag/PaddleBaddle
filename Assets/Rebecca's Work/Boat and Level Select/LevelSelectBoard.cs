using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectBoard : MonoBehaviour {

    [SerializeField] Text levelSelectText;
    [SerializeField] float speed = 10;

    Vector3 targetMarker;
    Vector3 startMarker;
    Vector3 bounceTarget;
    Vector3 bounceStart;
    float startTime;
    float journeyLength;

    bool lower = false;
    bool bounce = false;

	// Use this for initialization
	void Start () {
        startMarker = this.GetComponent<RectTransform>().anchoredPosition;
        targetMarker = new Vector3(startMarker.x, startMarker.y - 150, startMarker.z);
	}

    // Update is called once per frame
    void Update() {

        if (!bounce) {
            if (lower) { LowerPanel(); }
            else { RaisePanel(); }
        }

        if (bounce) {
            Bounce();
        }
	}

    void Bounce() {
        float distCovered = (Time.time - startTime) * 50;
        float fracJourney = distCovered / journeyLength;

        this.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(bounceStart, bounceTarget, fracJourney);
        
    }

    void LowerPanel() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        this.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startMarker, targetMarker, fracJourney);

        if (fracJourney >= 1) {
            lower = false;
            bounce = true;
            bounceStart = this.GetComponent<RectTransform>().anchoredPosition;
            bounceTarget = new Vector3(this.GetComponent<RectTransform>().anchoredPosition.x, this.GetComponent<RectTransform>().anchoredPosition.y + 10, 0);
        } 
    }

    void RaisePanel() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        this.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(targetMarker, startMarker, fracJourney);
    }

    public void Activate() {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker, targetMarker);
        lower = true;
        bounce = false;
    }

    public void Deactivate() {
        startTime = Time.time;
        journeyLength = Vector3.Distance(targetMarker, startMarker);
        lower = false;
        bounce = false;
    }

    public void SetText(string level) {
        levelSelectText.text = level;
    }
}
