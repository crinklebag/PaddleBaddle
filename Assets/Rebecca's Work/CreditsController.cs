using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour {

    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject[] credits;

    int destructionCount = 0;
    int creationCount = 0;
    bool running = true;

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnCredits());
	}
	
	// Update is called once per frame
	void Update () {

        // Check destruction count to see when to load back to the lobby
        if (destructionCount == credits.Length - 1) {
            SceneManager.LoadScene("Lobby");
        }
	}

    IEnumerator SpawnCredits() {
        while (running) {

            GameObject newCredit = Instantiate(credits[creationCount], spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
            newCredit.GetComponent<RealisticBuoyancy>().setup();
            newCredit.GetComponent<RealisticBuoyancy>().waterLevelOverride = RealisticWaterPhysics.currentWaterLevel;

            creationCount++;
            if (creationCount == credits.Length) { running = false; }
            yield return new WaitForSeconds(3);
        }
    }

    public void IncreaseDestructionCount() {
        destructionCount++;
    }
}
