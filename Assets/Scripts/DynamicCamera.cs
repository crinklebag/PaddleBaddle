using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour {

    public bool Dynamic = false;

    [SerializeField]
    private float minDistance = 10f;

    [SerializeField]
    private float maxDistance = Mathf.Infinity;

    [SerializeField]
    private float sizeRate = 1f;

    [SerializeField]
    private float moveSpeed = 1f;

    private float distanceToTarget;

    private float tolerance = 0.15f;

    private float zDistance;

    private GameObject[] targets;

    private Vector3 target;

    private GameObject Boom;

	// Use this for initialization
	void Start () {
        targets = GameObject.FindGameObjectsWithTag("Player");
        Boom = new GameObject("Camera Boom");
        findCameraTarget();
        transform.SetParent(Boom.transform);
	}
	
	// Update is called once per frame
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

    void findZDistance()
    {
        float dist = Vector3.Distance(targets[0].transform.position, targets[1].transform.position);
        float FOV = gameObject.GetComponent<Camera>().fieldOfView;

        zDistance = Mathf.Abs(dist) / Mathf.Tan(FOV / 2);

    }

    void centreCamera()
    {
        findCameraTarget();

        Boom.transform.position = Vector3.MoveTowards(Boom.transform.position, target, Time.deltaTime * moveSpeed);
    }

    void resize()
    {
        findZDistance();

        float dist = Vector3.Distance(transform.position, Boom.transform.position);

        distanceToTarget = dist;

        if (zDistance - tolerance < dist)
        {
            transform.position += transform.forward * sizeRate * Time.deltaTime;
        }

        if (dist < zDistance + tolerance)
        {
            transform.position -= transform.forward * sizeRate * Time.deltaTime;
        }
    }
}
