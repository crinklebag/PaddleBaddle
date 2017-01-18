using UnityEngine;
using System.Collections;

public class BirdSpawner : MonoBehaviour {
    [HideInInspector]
    public bool spawning = true;

    [SerializeField]
    private GameObject birdPrefab;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float minTime = 0f;
    [SerializeField]
    private float maxTime = 10f;
    [SerializeField]
    private float waitTime = 1f;

    // Use this for initialization
    void Start () {

        StartCoroutine(Spawn());
	}

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(waitTime);
        while(spawning)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
            makeBird();
        }
    }

    void makeBird()
    {
        GameObject bird;
        BirdBehaviour myBird;
        bird = Instantiate(birdPrefab, transform.position, transform.rotation) as GameObject;

        try
        {
            myBird = bird.GetComponent<BirdBehaviour>();
        }
        catch
        {
            Debug.Log("Something went wrong with spawning bird");
            DestroyImmediate(bird);
            return;
        }

        myBird.target = target.position;
    }

    // Draws the line in the editor
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            // Draws a blue line from this transform to the target
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
