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

    Animator animator;
    Player player;
    bool inGame = false;

	// Use this for initialization
	void Start () {
        animator = this.GetComponentInChildren<Animator>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<LobbyController>();
        player = ReInput.players.GetPlayer(playerID);
	}
	
	// Update is called once per frame
	void Update () {
        if (gameController.CanSelectCharacter()) {
            if (player.GetButtonDown("Attack") && gameController.CanInput())  {
                if (outText.activeSelf) {
                    EnterGame();
                }
            }
            if (player.GetButtonDown("Shove") && gameController.CanInput()) {
                if (inText.activeSelf) {
                    ExitGame();
                }
            }
        }
    }

    public void EnterGame() {

        animator.SetBool("Selected", true);
        inText.SetActive(true);
        outText.SetActive(false);
        gameController.AddPlayer();
        inGame = true;
    }

    public void ExitGame() {

        animator.SetBool("Selected", false);
        inText.SetActive(false);
        outText.SetActive(true);
        gameController.RemovePlayer();
        inGame = false;
    }

    public bool IsInGame() {
        return inGame;
    }
}
