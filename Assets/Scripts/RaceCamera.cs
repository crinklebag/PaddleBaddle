﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCamera : MonoBehaviour {

    /// <summary>
    /// Private vars to store the original settings of the camera object
    /// </summary>
    private GameObject original;

    /// <summary>
    /// Going to use this regularly
    /// </summary>
    private Camera myCam;

    /// <summary>
    /// Serialized private vars to adjust camera behaviour in editor
    /// </summary>
    [SerializeField]
    private float sizeRate = 2;
    [SerializeField]
    private float pxPadding = 150;
    [SerializeField]
    private float minSize = 10;
    [SerializeField]
    private float maxSize = 100;
    [SerializeField]
    private float moveSpeed = 10;
    [SerializeField]
    private float rHeight = 50;

    /// <summary>
    /// Targets to keep in camera view
    /// Set in editor
    /// </summary>
    public Transform[] targets;
    public Vector3 target;

    /// <summary>
    /// Bool in case we want to turn 
    /// race cam mode off.
    /// Default is on.
    /// </summary>
    public bool on = true;

    /// <summary>
    /// Get the original details of the object before we start
    /// In case we need them later
    /// </summary>
    void Awake()
    {
        original = new GameObject();
        original = gameObject;

        myCam = gameObject.GetComponent<Camera>();
    }

    /// <summary>
    /// Set up the camera to be used
    /// as a dynamic race cam
    /// </summary>
	void Start ()
    {
        target = findCameraTarget(); // Find singular camera target
        myCam.orthographic = true;
        myCam.orthographicSize = 20;
    }


    /// <summary>
    /// Resizes camera to fit all targets
    /// over time.
    /// </summary>
    private void resize()
    {
        float mod = 1;

        myCam.orthographicSize += Time.deltaTime * sizeRate * mod;
    }

    private void stepTo(Transform camTarget)
    {
        transform.position = Vector3.Lerp(camTarget.position, transform.position, Time.deltaTime * moveSpeed);
    }

    /// <summary>
    /// Finds a weighted center of all targets 
    /// </summary>
    /// <returns>Position @ y = 50</returns>
    private Vector3 findCameraTarget()
    {
        float sumX = 0;
        //float sumY = 0;
        float sumZ = 0;
        int numSteps = 0;

        foreach(Transform piece in targets)
        {
            ++numSteps;
            sumX += piece.position.x;
            //sumY += piece.position.y;
            sumZ += piece.position.z;
        }

        return new Vector3(sumX / numSteps, 50, sumZ / numSteps);
    }

    // Update is called once per frame
    void Update () {
		
	}
}