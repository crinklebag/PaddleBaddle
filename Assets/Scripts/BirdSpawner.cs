using UnityEngine;
using System.Collections;

public class BirdSpawner : MonoBehaviour {
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float minTime = 0f;
    [SerializeField]
    private float maxTime = 10f;
    [SerializeField]
    private float waitTime = 1f;

    private Vector3 startPos;
    private Vector3 endPos;

    // Use this for initialization
    void Start () {
        startPos = transform.position;
        endPos = target.position;
	}

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(waitTime);
    }

    // Draws the line in the editor
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
