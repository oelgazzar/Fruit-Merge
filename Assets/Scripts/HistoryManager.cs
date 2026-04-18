using System.Collections.Generic;
using UnityEngine;

/*
 * Saves snapshots of every move
 * Undo functionality
 */

public class HistoryManager : MonoBehaviour
{
    [SerializeField] Fruit _fruitPrefab;

    public static HistoryManager Instance { get; private set; }

    readonly Stack<StateSnapshot> snapshotRecord = new();

    private void Awake()
    {
        Instance = this;
    }

    void SaveSnapshot()
    {
        var snapshot = new StateSnapshot
        {
            ScoreState = ScoreManager.Instance.Score,
            FruitQueueState = FruitSpawner.Instance.Queue,
            FruitsState = GetFruitsState(),
            RandomState = Random.state
        };

        snapshotRecord.Push(snapshot);
    }

    FruitState[] GetFruitsState()
    {
        var fruits = FruitTracker.Instance.ActiveFruits;
        var fruitsState = new FruitState[fruits.Length];

        for (var i = 0; i < fruits.Length; i++)
        {
            var fruit = fruits[i];
            var rb = fruit.GetComponent<Rigidbody2D>();
            var fruitState = new FruitState()
            {
                Model = fruit.Model,
                Position = fruit.transform.position,
                Rotation = fruit.transform.rotation,
                Velocity = rb.linearVelocity,
                AngularVelocity = rb.angularVelocity,
                IsSleeping = rb.IsSleeping()
            };
            fruitsState[i] = fruitState;
        }

        return fruitsState;
    }

    void RestoreSnapshot(StateSnapshot snapshot)
    {
        Physics2D.simulationMode = SimulationMode2D.Script;
        ScoreManager.Instance.RestoreScore(snapshot.ScoreState);
        FruitSpawner.Instance.RestoreQueue(snapshot.FruitQueueState);
        SetFruitsState(snapshot.FruitsState);
        Random.state = snapshot.RandomState;
        Physics2D.Simulate(Time.fixedDeltaTime);
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
    }   

    void SetFruitsState(FruitState[] fruitsState)
    {
        var fruits = new Fruit[fruitsState.Length];

        for (var i = 0; i < fruitsState.Length; i++)
        {
            var fruitState = fruitsState[i];
            var fruit = Instantiate(_fruitPrefab, fruitState.Position, fruitState.Rotation);
            fruit.Init(fruitState.Model);
            var rb = fruit.GetComponent<Rigidbody2D>();
            rb.linearVelocity = fruitState.Velocity;
            rb.angularVelocity = fruitState.AngularVelocity;
            rb.simulated = true;
            fruits[i] = fruit;
        }

        FruitTracker.Instance.RestoreActiveFruits(fruits);
    }

    private void OnFruitDropStarted(Fruit _)
    {
        SaveSnapshot();
    }

    public void Undo()
    {
        if (snapshotRecord.Count > 0)
            RestoreSnapshot(snapshotRecord.Pop());
    }

    private void OnEnable()
    {
        FruitController.OnFruitDropStarted += OnFruitDropStarted;
    }

    private void OnDisable()
    {
        FruitController.OnFruitDropStarted -= OnFruitDropStarted;
    }    
}