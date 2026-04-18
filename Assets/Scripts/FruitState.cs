using System;
using UnityEngine;

[Serializable]
public class FruitState
{
    public FruitModel Model;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Velocity;
    public float AngularVelocity;
    public bool IsSleeping;
}