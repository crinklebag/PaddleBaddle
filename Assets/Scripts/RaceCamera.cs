using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCamera : MonoBehaviour {
    
    /// <summary>
    /// Private vars to store the original settings of the camera object
    /// </summary>
    private Transform oTransform;
    private Camera oCamera;

    /// <summary>
    /// Serialized private vars to adjust camera behaviour in editor
    /// </summary>
    [SerializeField]
    private float sizeRate;
    [SerializeField]
    private float padding;
    [SerializeField]
    private float minSize;
    [SerializeField]
    private float maxSize;
    [SerializeField]
    private float moveSpeed;

    /// <summary>
    /// Targets to keep in camera view
    /// </summary>
    public Transform[] targets;

    /// <summary>
    /// Get the original details of the object before we start
    /// </summary>
    void Awake()
    {
        oTransform = gameObject.GetComponent<Transform>();
        oCamera = gameObject.GetComponent<Camera>();
    }

	void Start ()
    {
        findTargets();
        Transform camTarget = findCameraTarget(); // Find singular camera target
        gameObject.GetComponent<Camera>().orthographic = true;
        moveCamTo(camTarget);
        resize();
    }

    private void resize()
    {
        throw new NotImplementedException();
    }

    private void moveCamTo(Transform camTarget)
    {
        throw new NotImplementedException();
    }

    private Transform findCameraTarget()
    {
        throw new NotImplementedException();
    }

    private void findTargets()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
