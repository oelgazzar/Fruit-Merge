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
    [SerializeField] int _queueSize = 4;

    public static event Action<FruitModel[]> OnNextQueueUpdated;
    public static event Action<Fruit> OnFruitSpawn;

    public static FruitSpawner Instance { get; private set; }
    public FruitModel[] Queue => _fruitQueue.ToArray();

    // This include both active and next n fruits
    readonly LinkedList<FruitModel> _fruitQueue = new();
    FruitModel[] NextQueue => _fruitQueue.ToArray()[1.._queueSize];

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitFruitQueue();
        SpawnFruit(_fruitQueue.First());
    }

    public void RestoreQueue(FruitModel[] queue)
    {
        _fruitQueue.Clear();

        for (var i = 0; i < queue.Length; i++)
        {
            PushToQueue(queue[i]);
        }

        SpawnFruit(_fruitQueue.First());
        OnNextQueueUpdated?.Invoke(NextQueue);
    }


    private void InitFruitQueue()
    {
        for (var i = 0; i < _queueSize; i++)
        {
            PushToQueue(GetRandomFruitModel());
        }

        OnNextQueueUpdated?.Invoke(NextQueue);
    }

    void PushToQueue(FruitModel fruitModel)
    {
        _fruitQueue.AddLast(fruitModel);
    }

    void AdvanceQueue()
    {
        _fruitQueue.RemoveFirst();
        PushToQueue(GetRandomFruitModel());

        OnNextQueueUpdated?.Invoke(NextQueue);
    }

    void SpawnFruit(FruitModel fruitModel)
    {
        var fruit = Instantiate(_fruitPrefab, _spawnPoint.position, Quaternion.identity);
        fruit.Init(fruitModel);
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
        FruitController.OnFruitDropCompleted += FruitController_OnFruitDropped;
    }

    private void OnDisable()
    {
        FruitController.OnFruitDropCompleted -= FruitController_OnFruitDropped;        
    }

    private void FruitController_OnFruitDropped(Fruit _)
    {
        AdvanceQueue();
        SpawnFruit(_fruitQueue.First());
    }
}
