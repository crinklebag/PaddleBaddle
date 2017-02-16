using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;

public class GameModeSelect : MonoBehaviour
{
    //[SerializeField] Player Identification Number.
    [SerializeField]
    int playerID;

    [SerializeField]
    GameObject RaceMode;
    [SerializeField]
    GameObject BattleMode;
    Player player;

    // Use this for initialization
    void Start()
    {
        RaceMode.SetActive(true);
        BattleMode.SetActive(false);
        player = ReInput.players.GetPlayer(playerID);
    }
    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("Attack"))
        {
            if (RaceMode.activeSelf)
            {
                SceneManager.LoadScene("Four Player Battle Arena");
            }
            else
            {
                SceneManager.LoadScene("Four Player Battle Arena");
            }
        }
        if (player.GetButtonDown("Left"))
        {
            Debug.Log("Something");
            if (RaceMode.activeSelf == true)
            {
                RaceMode.SetActive(false);
                BattleMode.SetActive(true);
            }
            else
            {
                RaceMode.SetActive(true);
                BattleMode.SetActive(false);
            }
        }
        if (player.GetButtonDown("Right"))
        {
            if (RaceMode.activeSelf == true)
            {
                RaceMode.SetActive(false);
                BattleMode.SetActive(true);
            }
            else
            {
                RaceMode.SetActive(true);
                BattleMode.SetActive(false);
            }
        }
    }
}
