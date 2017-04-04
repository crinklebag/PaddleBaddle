using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Boat : MonoBehaviour {

    [SerializeField] int team;
    [SerializeField] Transform flipCheck;
    [SerializeField] TrailRenderer trail;
    [SerializeField] AudioSource characterAudio;

    // Pickup UI Information
    [SerializeField] GameObject fishHook;
    [SerializeField] GameObject strengthIcon;
    [SerializeField] GameObject speedIcon;

    [SerializeField] ParticleSystem boostParticles;

    // Information for powerups
    public bool isFlipped { get; private set; }
    bool hasPowerUp = false;
    string powerUpType = "";
    MeshRenderer meshRenderer;

    public bool invincible { get; private set; }

    private float invincibleTime = 3.0f;

    private float invincibileBlink = 0.2f;

    private GameController.Modes gameMode;

    // Use this for initialization
    void Start()
    {

        // use the reference to set up the buoyancy of the object
        this.GetComponent<RealisticBuoyancy>().setup();
        // hack fix the water level
        this.GetComponent<RealisticBuoyancy>().waterLevelOverride = RealisticWaterPhysics.currentWaterLevel;
        meshRenderer = GetComponent<MeshRenderer>();
        // Check what the gameMode is
        gameMode = GameObject.Find("GameController").GetComponent<GameController>().mode;
    }

    void Update() {

        if (flipCheck.position.y < this.transform.position.y && isFlipped == false)
        {
            isFlipped = true;

            SetPlayerInput(false);
            ControllerInput[] players = GetComponents<ControllerInput>();

            foreach (var player in players)
            {
                player.StartCoroutine("Rumble", 0.5f);
            }

            // Send data to game controller if it's relevant to the gameMode
            if (gameMode == GameController.Modes.Flip)
                Score(1);

            StartCoroutine(Respawn());
        }
        else
        {
            SetPlayerInput(true);
            isFlipped = false;
        }
    }

    void OnCollisionEnter(Collision other) {

        // Debug.Log ("Colliding with: " + other);

        if (other.gameObject.CompareTag("Player")) {
            // Debug.Log("Players Hit");
            this.GetComponent<AudioSource>().Play();
            this.GetComponent<ControllerInput>().RumbleControllers();
            other.gameObject.GetComponent<ControllerInput>().RumbleControllers();
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

    void Score(int points)
    {
        GameObject.Find("GameController").GetComponent<GameController>().AddTeamPoint(team, points);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gold") && gameMode == GameController.Modes.Pickup)
        {
            Score(3);
            Destroy(other.gameObject); // Don't pick up twice
            this.GetComponent<ControllerInput>().StartTaunt();
            Cheer();
        } else if (other.CompareTag("Silver") && gameMode == GameController.Modes.Pickup) {
            Score(2);
            Destroy(other.gameObject); // Don't pick up twice
            this.GetComponent<ControllerInput>().StartTaunt();
            Cheer();
        } else if (other.CompareTag("Wood") && gameMode == GameController.Modes.Pickup) {
            Score(1);
            Destroy(other.gameObject); // Don't pick up twice
            this.GetComponent<ControllerInput>().StartTaunt();
            Cheer();
        }

    }

    void SetPlayerInput(bool value)
    {
        foreach (ControllerInput controller in GetComponentsInChildren<ControllerInput>())
        {
            controller.enabled = value;
        }
    }

    public void PickupObject(string newPowerUpType) {
        hasPowerUp = true;
        powerUpType = newPowerUpType;
        fishHook.SetActive(false);

        this.GetComponent<ControllerInput>().StartTaunt();

        if (powerUpType == "strength")
        {
            strengthIcon.SetActive(true);
        }
        else {
            speedIcon.SetActive(true);
        }
    }

    public void UsePickup() {
        if (speedIcon.activeSelf) { boostParticles.Play(); }
        hasPowerUp = false;
        fishHook.SetActive(true);
        strengthIcon.SetActive(false);
        speedIcon.SetActive(false);
    }

    public bool GetHasPowerUp() {
        return hasPowerUp;
    }

    public string GetPowerUpType() {
        return powerUpType;
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

		// If it's a race - JUST TRYING SOMETHING OUT!!!!
		if (gameMode == GameController.Modes.Pickup)
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

            trail.Clear();

			// Debug.Log ("Player Flipped");
			Score (-1);

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

        trail.Clear();

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

    public void Cheer() {
        characterAudio.Play();
    }
}
