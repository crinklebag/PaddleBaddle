using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PaddleData : ScriptableObject 
{

    [Tooltip("How much the paddle pushes the boat forwards. Default is 50000")]
    public float forwardForce = 50000;

    [Tooltip("How much the paddle pushes the boat sideways. Default is 5000")]
    public float rotationForce = 5000;

    [Tooltip("How much the boat turns when the player rotates it. Default is 30000")]
    public float torque = 30000;

    [Tooltip("How many seconds it takes for the player to rotate the paddle. Default is 0.25")]
    public float rotationTime = .25f;

    [Tooltip("How far away from the paddle can the player attack. Default is 1.0")]
    public float reach = 1.0f;

    [Tooltip("How much the paddle flips the opponent. Default is 1000")]
    public float attackForce = -1000f;

    [Tooltip("How much the paddle pushes away the opponent. Default is 10000")]
    public float shoveForce = 10000f;

}
