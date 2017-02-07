using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceGoal : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<GameController>().GetComponent<GameController>().raceOver = true;
    }
}
