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

    private GameObject[] targets;

    private GameObject target;

	// Use this for initialization
	void Start () {
        targets = GameObject.FindGameObjectsWithTag("Player");
        target = new GameObject("CamTarget");
        findCameraTarget();
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

        target.transform.position = new Vector3(sumX / numSteps, sumY / numSteps, sumZ / numSteps);
    }

    void centreCamera()
    {

    }

    void resize()
    {

    }
}
