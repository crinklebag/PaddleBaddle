using UnityEngine;
using System.Collections;

public class RotateClouds : MonoBehaviour {
    [SerializeField]
    float speed = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, speed * -1f, 0);
	}
}
