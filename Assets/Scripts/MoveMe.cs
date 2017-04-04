using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMe : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;

    private Rigidbody rb;

    public void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        rb.velocity = new Vector3(
            Input.GetAxis("Horizontal") * speed,
            0f,
            Input.GetAxis("Vertical") * speed);
    }
}
