using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMode : GameMode {

    public override GameController.Modes mode
    {
        get
        {
            return GameController.Modes.Pickup;
        }
    }
}
