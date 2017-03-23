using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipMode : GameMode {
    public override GameController.Modes mode
    {
        get
        {
            return GameController.Modes.Flip;
        }
    }
}
