using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour {
    public GameObject target;
    public float TeamOnePoints;
    public float TeamTwoPoints;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    void Update()
    {
        TeamOnePoints = target.GetComponent<GameController>().TeamOneScore;
        TeamTwoPoints = target.GetComponent<GameController>().TeamTwoScore;
    }
   public void ResetPoints()
    {
        TeamOnePoints = 0;
        TeamTwoPoints = 0;
    }
}
