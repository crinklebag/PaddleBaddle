using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour {

    [SerializeField] Text part1;
    [SerializeField] Text part2;

    int textCount = 0;

	// Use this for initialization
	void Start () {
        StartCoroutine(FadeText());
        StartCoroutine(RunSplash());
	}

    IEnumerator RunSplash() {
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene("Lobby Design");
    }

    IEnumerator FadeText() {
        yield return new WaitForSeconds(2);

        if (textCount == 0) {
            StartCoroutine(FadeIn(part1));
        } else if (textCount == 1) {
            StartCoroutine(FadeIn(part2));
        }

        if (textCount < 1) StartCoroutine(FadeText());
        textCount++;
    }

    IEnumerator FadeIn(Text textPart) {
        textPart.color = new Color(textPart.color.r, textPart.color.g, textPart.color.b, 0);
        while (textPart.color.a < 1.0f) {
            textPart.color = new Color(textPart.color.r, textPart.color.g, textPart.color.b, textPart.color.a + (Time.deltaTime / 2));
            yield return null;
        }
    }
}
