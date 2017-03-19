using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupParticles : MonoBehaviour {
    [SerializeField] GameObject particles;
    [SerializeField] GameObject plusOneText;

   void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject newParticles = Instantiate(particles, transform.position, Quaternion.identity) as GameObject;
            GameObject newPlusOneText = Instantiate(plusOneText, transform.position, Quaternion.Euler(0,-90,0)) as GameObject;
            Destroy(newParticles, 3);
            Destroy(newPlusOneText, 1);
        }
    }
}
