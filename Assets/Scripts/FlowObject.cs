using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// For organizational purposes, information about 
/// Whirlpool and CurrentObjects will be stored up here in seperate classes
/// </summary>
[System.Serializable]
class WhirlPool {
	public float effectRadius = 0;
	public float pullStrength = 0;
	public float rotateStrength = 0;
}

[System.Serializable]
class CurrentObject {
	public float magnitude = 10000;
}

public class FlowObject : MonoBehaviour {
	[SerializeField]
	private WhirlPool whirlpool;
	[SerializeField]
	private CurrentObject current;

	private bool isCurrent = false;
	private bool isWhirlpool = false;
	private List<GameObject> thingsInRange = new List<GameObject>();

	/// <summary>
	/// Use the type of collider attached to the object to determine whether or not
	/// it is a whirlpool object or a current object
	/// </summary>
	void Start()
	{
		System.Type colliderType = gameObject.GetComponent<Collider>().GetType();

		if (colliderType == typeof(SphereCollider))
		{
			isWhirlpool = true;
			whirlpool.effectRadius = gameObject.GetComponent<SphereCollider> ().radius;
		}

		if (colliderType == typeof(BoxCollider))
			isCurrent = true;
	}

	// Are we applying whirlpool, current, or both right now?
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
		
		foreach (GameObject thing in thingsInRange)
		{
			// Get a vector going from thing to center of whirlpool
			Vector3 toCentre;
			toCentre = transform.position - thing.transform.position;

			// Find the tangent vector
			Vector3 centreNormal = Vector3.Cross (transform.up, toCentre);
			if (toCentre.magnitude > whirlpool.effectRadius) {
				toCentre.Normalize ();
				toCentre *= whirlpool.effectRadius;
			}

			// Calculate the percentage of the "way into the whirlpool"
			float inPercent = 1 - (toCentre.magnitude / whirlpool.effectRadius);
			toCentre.Normalize (); // Normalize vectors now to simplify calculations
			centreNormal.Normalize ();
			// Add expontial pull force and linear normal force
			Vector3 pullForce = toCentre * inPercent * inPercent * whirlpool.pullStrength;
			Vector3 normalForce = centreNormal * inPercent * whirlpool.rotateStrength;
			Vector3 resultingForce = pullForce + normalForce;

			// Apply force ONCE to parent rigidbody
			Rigidbody[] rigidbodies = thing.GetComponentsInChildren<Rigidbody> ();
			if (rigidbodies.Length > 0) {
				rigidbodies [0].AddForce (resultingForce);
			}
		}
	}
		
	void currentEffect()
	{
		foreach (GameObject thing in thingsInRange) {
			// Only apply ONCE to parent game object
			Rigidbody[] rigidbodies = thing.GetComponentsInChildren<Rigidbody> ();
			if (rigidbodies.Length > 0) {
                
				rigidbodies [0].AddForce (transform.forward *  current.magnitude, ForceMode.Force);
			}
		}
	}
		

	// Keep track of objects in range
	void OnTriggerEnter(Collider other)
	{
		// Add them if they have a rigidbody
		if (other.GetComponentsInChildren<Rigidbody>().Length > 0)
			thingsInRange.Add (other.gameObject);
	}

	// Remove them from effect when they escape trigger volume 
	void OnTriggerExit(Collider other)
	{
		if (thingsInRange.Contains (other.gameObject)) {
			thingsInRange.Remove (other.gameObject);
		}
	}

}
