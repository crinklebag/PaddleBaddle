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
    /// Logic for a game that can end 
    /// before time runs out
    /// </summary>
    virtual public void earlyWin()
    { }

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
    }

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

        public override void earlyWin()
        {

        }
    }
}
