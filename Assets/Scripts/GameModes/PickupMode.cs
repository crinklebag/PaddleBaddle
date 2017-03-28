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
    GameObject respawnArea;

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
            Collider[] availableAreas = respawnArea.GetComponents<Collider>();

            Vector3 spawnPoint = Vector3.zero;
            bool excludedPoint = false;

            ////////// do all of this

            do
            {
                excludedPoint = false;

                // choose a random collider from the ones contained in Respawn Area

                int selectedAreaIndex = Random.Range(0, availableAreas.Length);
                Collider selectedArea = availableAreas[selectedAreaIndex];

                // generate a random point depending on the type of collider it is

                if (selectedArea is SphereCollider)
                {
                    spawnPoint = GetRandomPointInCollider(selectedArea as SphereCollider);
                }
                else if (selectedArea is BoxCollider)
                {
                    spawnPoint = GetRandomPointInCollider(selectedArea as BoxCollider);
                }
                // need to add capsule collider later; a little more tricky

                // get a list of colliders this point (and a safe area around it) overlap with

                Collider[] intersectingColliders = Physics.OverlapSphere(spawnPoint, 1.0f);

                // if the game object for any of these colliders contains Exclude in its name, the point is too close / inside a respawn exclude area

                foreach (Collider col in intersectingColliders)
                {
                    excludedPoint = excludedPoint || col.gameObject.name.Contains("Exclude");
                }
            }
            while (excludedPoint == true);

            ////////// until the point no longer intersects with any exclude areas

            //spawnPoint.Scale(new Vector3(respawnArea.radius, 0, respawnArea.radius));
            //spawnPoint += respawnArea.transform.position;
            GameObject thisPickup = Object.Instantiate(coinPrefab, spawnPoint, Quaternion.identity);

            // use the reference to set up the buoyancy of the object
            thisPickup.GetComponent<RealisticBuoyancy>().setup();
            // hack fix the water level
            thisPickup.GetComponent<RealisticBuoyancy>().waterLevelOverride = RealisticWaterPhysics.currentWaterLevel;

            yield return new WaitForSeconds(spawnRate);
        }
    }

    public Vector3 GetRandomPointInCollider(SphereCollider collider)
    {
        Vector3 result;

        Vector2 unitCircle = Random.insideUnitCircle;

        result = new Vector3(unitCircle.x, 0, unitCircle.y) * collider.radius;
        result += collider.center + collider.transform.position;

        return result;
    }

    public Vector3 GetRandomPointInCollider(BoxCollider collider)
    {
        Vector3 result;

        Vector2 unitSquare = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

        result = new Vector3(unitSquare.x, 0, unitSquare.y);
        result.Scale(collider.bounds.extents);
        result += collider.center + collider.transform.position;

        return result;
    }
}
