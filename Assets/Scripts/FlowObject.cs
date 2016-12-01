using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowObject : MonoBehaviour {
	[SerializeField]
	private bool isCurrent = false;
	[SerializeField]
	private bool isWhirlpool = false;
	[SerializeField]
	private float effectRadius = 0;
	[SerializeField]
	private float pullStrength = 0;
	[SerializeField]
	private float rotateStrength = 0;

	private List<GameObject> thingsInRange = new List<GameObject>();


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isWhirlpool) {
			whirlPoolEffect ();
		}

		if (isCurrent) {
			currentEffect ();
		}
	}

	void whirlPoolEffect()
	{
		if (thingsInRange.Count < 1)
			return;
		
		Vector3 toCentre;
		
		foreach (GameObject thing in thingsInRange)
		{
			toCentre = transform.position - thing.transform.position;

			Vector3 centreNormal = Vector3.Cross (transform.up, toCentre);
			if (toCentre.magnitude > effectRadius) {
				toCentre.Normalize ();
				toCentre *= effectRadius;
			}

			float inPercent = 1 - (toCentre.magnitude / effectRadius);
			toCentre.Normalize ();
			centreNormal.Normalize ();

			Vector3 resultingForce = (toCentre * inPercent * pullStrength) + (centreNormal * inPercent * rotateStrength);
			Debug.Log ("Adding " + resultingForce + " to " + thing);
			//thing.GetComponent<Rigidbody> ().AddForce (resultingForce);

			Rigidbody[] rigidbodies = thing.GetComponentsInChildren<Rigidbody> ();
			if (rigidbodies.Length > 0) {
				rigidbodies [0].AddForce (resultingForce);
			}
		}
	}

	void currentEffect()
	{
		foreach (GameObject thing in thingsInRange) {
			Rigidbody[] rigidbodies = thing.GetComponentsInChildren<Rigidbody> ();
			if (rigidbodies.Length > 0) {
				rigidbodies [0].AddForce (transform.forward *  pullStrength);
			}
		}
	}
		

	// Keep track of objects in range
	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Adding " + other.name + " to the list");
		thingsInRange.Add (other.gameObject);
	}

	void OnTriggerExit(Collider other)
	{
		if (thingsInRange.Contains (other.gameObject)) {
			Debug.Log ("Trying to remove " + other.name);
			thingsInRange.Remove (other.gameObject);
		}
	}

}
