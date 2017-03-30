using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupParticles : MonoBehaviour {
    [SerializeField] GameObject particles;
    [SerializeField] GameObject plusOneText;
	[SerializeField] GameObject audioClip;

   void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
			Vector3 newPos = new Vector3 (transform.position.x, transform.position.y + 1.5f, transform.position.z);
            GameObject newParticles = Instantiate(particles, transform.position, Quaternion.identity) as GameObject;
            GameObject newPlusOneText = Instantiate(plusOneText, newPos, Quaternion.Euler(0,-90,0)) as GameObject;
			GameObject newAudioClip = Instantiate(audioClip, this.transform.position, Quaternion.identity) as GameObject;
            Destroy(newParticles, 3);
            Destroy(newPlusOneText, 1);
			Destroy (newAudioClip, 2);
        }
    }
}
