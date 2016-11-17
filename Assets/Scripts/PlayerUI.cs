using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    
    [SerializeField] Image playerExpression;
    [SerializeField] Sprite[] expressions;

    Vector2 startPosition;

	// Use this for initialization
	void Start () {
        startPosition = playerExpression.GetComponent<RectTransform>().localPosition;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetSpeed(float newSpeed) {
        // playerSpeed.text = newSpeed.ToString();
        Debug.Log("New Speed" + (int)newSpeed);
        switch ((int)newSpeed) {
            case 0:
                // size: 17 x 6.5
                // pos: -35, 0
                SetExpression((int)newSpeed, new Vector2(17, 7.5f), startPosition);
                break;
            case 2:
                SetExpression(0, new Vector2(17, 7.5f), startPosition);
                break;
            case 4:
                SetExpression(((int)newSpeed - 2) / 2, new Vector2(17, 7.5f), startPosition);
                break;
            case 6:
                // size: 25 x 7
                // pos: -38, 0, 0
                SetExpression(((int)newSpeed - 2) / 2, new Vector2(27, 9.5f), new Vector2(startPosition.x - 4, startPosition.y));
                break;
            case 8:
                SetExpression(((int)newSpeed - 2) / 2, new Vector2(27, 9.5f), new Vector2(startPosition.x - 4, startPosition.y));
                break;
            case 10:
                SetExpression(((int)newSpeed - 2) / 2, new Vector2(27, 9.5f), new Vector2(startPosition.x - 4, startPosition.y));
                break;
        }
    }

    void SetExpression(int index, Vector2 newSize, Vector2 newPosition) {
        playerExpression.GetComponent<RectTransform>().sizeDelta = newSize;
        playerExpression.GetComponent<RectTransform>().localPosition = newPosition;
        playerExpression.sprite = expressions[index];
    }
}
