using UnityEngine;
using System.Collections;

public class RotateClouds : MonoBehaviour {
    [SerializeField] float speed = -1;
    [SerializeField] Vector3 axisOfRotation = Vector3.up;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(axisOfRotation * speed);
	}

    public void ChangeDirection() {
        speed *= -1;
    }
}
