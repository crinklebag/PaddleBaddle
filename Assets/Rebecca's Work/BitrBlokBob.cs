using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitrBlokBob : MonoBehaviour {

    [SerializeField] float maxMoveDistance = 0.5f;
    [SerializeField] float speed = 200;
    float angle = -90;
    float startHeight;

    void Start() {
        startHeight = this.transform.localPosition.y;
    }

	// Update is called once per frame
	void FixedUpdate () {
        angle += speed * Time.deltaTime;
        if (angle > 270) angle -= 360;
        float destY = startHeight + maxMoveDistance * Mathf.Sin(Mathf.Rad2Deg * angle);
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, destY, this.transform.localPosition.z);
	}
}
