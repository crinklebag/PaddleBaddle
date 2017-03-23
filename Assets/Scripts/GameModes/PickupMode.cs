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
            return false;
        }
    }

    public override IEnumerator CR
    {
        get
        {
            return coinSpawner;
        }
    }

    GameController GC;
    private IEnumerator coinSpawner;
    GameObject coinPrefab;
    float spawnRate = 3f;
    SphereCollider respawnArea;

    public override void init(MonoBehaviour _GC)
    {
        GC = _GC.GetComponent<GameController>();

        coinSpawner = SpawnCoins();
        coinPrefab = GC.coinPrefab;
        spawnRate = GC.spawnRate;
        respawnArea = GC.respawnArea;

        GC.runCR(coinSpawner);
    }



    IEnumerator SpawnCoins()
    {
        while (true)
        {
            // For simplicity we'll use Respawn area for now
            Vector3 spawnPoint = UnityEngine.Random.insideUnitSphere;
            spawnPoint.Scale(new Vector3(respawnArea.radius, 0, respawnArea.radius));
            spawnPoint += respawnArea.transform.position;
            GameObject thisPickup = Object.Instantiate(coinPrefab, spawnPoint, Quaternion.identity);

            // use the reference to set up the buoyancy of the object
            thisPickup.GetComponent<RealisticBuoyancy>().setup();
            // hack fix the water level
            thisPickup.GetComponent<RealisticBuoyancy>().waterLevelOverride = RealisticWaterPhysics.currentWaterLevel;

            yield return new WaitForSeconds(spawnRate);
        }
    }
}
