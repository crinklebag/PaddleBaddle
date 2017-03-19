using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupParticles : MonoBehaviour {
    [SerializeField] GameObject particles;

   void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject newParticles = Instantiate(particles, transform.position, Quaternion.identity) as GameObject;
            Destroy(newParticles, 3);
        }
    }
}
