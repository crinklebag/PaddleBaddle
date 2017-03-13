using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine("DelayNextScene");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator DelayNextScene() {
        yield return new WaitForSeconds(10);

        if (PlayerPrefs.GetInt("numPlayers") == 2) {
            SceneManager.LoadScene("Cyclone Zone");
        } else {
            SceneManager.LoadScene("Four Player Battle Arena");
        }
    }
}
