using UnityEngine;
using System.Collections;

public class BirdBehaviour : MonoBehaviour {
    public float speed = 1f;

    [HideInInspector]
    public Vector3 target;

    private Vector3 startPos;
    private float startTime;
    private float journeyLength;
    private float attackDist;
    private bool needsAttack = true;
    private const float tolerance = 0.1f;

    void Start()
    {
        startTime = Time.time;
        startPos = transform.position;
        journeyLength = Vector3.Distance(startPos, target);
        attackDist = Random.Range(0.1f, (journeyLength - 0.1f));
    }

    void Update()
    {
        if ((Vector3.Distance(transform.position, target) < attackDist) && needsAttack)
        {
            attack();
        }

        if (Vector3.Distance(transform.position, target) > tolerance)
        {
            fly();
        }
        else
        {
            arrive();
        }
    }

    void fly()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPos, target, fracJourney);
    }

    void arrive()
    {
        Destroy(gameObject);
    }

    void attack()
    {
        needsAttack = false;
        Debug.Log("Attacking!");
    }
}
