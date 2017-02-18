using System;
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
    private float sizeRate = 0.5f;
    //[SerializeField]
    //private float pxPadding = 150;
    [SerializeField]
    private float minSize = 10;
    [SerializeField]
    private float maxSize = 100;
    [SerializeField]
    private float moveSpeed = 3;
    [SerializeField]
    private float rHeight = 50;
    [SerializeField]
    private float frac = 1;

    /// <summary>
    /// Targets to keep in camera view
    /// Set in editor
    /// </summary>
    public GameObject[] targets;
    [HideInInspector]
    public Vector3 target;

    /// <summary>
    /// Bool in case we want to turn 
    /// race cam mode off.
    /// Default is on.
    /// </summary>
    public bool on = true;
    private bool gate = false;
    private bool change = false;

    /// <summary>
    /// Get the original details of the object before we start
    /// In case we need them later
    /// </summary>
    void Awake()
    {
        camSetup();
    }

    /// <summary>
    /// Set up the race camera
    /// </summary>
    private void camSetup()
    {
        original = new GameObject();
        original = gameObject;

        myCam = gameObject.GetComponent<Camera>();
        myCam.orthographic = true;
    }

    /// <summary>
    /// Set up the camera to be used
    /// as a dynamic race cam
    /// </summary>
    void Start ()
    {
        if (on)
        { camSetup(); }
    }

    /// <summary>
    /// Resizes camera to fit all targets
    /// over time.
    /// </summary>
    private void resize()
    {
        float distance = Vector3.Distance(targets[0].transform.position, targets[1].transform.position);
        myCam.orthographicSize = Mathf.Clamp((myCam.orthographicSize + ((frac * distance) - myCam.orthographicSize) * Time.deltaTime * sizeRate),
            minSize, maxSize);
    }

    /// <summary>
    /// Finds a weighted center of all targets 
    /// </summary>
    private void findCameraTarget()
    {
        float sumX = 0;
        //float sumY = 0;
        float sumZ = 0;
        int numSteps = 0;

        foreach(GameObject piece in targets)
        {
            ++numSteps;
            sumX += piece.transform.position.x;
            //sumY += piece.transform.position.y;
            sumZ += piece.transform.position.z;
        }

        target = new Vector3(sumX / numSteps, 50, sumZ / numSteps);
    }

    /// <summary>
    /// Determine if the script should run,
    /// reset camera,
    /// or set up race cam again.
    /// 
    /// Then do that.
    /// </summary>
    void Update ()
    {
        // Is the gate open or closed?
        gate = on && change;

        if (gate) // is open
        {
            change = !change; // close the gate

            if (on)
            { camSetup(); } // set up race camera
            else 
            { myCam = original.GetComponent<Camera>(); } //reset state of camera
        }

        if (on)
        {
            findCameraTarget();
            transform.position = Vector3.MoveTowards(target,
                transform.position, Time.deltaTime * moveSpeed); 
            resize();
        }
    }
}
