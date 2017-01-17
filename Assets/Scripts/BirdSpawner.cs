using UnityEngine;
using System.Collections;

public class BirdSpawner : MonoBehaviour {
    [HideInInspector]
    public Vector3 startPos;
    [HideInInspector]
    public Vector3 endPos;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private float minTime;
    [SerializeField]
    private float maxTime;
    [SerializeField]
    private float waitTime;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        endPos = target.position;
	}

    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            // Draws a blue line from this transform to the target
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
