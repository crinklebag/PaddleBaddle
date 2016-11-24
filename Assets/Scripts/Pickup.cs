using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	public string type = "";

	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Do the pickup thing");
		if (other.CompareTag ("Player"))
		{
			other.GetComponent<Boat> ().hasPowerUp = true;
			other.GetComponent<Boat> ().powerUpType = type;

			gameObject.GetComponent<Collider> ().isTrigger = false;
			transform.parent = other.transform;

			transform.localPosition = new Vector3 (0, 0, 0.5f);
			transform.localRotation = Quaternion.identity;

			Destroy (gameObject.GetComponent<RealisticBuoyancy> ());
			Destroy(gameObject.GetComponent<Rigidbody>());
		}
	}
}