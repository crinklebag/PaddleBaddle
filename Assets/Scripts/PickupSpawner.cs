using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupSpawner : MonoBehaviour {

	GameObject pickupPrefab;
	public bool running = true;
	[SerializeField] private float spawnRateLowerBound = 5f;
	[SerializeField] private float spawnRateUpperBound = 10f;
	[SerializeField] private float timeBeforeStart = 5f;
	[SerializeField] private List<GameObject> powerups;
	private bool nothingInTheWay = true;

	// Turn it on
	void Start()
	{
		Debug.Log ("Spawner is alive");
		StartCoroutine (spawning());
	}

	// Make sure it doesn't spawn if things are on top of the spawner
	void OnTriggerEnter(){
		nothingInTheWay = false;
	}

	// Turn on spawning again when the space above the spawner is clear
	void OnTriggerExit(){
		nothingInTheWay = true;
	}

	// Spawning corouting
	IEnumerator spawning()
	{
		Debug.Log ("Waiting to start for: " + timeBeforeStart);
		yield return new WaitForSeconds (timeBeforeStart); // wait this long before turning on
		// while its on, keep doing this
		while (running) {
			GameObject thisPickup;

			// as long as nothing is in the way
			if (nothingInTheWay) {
				// if no powerups were set for the spawner, turn this off
				if (powerups.Count == 0) { 
					running = false;
					yield break;
				} else if (powerups.Count > 1) {
					// spawn a random pickup, and get a reference to it
					thisPickup = Instantiate (powerups [(int)(Random.value * 10) % powerups.Count],
						transform.position, transform.rotation) as GameObject;
				} else {
					thisPickup = Instantiate (powerups [0], transform.position, transform.rotation) as GameObject;
				}

				// use the reference to set up the buoyancy of the object
				thisPickup.GetComponent<RealisticBuoyancy> ().setup ();
				// hack fix the water level
				thisPickup.GetComponent<RealisticBuoyancy> ().waterLevelOverride = RealisticWaterPhysics.currentWaterLevel;
			}
			// wait a random time and spawn again
			yield return new WaitForSeconds(Random.Range (spawnRateLowerBound, spawnRateUpperBound));
		}
	}
}
