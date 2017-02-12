using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;


public class FadeToBlack : MonoBehaviour {

    [SerializeField] Image cameraOverlay;
    [SerializeField] float fadeSpeed = 0.1f;

	// Use this for initialization
	void Start () {
        StartCoroutine(FadeScreen());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator FadeScreen()
    {

        yield return new WaitForSeconds(6);

        // Fade out
        while (cameraOverlay.color.a < 1)
        {

            float newAlpha = cameraOverlay.color.a + fadeSpeed * Time.deltaTime;
            cameraOverlay.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
    }

}
