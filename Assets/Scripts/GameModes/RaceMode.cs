using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceMode : GameMode {
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
