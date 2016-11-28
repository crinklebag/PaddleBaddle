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

	void Start()
	{
		Debug.Log ("Spawner is alive");
		StartCoroutine (spawning());
	}

	void OnTriggerEnter(){
		nothingInTheWay = false;
	}

	void OnTriggerExit(){
		nothingInTheWay = true;
	}
	
	IEnumerator spawning()
	{
		Debug.Log ("Waiting to start for: " + timeBeforeStart);
		yield return new WaitForSeconds (timeBeforeStart);
		while (running) {
			GameObject thisPickup;

			if (nothingInTheWay) { 
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
				thisPickup.GetComponent<RealisticBuoyancy> ().waterLevelOverride = RealisticWaterPhysics.currentWaterLevel;
			}

			yield return new WaitForSeconds(Random.Range (spawnRateLowerBound, spawnRateUpperBound));
		}
	}
}
