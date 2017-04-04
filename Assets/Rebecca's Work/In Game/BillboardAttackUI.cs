using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BillboardAttackUI : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
		
	}

	void Awake() {
		transform.LookAt (Camera.main.transform);
	}

	// Update is called once per frame
	void Update () {
		transform.LookAt (Camera.main.transform);
	}
}
