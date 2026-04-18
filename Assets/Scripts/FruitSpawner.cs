using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [SerializeField] Fruit _fruitPrefab;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] FruitData _fruitsData;
    [SerializeField] SpawnData _spawnData;
    [SerializeField] int _nextFruitQueueSize = 3;

    public static event Action<FruitModel[]> OnNextQueueUpdated;
    public static event Action<Fruit> OnFruitSpawn;

    readonly LinkedList<FruitModel> _nextFruitQueue = new();

    private void Start()
    {
        InitNextFruitQueue();
        SpawnFruit();
    }

    private void InitNextFruitQueue()
    {
        for (var i = 0; i < _nextFruitQueueSize; i++)
        {
            AddNewFruitToNextQueue();
        }
        
        OnNextQueueUpdated?.Invoke(_nextFruitQueue.ToArray());
    }

    void AddNewFruitToNextQueue()
    {
        var fruitModel = GetRandomFruitModel();
        _nextFruitQueue.AddLast(fruitModel);
    }

    FruitModel PullFromNextQueue()
    {
        var fruitModel = _nextFruitQueue.First();
        _nextFruitQueue.RemoveFirst();
        AddNewFruitToNextQueue();
        
        OnNextQueueUpdated?.Invoke(_nextFruitQueue.ToArray());
        return fruitModel;
    }

    void SpawnFruit()
    {
        var fruit = Instantiate(_fruitPrefab, _spawnPoint.position, Quaternion.identity);
        fruit.Init(PullFromNextQueue());
        OnFruitSpawn?.Invoke(fruit);
    }

    FruitModel GetRandomFruitModel()
    {
        var score = ScoreManager.Instance.Score;
        var difficultyScoreRatio = (float) score / _spawnData.SpawnWeightCurve.MaxDifficultyScore;
        
        var tierCount = _spawnData.Data.Fruits.Length;

        List<float> weights = new();

        float totalWeights = 0;
        foreach (var fruitModel in _spawnData.Data.Fruits)
        {
            var tier = fruitModel.Tier;
            var startWeight = _spawnData.SpawnWeightCurve.TierWeights[tier].StartWeight;
            var endWeight = _spawnData.SpawnWeightCurve.TierWeights[tier].EndWeight;
            var calculatedWeight = Mathf.Lerp(startWeight, endWeight, difficultyScoreRatio);
            weights.Add(calculatedWeight);
            totalWeights += calculatedWeight;
        }

        var r = UnityEngine.Random.Range(0, totalWeights);

        for (var i = 0; i < tierCount; i++)
        {
            var calculatedWeight = weights[i];
            if (r < calculatedWeight)
            {
                return _spawnData.Data.Fruits[i];
            }

            r -= calculatedWeight;
        }

        return null;
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
