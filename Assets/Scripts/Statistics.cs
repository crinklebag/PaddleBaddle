using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Statistics : MonoBehaviour {
    public GameObject variables;
    public GameObject TeamOnePanel;
    public GameObject TeamTwoPanel;
    public float TeamOneScore;
    public float TeamTwoScore;

    private Player firstPlayer;
    // Use this for initialization
    void Start () {
        firstPlayer = ReInput.players.GetPlayer(0);
        //Select the global game object to use its variables.
        variables = GameObject.FindGameObjectWithTag("Global");

        TeamOneScore = variables.GetComponent<GlobalVariables>().TeamOnePoints;
        TeamTwoScore = variables.GetComponent<GlobalVariables>().TeamTwoPoints;
        //Confirming the variables were passed from the previous scene.
        Debug.Log(TeamOneScore);
        Debug.Log(TeamTwoScore);
        //Start the level with both panels off.
        TeamOnePanel.SetActive(false);
        TeamTwoPanel.SetActive(false);
    }
   
    // Update is called once per frame
    void Update () {
        if (firstPlayer.GetButtonDown("Attack"))
        {
            variables.GetComponent<GlobalVariables>().ResetPoints();
            SceneManager.LoadScene("Lobby Design");
        }
        // if Team 1 has a higher score set panel 1 as active.
        if (TeamOneScore > TeamTwoScore)
        {
            TeamOnePanel.SetActive(true);
            GameObject.Find("End Score Team 1").GetComponent<Text>().text = TeamOneScore + " Points";
            GameObject.Find("End Score Team 2").GetComponent<Text>().text = TeamTwoScore + " Points";
        }
        else
        TeamTwoPanel.SetActive(true);
        GameObject.Find("End Score Team 1").GetComponent<Text>().text = TeamOneScore + " Points";
        GameObject.Find("End Score Team 2").GetComponent<Text>().text = TeamTwoScore + " Points";

        if (TeamTwoScore == TeamOneScore)
        {
            TeamTwoPanel.SetActive(true);
            GameObject.Find("End Score Team 1").GetComponent<Text>().text = TeamOneScore + " Tie Game!";
            GameObject.Find("End Score Team 2").GetComponent<Text>().text = TeamTwoScore + " Tie Game!";
        }
    }
}
