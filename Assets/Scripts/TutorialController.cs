using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour {

    [SerializeField]
    string target2 = "Two Player Battle Arena";
    [SerializeField]
    string target4 = "Four Player Battle Arena";

	// Use this for initialization
	void Start () {
        StartCoroutine("DelayNextScene");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator DelayNextScene() {
        yield return new WaitForSeconds(20);

        if (PlayerPrefs.GetInt("numPlayers") == 2) {
            SceneManager.LoadScene(target2);
        } else {
            SceneManager.LoadScene(target4);
        }
    }
}
