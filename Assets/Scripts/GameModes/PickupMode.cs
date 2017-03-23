using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMode : GameMode {
    public override GameController.Modes mode
    {
        get
        {
            return GameController.Modes.Pickup;
        }
    }

    public override bool hasCoroutine
    {
        get
        {
            return true;
        }
    }

    public override IEnumerator CR
    {
        get
        {
            return coinSpawner;
        }
    }

    private IEnumerator coinSpawner;
    GameObject coinPrefab;
    float spawnRate = 3f;

    SphereCollider respawnArea;

    public override void init(MonoBehaviour GC)
    {
        coinSpawner = SpawnCoins();
        coinPrefab = GC.GetComponent<GameController>().coinPrefab;
        spawnRate = GC.GetComponent<GameController>().spawnRate;
        respawnArea = GC.GetComponent<GameController>().respawnArea;
    }



    IEnumerator SpawnCoins()
    {
        while (true)
        {
            // For simplicity we'll use Respawn area for now
            Vector3 spawnPoint = UnityEngine.Random.insideUnitSphere;
            spawnPoint.Scale(new Vector3(respawnArea.radius, 0, respawnArea.radius));
            spawnPoint += respawnArea.transform.position;
            Object.Instantiate(coinPrefab, spawnPoint, Quaternion.identity);

            yield return new WaitForSeconds(spawnRate);
        }
    }
}
