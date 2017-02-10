using UnityEngine;
using System.Collections;

public class LookTowardsCamera : MonoBehaviour
{
    public Camera m_Camera;
    // public bool isActive = false;
    public bool autoInit = false;
    GameObject myContainer;

    void Awake()
    {
        if (autoInit == true)
        {
            m_Camera = Camera.main;
            // isActive = true;
        }

        /*myContainer = new GameObject();
        myContainer.name = "GRP_" + transform.gameObject.name;
        myContainer.transform.position = transform.position;
        myContainer.transform.parent = transform.parent;
        transform.parent = myContainer.transform;*/
    }

    void Update() {
        Debug.Log("Looking at comera");
        if (this.gameObject.activeSelf)
        {
            Debug.Log("Looking at comera");
            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
        }
    }
}
