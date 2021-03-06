﻿using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	public string type = ""; // stores the key
	// This determines type and is set in the editor for each prefab

	// Something touched me!
	void OnTriggerEnter(Collider other)
	{
		// It was a player
		if (other.CompareTag ("Player"))
		{
			// the player that touched me does not have a powerup
			if (other.GetComponent<Boat> ().hasPowerUp == false) 
			{
				// let the boat know they have a powerup now, of the same type as me
				other.GetComponent<Boat> ().hasPowerUp = true;
				other.GetComponent<Boat> ().powerUpType = type;

				// this is not a trigger any more
				gameObject.GetComponent<Collider> ().isTrigger = false;
				transform.parent = other.transform;

				// attach to the boat
				transform.localPosition = new Vector3 (0, 0, 0.5f);
				transform.localRotation = Quaternion.identity;

				// get rid of physics on this object
				Destroy (gameObject.GetComponent<RealisticBuoyancy> ());
				Destroy (gameObject.GetComponent<Rigidbody> ());
			}
		}
	}
}