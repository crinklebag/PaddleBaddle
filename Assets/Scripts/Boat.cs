﻿using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Boat : MonoBehaviour {

	[SerializeField] bool isTeam1;
	[SerializeField] GameObject player1;
	[SerializeField] GameObject player2;
	[SerializeField] Transform flipCheck;

    MeshRenderer meshRenderer;

    bool isFlipped = false;

    public bool Invincible { get; private set; }

    private float invincibleTime = 3.0f;

    private float invincibileBlink = 0.2f;

    // Use this for initialization
	void Start ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

	void Update () {

		if (flipCheck.position.y < this.transform.position.y && !isFlipped)
        {
			isFlipped = true;


            //// Detach players for the funnies
            //if(player1) player1.transform.SetParent (null);
            //if(player2) player2.transform.SetParent (null);

            // Send data to game controller
            if (isTeam1)
            {
				GameObject.Find("GameController").GetComponent<GameController>().AddTeamPoint(1,1);
			}
			else
            {
                GameObject.Find("GameController").GetComponent<GameController>().AddTeamPoint(0,1);
			}
            
            StartCoroutine(Respawn());
		}
	}

    IEnumerator Respawn()
    {
        ControllerInput[] controllers = this.GetComponentsInChildren<ControllerInput>();

        //
        foreach (ControllerInput controller in controllers)
        {
            controller.enabled = false;
        }

        yield return new WaitForSeconds(3);
                
        SphereCollider respawnArea = GameObject.Find("Respawn Area").GetComponent<SphereCollider>();

        Vector3 respawnPoint = Random.insideUnitSphere;
        respawnPoint.Scale(new Vector3(respawnArea.radius, 0, respawnArea.radius));
        respawnPoint += respawnArea.transform.position;

        transform.position = respawnPoint;
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        
        foreach (ControllerInput controller in controllers)
        {
            controller.enabled = true;
        }

        GetComponent<TrailRenderer>().Clear();

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
