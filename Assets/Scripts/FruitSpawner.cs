using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Spawn Fruits
 * Manage Fruit Queue
 */

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

    // This include both active and next n fruits
    readonly LinkedList<FruitModel> _fruitQueue = new();
    public FruitModel[] Queue => _fruitQueue.ToArray();
    FruitModel[] NextQueue => _fruitQueue.ToArray()[1.._queueSize];

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        InitFruitQueue();
        SpawnFruit(_fruitQueue.First());
    }

    private void InitFruitQueue()
    {
        for (var i = 0; i < _queueSize; i++)
        {
            _fruitQueue.AddLast(GetRandomFruitModel());
        }

        OnNextQueueUpdated?.Invoke(NextQueue);
    }

    void SpawnFruit(FruitModel fruitModel)
    {
        var fruit = Instantiate(_fruitPrefab, _spawnPoint.position, Quaternion.identity);
        fruit.Init(fruitModel);
        
        // Translate up half height to align with spawn point
        fruit.transform.Translate(Vector2.up * fruitModel.Size / 2);

        OnFruitSpawn?.Invoke(fruit);
    }

    public void RestoreQueue(FruitModel[] queue)
    {
        _fruitQueue.Clear();

        for (var i = 0; i < queue.Length; i++)
        {
            _fruitQueue.AddLast(queue[i]);
        }

        SpawnFruit(_fruitQueue.First());
        OnNextQueueUpdated?.Invoke(NextQueue);
    }    

    void AdvanceQueue()
    {
        _fruitQueue.RemoveFirst();
        _fruitQueue.AddLast(GetRandomFruitModel());

        OnNextQueueUpdated?.Invoke(NextQueue);
    }   

    FruitModel GetRandomFruitModel()
    {
        var score = ScoreManager.Instance.Score;
        var difficultyScoreRatio = (float) score / _spawnData.SpawnWeightCurve.MaxDifficultyScore;
        
        var tierCount = _spawnData.Data.Data.Length;

        List<float> weights = new();

        float totalWeights = 0;
        foreach (var fruitModel in _spawnData.Data.Data)
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
                return _spawnData.Data.Data[i];
            }

            r -= calculatedWeight;
        }

        return null;
    }
    void OnFruitDrop(Fruit _)
    {
        AdvanceQueue();
        SpawnFruit(_fruitQueue.First());
    }

    public void ClearQueue()
    {
        _fruitQueue.Clear();
    }

    private void OnEnable()
    {
        FruitController.OnFruitDrop += OnFruitDrop;
    }

    private void OnDisable()
    {
        FruitController.OnFruitDrop -= OnFruitDrop;        
    }   
}
