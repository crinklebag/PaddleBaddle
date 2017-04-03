using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	public string type = ""; // stores the key
                             // This determines type and is set in the editor for each prefab
     
    private void Update()
    {
        transform.localRotation = Quaternion.identity;
    }
    

	// Something touched me! :(
	void OnTriggerEnter(Collider other)
	{
        // Debug.Log("Colliding with " + other.name);

		// It was a player
		if (other.CompareTag ("Player"))
		{
			// the player that touched me does not have a powerup
			if (other.GetComponent<Boat> ().GetHasPowerUp() == false) 
			{

                other.GetComponent<Boat>().PickupObject(type);

                Destroy(gameObject);
                
			}
		}
	}
}