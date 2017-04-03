using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour {

    /// <summary>
    /// Public bool to turn on dynamic camera.
    /// </summary>
    public bool Dynamic = false;

    /// <summary>
    /// Exposed minimum zDistance cap.
    /// </summary>
    [SerializeField]
    private float minDistance = 10f;

    /// <summary>
    /// Exposed maximum zDistance cap.
    /// </summary>
    [SerializeField]
    private float maxDistance = Mathf.Infinity;

    /// <summary>
    /// Zoom rate.
    /// </summary>
    [SerializeField]
    private float sizeRate = 3f;

    /// <summary>
    /// Move rate.
    /// </summary>
    [SerializeField]
    private float moveSpeed = 5f;

    /// <summary>
    /// Modifier for zDistance
    /// Should increase with a shallower viewing angle.
    /// </summary>
    [SerializeField]
    private float zoomMod = 1.5f;

    /// <summary>
    /// How close to get before stopping
    /// </summary>
    private float tolerance = 0.05f;

    /// <summary>
    /// Boom length
    /// </summary>
    private float zDistance;

    /// <summary>
    /// Camera field of view in degrees
    /// </summary>
    private float FOV;
    
    /// <summary>
    /// All GameObjects in scene tagged "Player"
    /// </summary>
    private GameObject[] targets;

    /// <summary>
    /// Weighted centre of all targets
    /// </summary>
    private Vector3 target;

    /// <summary>
    /// Artificial camera boom parented to camera
    /// and positioned at target.
    /// </summary>
    private GameObject Boom;

	// Use this for initialization
	void Start () {
        targets = GameObject.FindGameObjectsWithTag("Player");
        Boom = new GameObject("Camera Boom");
        FOV = gameObject.GetComponent<Camera>().fieldOfView;
        findCameraTarget();
        transform.SetParent(Boom.transform);
	}
	
	/// <summary>
    /// If we make a re-init function, switch this from 
    /// if to gate.
    /// </summary>
	void Update () {
		if (Dynamic)
        {
            centreCamera();
            resize();
        }
	}

    /// <summary>
    /// Finds a weighted center of all targets 
    /// </summary>
    private void findCameraTarget()
    {
        float sumX = 0;
        float sumY = 0;
        float sumZ = 0;
        int numSteps = 0;

        foreach (GameObject piece in targets)
        {
            ++numSteps;
            sumX += piece.transform.position.x;
            sumY += piece.transform.position.y;
            sumZ += piece.transform.position.z;
        }

        target = new Vector3(sumX / numSteps, sumY / numSteps, sumZ / numSteps);
    }

    /// <summary>
    /// Find the needed local Z distance
    /// Given FOV and distance between targets.
    /// </summary>
    void findZDistance()
    {
        float dist = Vector3.Distance(targets[0].transform.position, targets[1].transform.position);

        zDistance = (dist/2) / Mathf.Tan(toRad(FOV / 2)) * zoomMod;
        zDistance = Mathf.Clamp(zDistance, minDistance, maxDistance);
    }

    /// <summary>
    /// Smoothly move Boom to weighted centre of targets
    /// </summary>
    void centreCamera()
    {
        findCameraTarget();

        Boom.transform.position = Vector3.MoveTowards(Boom.transform.position, target, Time.deltaTime * moveSpeed);
    }

    /// <summary>
    /// Smoothly zoom in or out until within 
    /// tolerance of desired zDistance.
    /// </summary>
    void resize()
    {
        findZDistance();

        float dist = Vector3.Distance(transform.position, Boom.transform.position);

        if (zDistance - tolerance < dist)
        {
            transform.position += transform.forward * sizeRate * Time.deltaTime;
        }

        if (dist < zDistance + tolerance)
        {
            transform.position -= transform.forward * sizeRate * Time.deltaTime;
        }
    }

    /// <summary>
    /// Conversion from degrees to radians.
    /// </summary>
    /// <param name="degrees"></param>
    /// <returns></returns>
    float toRad(float degrees)
    {
        return ((degrees * Mathf.PI) / 180f);
    }
}
