using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Scriptable Objects/SpawnData")]
public class SpawnData : ScriptableObject
{
    public Fruit Prefab;
    public FruitData Data;
    public SpawnWeightCurve SpawnWeightCurve;
}

