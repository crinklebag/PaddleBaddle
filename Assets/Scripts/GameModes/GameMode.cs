using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GameMode {

    /// <summary>
    /// Optional winCon can be used instead of 
    /// score over time.
    /// </summary>
    public bool winCon = false;
    
    virtual public bool hasCoroutine
    {
        get { return false; }
    }

    virtual public IEnumerator CR
    {
        get { return null; }
    }

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
    virtual public void init(MonoBehaviour GC)
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
}
