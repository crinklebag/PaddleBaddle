using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GameMode : MonoBehaviour {

    /// <summary>
    /// Optional winCon can be used instead of 
    /// score over time.
    /// </summary>
    public bool winCon = false;

    /// <summary>
    /// Property mode.
    /// Each GameMode MUST implement a mode.get() 
    /// and return a valid mode.
    /// </summary>
    abstract public GameController.Modes mode
    {
        get;
    }

    /// <summary>
    /// init used if this mode requires special setup.
    /// </summary>
    virtual public void init()
    { }

    /// <summary>
    /// Empty protected constructor
    /// to make sure this object is not instantiated.
    /// </summary>
    protected GameMode()
    { }

    /// <summary>
    /// Logic for what happens at the end of the game
    /// </summary>
    virtual public int getWinner(Team[] team, Transform target)
    {
        if (team[0].score > team[1].score)
        {
            Debug.Log("Team 1 won");
            return 0;
        }
        else if (team[1].score > team[0].score)
        {
            Debug.Log("Team 2 won");
            return 1;
        }

        return -1;
    }

    /*
    /// <summary>
    /// Rules for the Flip mode
    /// </summary>
    internal class Flip : GameMode
    {
        public override GameController.Modes mode
        {
            get
            {
                return GameController.Modes.Flip;
            }
        }
    }*/

    /// <summary>
    /// Rules for the Pickup mode
    /// </summary>
    internal class Pickup : GameMode
    {
        public override GameController.Modes mode
        {
            get
            {
                return GameController.Modes.Pickup;
            }
        }
    }

    /// <summary>
    /// Rules for the Race mode
    /// </summary>
    internal class Race : GameMode
    {
        public override GameController.Modes mode
        {
            get
            {
                return GameController.Modes.Race;
            }
        }

        public override int getWinner(Team[] team, Transform target)
        {
            float minDistance = float.MaxValue;
            int winner = -1;

            for (int i = 0; i < team.Length; i++)
            {
                float thisDistance = Vector3.Distance(team[i].boat.transform.position
                    , target.position);

                // closest is winner
                if (thisDistance < minDistance)
                {
                    minDistance = thisDistance;
                    winner = i;
                }
            }

            ++team[winner].score;

            return winner;
        }
    }
}
