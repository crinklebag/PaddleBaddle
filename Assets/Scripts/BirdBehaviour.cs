using UnityEngine;
using System.Collections;

public class BirdBehaviour : MonoBehaviour {
    public float speed = 1f;

    [HideInInspector]
    public Vector3 target;

    private Vector3 startPos;
    private float startTime;
    private float journeyLength;

    void Start()
    {
        startTime = Time.time;
        startPos = transform.position;
        journeyLength = Vector3.Distance(startPos, target);
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPos, target, fracJourney);
    }
}
