using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupSpawner : MonoBehaviour {

	GameObject pickupPrefab;
	public bool running = true;
	[SerializeField] private int numPickups = 2;
	[SerializeField] private float spawnRateLowerBound = 5f;
	[SerializeField] private float spawnRateUpperBound = 10f;
	[SerializeField] private float timeBeforeStart = 5f;
	[SerializeField] private List<GameObject> powerups;

	void Start()
	{
		Debug.Log ("Spawner is alive");
		StartCoroutine (spawning());
	}
	
	IEnumerator spawning()
	{
		Debug.Log ("Waiting to start for: " + timeBeforeStart);
		yield return new WaitForSeconds (timeBeforeStart);
		while (running) {
			GameObject thisPickup;

			if (powerups.Count == 0) { 
				running = false;
				return false;
			} else if (powerups.Count > 1) {
				thisPickup = Instantiate (powerups [(int)(Random.value * 10) % powerups.Count],
					transform.position, transform.rotation) as GameObject;
			} else {
				thisPickup = Instantiate (powerups [0], transform.position, transform.rotation) as GameObject;
			}

			thisPickup.GetComponent<RealisticBuoyancy> ().setup ();

			Debug.Break ();

			yield return new WaitForSeconds(Random.Range (spawnRateLowerBound, spawnRateUpperBound));
		}
	}
}
