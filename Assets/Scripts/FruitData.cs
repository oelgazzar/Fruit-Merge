using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "Scriptable Objects/FruitData")]
public class FruitData : ScriptableObject
{
    public FruitModel[] Fruits;
}