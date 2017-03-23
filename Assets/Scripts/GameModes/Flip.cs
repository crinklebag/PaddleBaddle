using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flip : GameMode {
    public override GameController.Modes mode
    {
        get
        {
            return GameController.Modes.Flip;
        }
    }

    public Flip()
    { }
}
