using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectBoard : MonoBehaviour {

    [SerializeField] Text levelSelectText;

    bool lower = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate() {
        lower = true;
    }

    public void Deactivate() {
        lower = false;
    }

    public void SetText(string level) {
        levelSelectText.text = level;
    }
}
