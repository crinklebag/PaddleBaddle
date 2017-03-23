using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectBoard : MonoBehaviour {

    [SerializeField] Text levelSelectText;
    [SerializeField] float speed = 10;

    Vector3 targetMarker;
    Vector3 startMarker;
    float startTime;
    float journeyLength;

    bool lower = false;

	// Use this for initialization
	void Start () {
        startMarker = this.GetComponent<RectTransform>().anchoredPosition;
        targetMarker = new Vector3(startMarker.x, -50, startMarker.z);
	}
	
	// Update is called once per frame
	void Update () {

        if (lower) { LowerPanel(); }
        else { RaisePanel(); }
	}

    void LowerPanel() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        this.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startMarker, targetMarker, fracJourney);
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
    }

    public void Deactivate() {
        startTime = Time.time;
        journeyLength = Vector3.Distance(targetMarker, startMarker);
        lower = false;
    }

    public void SetText(string level) {
        levelSelectText.text = level;
    }
}
