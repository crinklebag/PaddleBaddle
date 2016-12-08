using UnityEngine;
using System.Collections;

public class UIDropDown : MonoBehaviour {

    [SerializeField] Vector2 endPoint = Vector2.zero;

	void Update ()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, endPoint, 0.1f);
	}
}
