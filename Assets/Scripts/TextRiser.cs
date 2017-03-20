using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextRiser : MonoBehaviour {

    Vector3 targetPos;
    Vector3 startPos;
    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;
   
    void Start()
    {
        startTime = Time.time;
        startPos = transform.position;
        targetPos = startPos + new Vector3 (0,1,0);
        journeyLength = Vector3.Distance(startPos, targetPos);
    }
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPos, targetPos, fracJourney);
    }
}