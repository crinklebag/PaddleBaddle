using UnityEngine;
using System.Collections;

public class CameraControll : MonoBehaviour {
   [SerializeField] GameObject camera;
   [SerializeField] float yDist = 2.2f;
   [SerializeField] float zDist = 4.5f;
   [SerializeField] float xDist = 0;


	// Use this for initialization
	void Start () {
        camera.transform.parent = this.transform;
        camera.transform.localPosition = new Vector3(xDist, yDist, zDist); 
        // SetCameraLocation();
	}


    Vector3 SetCameraLocation() 
   {

        Vector3 result1 = new Vector3(this.transform.position.x + xDist, this.transform.position.y + yDist, this.transform.position.z + zDist);
        return result1;
    }

    Vector3 SetCameraRotation()
    {


        Vector3 result2 = new Vector3(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z);
        return result2;

    }
}


