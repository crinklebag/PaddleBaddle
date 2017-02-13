﻿using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Boat : MonoBehaviour {

	[SerializeField] bool isTeam1;
	[SerializeField] GameObject player1;
	[SerializeField] GameObject player2;
	[SerializeField] Transform flipCheck;
    [SerializeField] TrailRenderer trail;
    
	// Information for powerups
	bool isFlipped = false;
	public bool hasPowerUp = false;
	public string powerUpType = "";
   	MeshRenderer meshRenderer;

    public bool Invincible { get; private set; }

    private float invincibleTime = 3.0f;

    private float invincibileBlink = 0.2f;

    private GameController.Modes gameMode;

    // Use this for initialization
	void Start ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        // Check what the gameMode is
        gameMode = GameObject.Find("GameController").GetComponent<GameController>().mode;
    }

	void Update () {

		if (flipCheck.position.y < this.transform.position.y && isFlipped == false)
        {
			isFlipped = true;

            SetPlayerInput(false);

            //// Detach players for the funnies
            //if(player1) player1.transform.SetParent (null);
            //if(player2) player2.transform.SetParent (null);

            // Send data to game controller if it's relevant to the gameMode
            if (gameMode == GameController.Modes.Flip)
                Score();
            
            StartCoroutine(Respawn());
		}
        else
        {
            SetPlayerInput(true);
            isFlipped = false;
        }
	}

    public void FlipBoat()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        { 
            rb.AddForceAtPosition(Vector3.down * 175, transform.right, ForceMode.Impulse);
            
        }
    }

    void Score()
    {
        if (isTeam1)
        {
            GameObject.Find("GameController").GetComponent<GameController>().AddTeamPoint(1, 1);
        }
        else
        {
            GameObject.Find("GameController").GetComponent<GameController>().AddTeamPoint(0, 1);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin") && gameMode == GameController.Modes.Pickup)
        {
            Score();
            Destroy(other.gameObject); // Don't pick up twice
        }
            
    }

    void SetPlayerInput(bool value)
    {
        foreach (ControllerInput controller in GetComponentsInChildren<ControllerInput>())
        {
            controller.enabled = value;
        }
    }


    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3);

        if(isFlipped == false)
        {
            yield break;
        }
                
        SphereCollider respawnArea = GameObject.Find("Respawn Area").GetComponent<SphereCollider>();

        Vector3 respawnPoint = Random.insideUnitSphere;
        respawnPoint.Scale(new Vector3(respawnArea.radius, 0, respawnArea.radius));
        respawnPoint += respawnArea.transform.position;

        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = respawnPoint;
        transform.rotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // GetComponent<TrailRenderer>().Clear();

        StartCoroutine(Invincibility());

        isFlipped = false;
    }

    IEnumerator Invincibility()
    {
        Invincible = true;

        float t = 0.0f;
        while(t < invincibleTime)
        {
            yield return new WaitForSecondsRealtime(invincibileBlink);

            meshRenderer.enabled = !meshRenderer.enabled;

            t += invincibileBlink;
        }
        meshRenderer.enabled = true;

        Invincible = false;
    }
}
