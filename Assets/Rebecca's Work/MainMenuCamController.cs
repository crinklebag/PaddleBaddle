﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamController : MonoBehaviour {

    public enum CameraState { MAIN_MENU, CHARACTER_SELECT };
    CameraState currentState;

    [SerializeField] Transform mainMenuDestination;
    [SerializeField] Transform characterSelectDestination;
    [SerializeField] Color nightSkyColor;
    [SerializeField] Color daySkyColor;
    [SerializeField] Light mainLight;
    [SerializeField] Light secondaryLight;

    [SerializeField] float nightLightIntensity;
    [SerializeField] float dayLightIntensity1;
    [SerializeField] float dayLightIntensity2;
    [SerializeField] float speed;

    bool moveCam = false;
    Transform destination;
    Color targetSkyColor;
    float targetLightIntensity1 = 0;
    float targetLightIntensity2 = 0;
    float startTime;
    float journeyLength;

    // Use this for initialization
    void Start () {
        currentState = CameraState.MAIN_MENU;
        SetUpMainMenu();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Space)) {
            if (currentState == CameraState.MAIN_MENU && destination != characterSelectDestination) {
                destination = characterSelectDestination;
                targetSkyColor = daySkyColor;
                targetLightIntensity1 = dayLightIntensity1;
                targetLightIntensity2 = dayLightIntensity2;
                currentState = CameraState.CHARACTER_SELECT;
            }
            else if (currentState == CameraState.CHARACTER_SELECT) {
                destination = mainMenuDestination;
                targetSkyColor = nightSkyColor;
                targetLightIntensity1 = nightLightIntensity;
                targetLightIntensity2 = nightLightIntensity;
                currentState = CameraState.MAIN_MENU;
            }

            startTime = Time.time;
            journeyLength = Vector3.Distance(this.transform.position, destination.position); 
            moveCam = true;
        }

        if (moveCam) {
            MoveCamera();
        }
	}

    void SetUpMainMenu() {
        this.transform.position = mainMenuDestination.position;
        this.GetComponent<Camera>().backgroundColor = nightSkyColor;
        mainLight.intensity = nightLightIntensity;
        secondaryLight.intensity = nightLightIntensity;
    }
    
    void MoveCamera() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        this.transform.position = Vector3.Lerp(this.transform.position, destination.transform.position, fracJourney);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, destination.transform.rotation, fracJourney);
        this.GetComponent<Camera>().backgroundColor = Color.Lerp(this.GetComponent<Camera>().backgroundColor, targetSkyColor, fracJourney);
        mainLight.intensity = Mathf.Lerp(mainLight.intensity, targetLightIntensity1, fracJourney);
        secondaryLight.intensity = Mathf.Lerp(secondaryLight.intensity, targetLightIntensity2, fracJourney);
    }
}