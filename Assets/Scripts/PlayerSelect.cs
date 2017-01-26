using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour {

    LobbyController gameController;
    //[SerializeField] Player Identification Number.
    [SerializeField] int playerID;
    // Game Objects for toggling active & inactive players.
    [SerializeField] GameObject inText;
    [SerializeField] GameObject outText;
    [SerializeField] GameObject inCharacter;
    [SerializeField] GameObject outCharacter;

    Player player;

	// Use this for initialization
	void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<LobbyController>();
        player = ReInput.players.GetPlayer(playerID);
	}
	
	// Update is called once per frame
	void Update () {
        if (player.GetButtonDown("Attack"))
        {
            if (outText.activeSelf)
            {
                inText.SetActive(true);
                outText.SetActive(false);
                inCharacter.SetActive(true);
                outCharacter.SetActive(false);
                gameController.AddPlayer();
            }
        }
        if (player.GetButtonDown("Back"))
        {
            if (inText.activeSelf)
            {
                inText.SetActive(false);
                outText.SetActive(true);
                inCharacter.SetActive(false);
                outCharacter.SetActive(true);
                gameController.RemovePlayer();
            }
        }
    }
}
