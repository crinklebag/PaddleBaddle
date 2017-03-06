using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobandRotate : MonoBehaviour {

    [SerializeField] float rotateamount;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, rotateamount);
        this.transform.position = new Vector3(transform.position.x, 18 + (Mathf.Sin(Time.time)/10), transform.position.z);
	}
}
