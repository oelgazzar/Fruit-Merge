using System;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [SerializeField] Fruit _fruitPrefab;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] FruitData _fruitsData;

    public static event Action<Fruit> OnFruitSpawn;

    private void Start()
    {
        SpawnFruit();
    }

    void SpawnFruit()
    {
        var fruit = Instantiate(_fruitPrefab, _spawnPoint.position, Quaternion.identity);
        fruit.Init(GetRandomFruit());
        OnFruitSpawn?.Invoke(fruit);
    }

    FruitModel GetRandomFruit()
    {
        
        return _fruitsData.Fruits[UnityEngine.Random.Range(0, _fruitsData.Fruits.Length)];
    }

    private void OnEnable()
    {
        FruitController.OnFruitDropped += FruitController_OnFruitDropped;
    }

    private void OnDisable()
    {
        FruitController.OnFruitDropped -= FruitController_OnFruitDropped;        
    }

    private void FruitController_OnFruitDropped()
    {
        SpawnFruit();
    }
}
