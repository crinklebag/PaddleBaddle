using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Boat : MonoBehaviour {
    
	[SerializeField] int team;
	[SerializeField] Transform flipCheck;
    [SerializeField] TrailRenderer trail;
    
	// Information for powerups
	public bool isFlipped { get; private set; }
	public bool hasPowerUp = false;
	public string powerUpType = "";
   	MeshRenderer meshRenderer;

    public bool invincible { get; private set; }

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
            ControllerInput[] players = GetComponents<ControllerInput>();

            foreach (var player in players)
            {
                player.StartCoroutine("Rumble", 0.5f);
            }

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
        GameObject.Find("GameController").GetComponent<GameController>().AddTeamPoint(team, 1);
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

    static Vector3 GetRandomPointInCollider(SphereCollider collider)
    {
        Vector3 result;

        Vector2 unitCircle = Random.insideUnitCircle;

        result = new Vector3(unitCircle.x, 0, unitCircle.y) * collider.radius;
        result += collider.center + collider.transform.position;

        return result;
    }

    static Vector3 GetRandomPointInCollider(BoxCollider collider)
    {
        Vector3 result;

        Vector2 unitSquare = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

        result = new Vector3(unitSquare.x, 0, unitSquare.y);
        result.Scale(collider.bounds.extents);
        result += collider.center + collider.transform.position;

        return result;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3.0f);

        if (isFlipped == false)
        {
            yield break;
        }

        // If it's a race
        if (gameMode == GameController.Modes.Race)
        {
            // Run respawn code without picking a new position
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.rotation = Quaternion.identity;

            Rigidbody body = GetComponent<Rigidbody>();
            if (body != null)
            {   
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            // GetComponent<TrailRenderer>().Clear();

            StartCoroutine(Invincibility());

            isFlipped = false;
            yield break;
        }

        GameObject respawnArea = GameObject.Find("Respawn Area");

        if (respawnArea == null)
        {
            Debug.LogError("Cannot respawn; no respawn area available!"); yield break;
        }

        Collider[] availableAreas = respawnArea.GetComponents<Collider>();

        Vector3 respawnPoint = Vector3.zero;
        bool excludedPoint = false;

        // do all of this

        do
        {
            excludedPoint = false;

            // choose a random collider from the ones contained in Respawn Area

            int selectedAreaIndex = Random.Range(0, availableAreas.Length);
            Collider selectedArea = availableAreas[selectedAreaIndex];

            // generate a random point depending on the type of collider it is

            if (selectedArea is SphereCollider)
            {
                respawnPoint = GetRandomPointInCollider(selectedArea as SphereCollider);
            }
            else if (selectedArea is BoxCollider)
            {
                respawnPoint = GetRandomPointInCollider(selectedArea as BoxCollider);
            }
            // need to add capsule collider later; a little more tricky

            // get a list of colliders this point (and a safe area around it) overlap with

            Collider[] intersectingColliders = Physics.OverlapSphere(respawnPoint, 1.0f);

            // if the game object for any of these colliders contains Exclude in its name, the point is too close / inside a respawn exclude area

            foreach (Collider col in intersectingColliders)
            {
                excludedPoint = excludedPoint || col.gameObject.name.Contains("Exclude");
            }
        }
        while (excludedPoint == true);

        // until the point no longer intersects with any exclude areas

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
        invincible = true;

        float t = 0.0f;
        while(t < invincibleTime)
        {
            yield return new WaitForSecondsRealtime(invincibileBlink);

            meshRenderer.enabled = !meshRenderer.enabled;

            t += invincibileBlink;
        }
        meshRenderer.enabled = true;

        invincible = false;
    }
}
