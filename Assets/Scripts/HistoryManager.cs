using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    [SerializeField] Fruit _fruitPrefab;

    Stack<StateSnapshot> snapshotRecord = new ();

    void SaveSnapshot()
    {
        var snapshot = new StateSnapshot
        {
            ScoreState = ScoreManager.Instance.Score,
            FruitQueueState = FruitSpawner.Instance.Queue,
            FruitsState = GetFruitsState(),
            RandomState = UnityEngine.Random.state
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
        UnityEngine.Random.state = snapshot.RandomState;
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

    private void OnEnable()
    {
        FruitController.OnFruitDropStarted += FruitController_OnFruitDropStarted;
    }

    private void OnDisable()
    {
        FruitController.OnFruitDropStarted -= FruitController_OnFruitDropStarted;
    }

    private void FruitController_OnFruitDropStarted(Fruit _)
    {
        SaveSnapshot();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 20), "Undo"))
        {
            Debug.Log("Undoing...");
            if (snapshotRecord.Count > 0)
            {
                Debug.Log("Restoreing Snapshot " + (snapshotRecord.Count - 1));
                RestoreSnapshot(snapshotRecord.Pop());
            }
        }
    }
}