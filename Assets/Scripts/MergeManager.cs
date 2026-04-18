using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [SerializeField] Fruit _fruitPrefab;
    [SerializeField] FruitData _fruits;

    public static event Action<Fruit, Fruit, Fruit> OnFruitMerge;

    readonly Dictionary<int, FruitModel> _fruitLookUp = new();

    private void Start()
    {
        foreach (var fruitModel in _fruits.Data)
        {
            _fruitLookUp[fruitModel.Tier] = fruitModel;
        }
    }

    private void Merge(Fruit fruit1, Fruit fruit2)
    {
        // Guard against duplicates
        if (fruit1.IsMerged || fruit2.IsMerged) return;

        var nextTier = fruit1.Model.Tier + 1;
        Fruit newFruit = null;

        if (_fruitLookUp.TryGetValue(nextTier, out var fruitModel))
        {
            newFruit = Instantiate(_fruitPrefab, fruit1.transform.position, Quaternion.identity);
            newFruit.Init(fruitModel);
            newFruit.GetComponent<Rigidbody2D>().simulated = true;
        }

        fruit1.MarkMerged();
        fruit2.MarkMerged();

        Destroy(fruit1.gameObject);
        Destroy(fruit2.gameObject);
        
        OnFruitMerge?.Invoke(fruit1, fruit2, newFruit);
    }
    private void OnEnable()
    {
        Fruit.OnFruitContact += Merge;
    }

    private void OnDisable()
    {
        Fruit.OnFruitContact -= Merge;
    }
}
