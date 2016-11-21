using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	public string type;

	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Do the pickup thing");
	}
}