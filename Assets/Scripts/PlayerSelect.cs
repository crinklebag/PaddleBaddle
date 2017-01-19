using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour {

    LobbyController gameController;

    [SerializeField] int playerID;
    [SerializeField] Image playerIn;
    [SerializeField] Sprite playerInImage;
    [SerializeField] GameObject inFace;
    [SerializeField] GameObject outFace;

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
            if (outFace.activeSelf)
            {
                inFace.SetActive(true);
                outFace.SetActive(false);
                playerIn.sprite = playerInImage;
                gameController.AddPlayer();
            }
        }
        if (player.GetButtonDown("Back"))
        {
            Debug.Log("Donezo");
            if (inFace.activeSelf)
            {
                inFace.SetActive(false);
                outFace.SetActive(true);
                playerIn.sprite = playerInImage;
                gameController.RemovePlayer();
            }
        }
    }
}
