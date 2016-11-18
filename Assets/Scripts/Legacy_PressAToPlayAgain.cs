using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Legacy_PressAToPlayAgain : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    void PlayAgain()
    {
        SceneManager.LoadScene("Character Select");
    }

}
