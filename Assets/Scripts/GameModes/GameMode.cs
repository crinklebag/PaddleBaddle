using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GameMode : MonoBehaviour {

    /// <summary>
    /// Optional winCon can be used instead of 
    /// score over time.
    /// </summary>
    public bool winCon;

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
    public void init()
    { }

    /// <summary>
    /// Empty protected constructor
    /// to make sure this object is not instantiated.
    /// </summary>
    protected GameMode()
    { }

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

    internal class Race : GameMode
    {
        public override GameController.Modes mode
        {
            get
            {
                return GameController.Modes.Race;
            }
        }
    }
}
