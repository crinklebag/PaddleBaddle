using UnityEngine;
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

    public static Vector3 RandomPointInSphere(SphereCollider sphere)
    {
        Vector3 sLocalScale = sphere.transform.localScale;
        Vector3 spherePosition = sphere.transform.position;
        spherePosition += new Vector3(sLocalScale.x * sphere.center.x, 0, sLocalScale.z * sphere.center.z);

        Vector3 dimensions = new Vector3(sLocalScale.x * sphere.radius,
        0,
        sLocalScale.z * sphere.radius);

        Vector3 newPos = new Vector3(UnityEngine.Random.Range(spherePosition.x - (dimensions.x / 2), spherePosition.x + (dimensions.x / 2)),
        spherePosition.y,
        UnityEngine.Random.Range(spherePosition.z - (dimensions.z / 2), spherePosition.z + (dimensions.z / 2)));
        return newPos;
    }

    public static Vector3 RandomPointInCapsule(CapsuleCollider capsule)
    {
        Vector3 cLocalScale = capsule.transform.localScale;
        Vector3 cpherePosition = capsule.transform.position;
        cpherePosition += new Vector3(cLocalScale.x * capsule.center.x, 0, cLocalScale.z * capsule.center.z);

        Vector3 dimensions = new Vector3(cLocalScale.x * capsule.radius,
        0,
        cLocalScale.z * capsule.radius);

        Vector3 newPos = new Vector3(UnityEngine.Random.Range(cpherePosition.x - (dimensions.x / 2), cpherePosition.x + (dimensions.x / 2)),
        cpherePosition.y,
        UnityEngine.Random.Range(cpherePosition.z - (dimensions.z / 2), cpherePosition.z + (dimensions.z / 2)));
        return newPos;
    }

    public static Vector3 RandomPointInBox(BoxCollider box)
    {
        Vector3 bLocalScale = box.transform.localScale;
        Vector3 boxPosition = box.transform.position;
        boxPosition += new Vector3(bLocalScale.x * box.center.x, 0, bLocalScale.z * box.center.z);

        Vector3 dimensions = new Vector3(bLocalScale.x * box.size.x,
        0,
        bLocalScale.z * box.size.z);

        Vector3 newPos = new Vector3(UnityEngine.Random.Range(boxPosition.x - (dimensions.x / 2), boxPosition.x + (dimensions.x / 2)),
        boxPosition.y,
        UnityEngine.Random.Range(boxPosition.z - (dimensions.z / 2), boxPosition.z + (dimensions.z / 2)));
        return newPos;
    }

    public Vector3 GetRespawnPoint(GameObject respawnAreaObject)
    {
        Vector3 respawnPoint = Vector3.zero;

        bool excluded = false;

        Collider[] allRespawnAreas = respawnAreaObject.GetComponents<Collider>();

        int selectedAreaIndex = 0;

        do
        {
            Debug.Log("Respawn attempt");

            excluded = false;

            selectedAreaIndex = Random.Range(0, allRespawnAreas.Length);

            Collider selectedArea = allRespawnAreas[selectedAreaIndex];
            
            if (selectedArea is SphereCollider)
            {
                respawnPoint = RandomPointInSphere(selectedArea as SphereCollider);
            }
            else if (selectedArea is BoxCollider)
            {
                respawnPoint = RandomPointInBox(selectedArea as BoxCollider);
            }
            else if (selectedArea is CapsuleCollider)
            {
                respawnPoint = RandomPointInCapsule(selectedArea as CapsuleCollider);
            }

            Collider[] overlapping = Physics.OverlapSphere(respawnPoint, 0.5f, 0, QueryTriggerInteraction.Collide);

            foreach(Collider col in overlapping)
            {
                if (col.gameObject.name.Contains("Exclude"))
                {
                    excluded = true; break;
                }
            }
        }
        while (excluded == true);

        Debug.Log("Spawning in area " + selectedAreaIndex + " of " + (allRespawnAreas.Length-1));

        return respawnPoint;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3);

        if(isFlipped == false)
        {
            yield break;
        }

        GameObject respawnObject = GameObject.Find("Respawn Area");

        if (respawnObject != null)
        {
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = GetRespawnPoint(respawnObject);
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
        else
        {
            Debug.Log("FATAL ERROR: Can't respawn; no respawn area found!");
        }
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
