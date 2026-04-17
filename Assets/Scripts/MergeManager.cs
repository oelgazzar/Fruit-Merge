using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [SerializeField] Fruit _fruitPrefab;
    [SerializeField] FruitData _fruits;

    public static event Action OnFruitMerge;

    readonly Dictionary<int, FruitModel> _fruitLookUp = new();

    private void Start()
    {
        foreach (var fruitModel in _fruits.Fruits)
        {
            _fruitLookUp[fruitModel.Type] = fruitModel;
        }
    }

    private void OnEnable()
    {
        Fruit.OnFruitContact += Fruit_OnFruitMerg;
    }

    private void OnDisable()
    {
        Fruit.OnFruitContact -= Fruit_OnFruitMerg;
    }

    private void Fruit_OnFruitMerg(Fruit fruit1, Fruit fruit2)
    {
        // Guard against duplicates
        if (fruit1.IsMerged || fruit2.IsMerged) return;

        var newType = fruit1.Type + 1;
        if (_fruitLookUp.TryGetValue(newType, out var fruitModel))
        {
            var fruit = Instantiate(_fruitPrefab, fruit1.transform.position, Quaternion.identity);
            fruit.Init(fruitModel);
            fruit.GetComponent<Rigidbody2D>().simulated = true;
        }
        Destroy(fruit1.gameObject);
        Destroy(fruit2.gameObject);
        
        OnFruitMerge?.Invoke();
    }
}
