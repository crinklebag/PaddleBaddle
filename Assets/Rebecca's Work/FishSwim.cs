using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FishSwim : MonoBehaviour {

    float counter = 0;
    [SerializeField] float speed = 1;
    [SerializeField] float width = 2;
    [SerializeField] float height = 2;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        counter += Time.deltaTime * speed;

        float x = Mathf.Cos(counter) * width;
        float y = this.transform.localPosition.y;
        float z = Mathf.Sin(counter) * width;

        this.GetComponent<Rigidbody>().MovePosition(this.transform.position + new Vector3(x, y, z) * Time.deltaTime);
	}
}
