using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "Scriptable Objects/FruitData")]
public class FruitData : ScriptableObject
{
    public FruitModel[] Fruits;
}

[Serializable]
public class FruitModel
{
    public int Type;
    public Color Color;
    public Sprite Sprite;
    public float Size = 1f;
}
