using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobandRotate : MonoBehaviour {

    [SerializeField] float rotateamount;
    [SerializeField] float bobSpeed;
    [SerializeField] Transform target;
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;
    [SerializeField] Vector3 moveTo;
    [SerializeField] Vector3 offset = Vector3.down;

	// Use this for initialization
	void Start () {

       
        pos1 = transform.position;
        pos2 = transform.position + offset;
    

}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Rotate(Vector3.up, rotateamount);

        if (transform.position == pos1)
        {
            moveTo = pos2;
        }
        if (transform.position == pos2)
        {
            moveTo = pos1;
        }

        transform.position = Vector3.MoveTowards(transform.position, moveTo, bobSpeed);

        //transform.position = Vector3.MoveTowards(transform.position, target.position, bobSpeed * Time.deltaTime); 
        //(transform.position.x, transform.position.y + (Mathf.Sin(Time.time)*bobSpeed), transform.position.z);
	}
}
